## CreateQR

A simple C# Console app to create a QR Code that when viewed from a phone, a linked file is downloaded to the phone.

```
Usage:
  CreateQR [--link=<url>] [--name=<filename>] [--location=<dir>]

Options:
  --link, -l             URL or text to encode. Default is the embedded APK URL.
  --name, --file, -n, -f File name for the generated QR (with or without extension). Default: qrcode1
  --location, -o         Directory to save the file. Default: c:\temp
  -h, --help             Show this help message.
```

Uses Nuget package [QRCoder](https://www.nuget.org/packages/QRCoder)
