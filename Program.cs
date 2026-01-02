using System;
using System.IO;
using QRCoder;
using TextCopy;

class Program
{
    static void Main(string[] args)
    {
        const string defaultLink = "https://weblocation/androidpackage/apk/androidpackage.apk";
        const string defaultName = "qrcode";
        const string defaultExt = "png";
        const string defaultLocation = @"c:\temp";
        const int numParamChars = 4; // number of chars to consider for parameter matching

        string? link = null;
        string? name = null;
        string? location = null;
        bool copyToClipboard = true;

        if(args.Length == 0)
        {
            Console.WriteLine("No command-line arguments provided. Press Enter to accept the shown defaults.");
            Console.Write($"Link [{defaultLink}]: ");
            var input = Console.ReadLine() ?? string.Empty;
            link = string.IsNullOrWhiteSpace(input) ? defaultLink : input.Trim();

            Console.Write($"Name [{defaultName}]: ");
            input = Console.ReadLine() ?? string.Empty;
            name = string.IsNullOrWhiteSpace(input) ? defaultName : input.Trim();

            Console.Write($"Location [{defaultLocation}]: ");
            input = Console.ReadLine() ?? string.Empty;
            location = string.IsNullOrWhiteSpace(input) ? defaultLocation : input.Trim();

            Console.Write($"Copy to clipboard? [Y/n] (default: {(copyToClipboard ? "Y" : "n")}): ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input))
            {
                var v = input.Trim().ToLowerInvariant();
                copyToClipboard = v == "y" || v == "yes" || v == "1";
            }
        }
        else if (args is { Length: > 0 } &&
            (args[0] == "-h" || args[0] == "--help"))
        {
            ShowHelp();
            return;
        }

        bool gotURi = false;
        if (args is { Length: 1 })
        {
            string input = args[0];
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri? uriResult) && uriResult is not null)
            {
                var scheme = uriResult.Scheme;
                if (scheme == Uri.UriSchemeHttp || scheme == Uri.UriSchemeHttps)
                {
                    link = uriResult.ToString();
                    gotURi = true;
                }
            }
        }

        if (!gotURi)
        {
            // Parse named (--key=value or --key value) and positional arguments.
            for (int i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (a.StartsWith("--", StringComparison.Ordinal))
                {
                    int eq = a.IndexOf('=');
                    string? key = eq >= 0 ? a.Substring(2, eq - 2) : a.Substring(2);
                    string? val = eq >= 0 ? a.Substring(eq + 1) : null;

                    if (string.IsNullOrEmpty(key)) continue;

                    key = key.Trim();
                    if (key.Length > numParamChars) key = key.Substring(0, numParamChars);
                    string keyLower = key.ToLowerInvariant();

                    if (val is null && i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.Ordinal))
                        val = args[++i];

                    switch (keyLower)
                    {
                        case "link":
                        case "limk":
                            link = val;
                            break;
                        case "file":
                        case "name":
                            name = val;
                            break;
                        case "loca":
                        case "loc":
                        case "locn":
                            location = val;
                            break;
                        case "copy":
                            copyToClipboard = val is null ? true : ParseBool(val);
                            break;
                        case "help":
                            ShowHelp();
                            return;
                        default:
                            Console.Error.WriteLine($"Warning: unknown option --{key} ignored.");
                            break;
                    }
                }
                else if (a.StartsWith("-", StringComparison.Ordinal))
                {
                    int eq = a.IndexOf('=');
                    string? key = null;
                    string? val = null;

                    if (eq >= 0 && eq < a.Length)
                    {
                        // "-k=value"
                        if (a.Length >= 2)
                        {
                            key = a.Substring(1, 1);
                            val = a.Substring(eq + 1);
                        }
                    }
                    else if (a.Length == 2)
                    {
                        // "-k" possibly followed by value
                        key = a.Substring(1, 1);
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.Ordinal))
                        {
                            val = args[++i];
                        }
                    }
                    else
                    {
                        // ignore combined short options like -cn
                        continue;
                    }

                    if (string.IsNullOrEmpty(key)) continue;

                    switch (key.ToLowerInvariant())
                    {
                        case "l":
                            link = val;
                            break;
                        case "n":
                        case "f":
                            name = val;
                            break;
                        case "o":
                        case "0":
                            location = val;
                            break;
                        case "c":
                            copyToClipboard = val is null ? true : ParseBool(val);
                            break;
                        case "h":
                            ShowHelp();
                            return;
                        default:
                            Console.Error.WriteLine($"Warning: unknown option -{key} ignored.");
                            break;
                    }
                }
                else
                {
                    // positional fallback: link, name, location
                    if (link is null) link = a;
                    else if (name is null) name = a;
                    else if (location is null) location = a;
                }
            }
        }

        // Apply defaults
        link ??= defaultLink;
        name ??= defaultName;
        location ??= defaultLocation;

        try
        {
            // If user supplied a rooted path as the name, treat it as the full file path.
            string qrPath;
            if (Path.IsPathRooted(name))
            {
                qrPath = name;
            }
            else
            {
                // Ensure extension exists
                if (!Path.HasExtension(name))
                {
                    name = $"{name}.{defaultExt}";
                }

                // Ensure directory exists
                Directory.CreateDirectory(location);
                qrPath = Path.Combine(location, name);
            }

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            byte[] qrBytes = qrCode.GetGraphic(20);
            File.WriteAllBytes(qrPath, qrBytes);

            Console.WriteLine($"QR code image for {link} saved as {qrPath}");

            if (copyToClipboard)
            {
                try
                {
                    // Use TextCopy for cross-platform clipboard operations.
                    string base64 = Convert.ToBase64String(qrBytes);
                    ClipboardService.SetText(base64);
                    Console.WriteLine("QR code file contents copied to clipboard as Base64 text.");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to copy to clipboard via TextCopy: {ex.Message}");
                    Console.Error.WriteLine("If running headless on Linux, ensure a clipboard helper (wl-clipboard/xclip/xsel) is available.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to create QR code: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    static bool ParseBool(string v) =>
        bool.TryParse(v, out var b) ? b : v == "1" || v.Equals("yes", StringComparison.OrdinalIgnoreCase);

    static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  CreateQR [<url>] [--link=<url>] [--name=<filename>] [--location=<dir>] [--copy]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  <url> If single parameter assume is the URL. Ignore other params.");
        Console.WriteLine("  --link, -l             URL or text to encode. Default is the embedded APK URL.");
        Console.WriteLine("  --name, --file, -n, -f File name for the generated QR (with or without extension). Default: qrcode");
        Console.WriteLine("  --location, -o         Directory to save the file. Default: c:\\temp");
        Console.WriteLine("  --copy, -c             Copy Base64 file contents to clipboard (uses TextCopy).");
        Console.WriteLine("  -h, --help             Show this help message.");
    }
}