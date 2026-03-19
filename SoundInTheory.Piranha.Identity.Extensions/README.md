# Piranha Identity Extensions

Module that extends some of the identity features of Piranha.

This module is still in development and not recommended for public use.

## Key Features
* Login Page themes designed to suit any brand.
* Reset password functionality for manager admins.

## Usage - Reset Password

The reset password functionality uses the Piranha Emails module to send reset password emails.

Set up Email Sender in `appsettings.json`. SMTP or Mailgun is currently supported.

```jsonc
{
    "PiranhaEmails": {
        "Senders": {
            "TestSender": {
                "FromEmail": "example@example.com",
                "FromName": "Mr Example",
                "SendToWhiteList": [],
                "SmtpSettings": {
                    "SmtpUser": "example@example.com",
                    "SmtpPassword": "password",
                    "SmtpServer": "smtp.gmail.com",
                    "SmtpPort": 465,
                    "UseSsl": true,
                    "RequiresAuthentication": true
                },
                //or
                "MailgunSettings": {
                    "MailgunSendingKey": "sendingkeyhere",
                    "MailgunDomain": "domain.example",
                    "MailgunRegion": "USAorEU"
                }
            }           
        }
    }
}

```

Add emails and identity extensions on startup

```cs
builder.AddPiranha(options => 
{
    //other services here

    options.UseIdentityExtensions();
    options.Services.AddPiranhaEmails(builder)
        .AddEmailTemplates(typeof(IdentityExtensionsModule).Assembly);
})

app.UsePiranha(options => {
    //other services here

    options.UsePiranhaEmails();
    options.UseIdentityExtensions();
})
```

The contact us template should be visible in the manager. Edit as required.
Uses MJML and Handlebars

## Usage - Theming

Themes are a straightforward way of changing Piranha's login page. Create a theme that implements `ILoginTheme`

```cs
public class SoundInTheoryLoginTheme : ILoginTheme
{
    public string Title { get; set; } = "Sound in Theory Admin";
    public string BackgroundColor { get; set; } = "#150536";
    public string ButtonColor { get; set; } = "#ea6e2a";
    public string LogoPath { get; set; } = "/assets/img/piranha-login-logo.png";
    public string BgPath { get; set; } = "/assets/img/piranha-login-bg.png";
    public bool IncludeVersionNumber { get; set; } = false;
}
```

Then add to DI

```cs
options.UseIdentityExtensions();
options.Services.AddScoped<ILoginTheme, SoundInTheoryLoginTheme>();
```

Your theme should appear on the login page. The latest theme that is added to the service provider will be the one that is used. 
