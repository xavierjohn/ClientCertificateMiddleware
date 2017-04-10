# ClientCertificateMiddleware
Asp.net core Client Certificate Middleware

The Client Certificate Middleware will authorize a request based on the configured AuthorizedCertficatesAndRoles

Example:
```sh
  "AuthorizedCertficatesAndRoles": {
    "CertificateAndRoles": [
      {
        "Subject": "CN=localhost",
        "Issuer": "CN=localhost",
        "Roles": [ "User" ]
      },
      {
        "Subject": "CN=Store",
        "Issuer": "Me",
        "Roles": [ "User" ]
      }
    ]
  }
```

To run the demonstration, you need to install a certificate and gives its subject in the configuration.
```sh
"ServerCertificateSubject": "CN=localhost",
```

To get the certificates to work, don't run the demo under IIS Express. Instead run under the app 'ClientCertificateMiddlewareDemo'
Refer here for more information.
http://www.blinkingcaret.com/2017/03/01/https-asp-net-core/


Here is the code that sets up the use of client certificate
```sh
UseKestrel(options =>
    {
        var httpsOptions = new HttpsConnectionFilterOptions();
        httpsOptions.ServerCertificate = certificate;
        httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
        options.UseHttps(httpsOptions);
    }
```
### Future development
Future development would be to create a Claims Principal and add the roles from the configuration. This will allow restricting API based on different certificates.
