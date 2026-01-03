## CreateQR

A simple C# Console app to create a QR Code that when viewed from a phone, a linked file is downloaded to the phone.

---

## Try Me
F
## 🚀 Run in GitHub Codespaces


Click the badge below to open this project in a ready-to-use development environment in your browser:

[![Open in NEW GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/djaus2/CreateQR) 

 with:

```bash
dotnet build # May need to do twice.
dotnet run -- <params>
# Subsequent runs:
dotnet run --no-build 
dotnet run --no-build -- "https://example.com"
dotnet run --no-build -- --link=https://example.com --name=myqr --location=/tmp
dotnet run --no-build -- -l https://example.com -n myqr -o /tmp
```

- In Linux use /tmp/qr as the location 
- Set Instance timeout to 10 minutes. Set Instance gets deleted after 2 days.
- Once you have run the app, you can download the QR code image from the `c:\temp` or '/tmp/qr' folder in the Codespace.

- [http://github.com/codepaces](http://github.com/codepaces) to show your instances. You cann reload or delete from there. Once created using link as above.
- In GitHub can set Codspaces Timeout (default 30 minutes) and Delete after (default 30 days):

> Per-user (applies to new codespaces _you_ create)  
> _Can be set at Corporate Level for all users in an organization._

### Timeout period

```
On GitHub: Profile → Settings → Codespaces → Default idle timeout.  
Choose 5–240 minutes (4 hours), then Save.
```
### Deletion after period

```
On GitHub: Profile → Settings → Codespaces → Default retention period.
Choose 0–30 days and Save.

0 days = delete immediately when the codespace stops or times out.
The countdown resets each time you reconnect; deletion is independent of unpushed changes. [docs.github.com], [docs.github.com]
```

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
