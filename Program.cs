using System;
using System.IO;
using QRCoder;

class Program
{
    static void Main(string[] args)
    {
        const string defaultLink = "https://davidjones.sportronics.com.au/apps/athsvideorecording/apk/sportronics.athsvideorecording.apk";
        const string defaultName = "qrcode1";
        const string defaultExt = "png";
        const string defaultLocation = @"c:\temp";

        string? link = null;
        string? name = null;
        string? location = null;

        if (args is { Length: > 0 } &&
            (args[0] == "-h" || args[0] == "--help"))
        {
            ShowHelp();
            return;
        }

        // Parse named (--key=value or --key value) and positional arguments.
        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.StartsWith("--", StringComparison.Ordinal))
            {
                int eq = a.IndexOf('=');
                string? key = eq >= 0 ? a.Substring(2, eq - 2) : a.Substring(2);
                string? val = eq >= 0 ? a.Substring(eq + 1) : null;

                if(string.IsNullOrEmpty(key))
                {
                    continue;
                }
                else if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (key.ToLowerInvariant())
                {
                    case "link":
                    case "limk": // tolerate common typo
                        link = val;
                        break;
                    case "filename":
                    case "name":
                        name = val;
                        break;
                    case "location":
                    case "loc":
                    case "locn":
                        location = val;
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

                if (eq ==2)
                {
                    // "-k=value" -> key = "k", val = "value"
                    key = a.Substring(1,1);
                    val = a.Substring(eq + 1);
                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }
                    else if (string.IsNullOrEmpty(val))
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                if (val is null && i + 1 < args.Length && !args[i + 1].StartsWith("-", StringComparison.Ordinal))
                {
                    val = args[++i];
                }

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
                    case "h":
                        ShowHelp();
                        return;
                    default:
                        Console.Error.WriteLine($"Warning: unknown option --{key} ignored.");
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

            Console.WriteLine($"QR code saved as {qrPath}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to create QR code: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  Program.exe [--link=<url>] [--name=<filename>] [--location=<dir>]");
        Console.WriteLine("  Program.exe <link> <name> <location>   (positional fallback)");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --link, -l       URL or text to encode. Default is the embedded APK URL.");
        Console.WriteLine("  --name, -n       File name for the generated QR (with or without extension). Default: qrcode1");
        Console.WriteLine("  --location, -o   Directory to save the file. Default: c:\\temp");
        Console.WriteLine("  -h, --help   Show this help message.");
    }
}