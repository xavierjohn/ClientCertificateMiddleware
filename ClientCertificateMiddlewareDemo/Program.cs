using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System;
using Microsoft.Extensions.Configuration;

namespace ClientCertificateMiddlewareDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var whb = new WebHostBuilder();

            var environment = whb.GetSetting("environment");
            var subjectName = GetCertificateSubjectNameBasedOnEnvironment(environment);
            var certificate = GetServiceCertificate(subjectName);

            var host = whb.UseKestrel(options =>
                {
                    var httpsOptions = new HttpsConnectionFilterOptions();
                    httpsOptions.ServerCertificate = certificate;
                    httpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                    httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls;
                    options.UseHttps(httpsOptions);
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("https://*:4430")
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }

        private static X509Certificate2 GetServiceCertificate(string subjectName)
        {
            var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            var certCollection = certStore.Certificates.Find(
                                       X509FindType.FindBySubjectDistinguishedName, subjectName, true);
            // Get the first cert with the thumbprint
            X509Certificate2 certificate = null;
            if (certCollection.Count > 0)
            {
                certificate = certCollection[0];
            }
            certStore.Close();
            return certificate;
        }

        private static string GetCertificateSubjectNameBasedOnEnvironment(string environment)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: false);

            var configuration = builder.Build();
            return configuration["ServerCertificateSubject"];
        }
    }
}
