## CreateQR

A simple C# Console app to create a QR Code that when viewed from a phone, a linked file is downloaded to the phone.

---

## Try Me

## 🚀 Run in GitHub Codespaces


Click the badge below to open this project in a ready-to-use development environment in your browser:

[![Open in NEW GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/djaus2/CreateQR)   NEW

[![Open in existing GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://effective-fortnight-5gvxjrq6wj4cp77r.github.dev/) EXISTING

 with:

```bash
dotnet build
dotnet run -- <params>
# Subsequent runs:
dotnet run --no-build 
dotnet run --no-build -- "https://example.com"
dotnet run --no-build -- --link=https://example.com --name=myqr --location=/tmp
dotnet run --no-build -- -l https://example.com -n myqr -o /tmp
```

- Leave image folder as ```/tmp/qr``` as this shows in **Explore**. 
- And instance timeout is 5 minutes.  
- Once you have run the app, you can download the QR code image from the `/tmp/qr` folder in the Codespace.
- Instance timeout is 5 minutes. Instance gets deleted after 2 days.

---



```
Usage:
  CreateQR [<url>] [--link=<url>] [--name=<filename>] [--location=<dir>]

Options:
  <url>                  If single parameter assume is the URL. Ignore other params.
  --link, -l             URL or text to encode. Default is the embedded APK URL.
  --name, --file, -n, -f File name for the generated QR (with or without extension). Default: qrcode
  --location, -o         Directory to save the file. Default: c:\temp
  -h, --help             Show this help message.
```

Uses Nuget package [QRCoder](https://www.nuget.org/packages/QRCoder)

***Was used to create a QR Code image for link to Android APK package for sideloading on the phone.*** 
