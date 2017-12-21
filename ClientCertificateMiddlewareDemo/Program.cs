using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ClientCertificateMiddlewareDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var whb = WebHost.CreateDefaultBuilder(args);

            var environment = whb.GetSetting("environment");
            var subjectName = GetCertificateSubjectNameBasedOnEnvironment(environment);
            var certificate = GetServiceCertificate(subjectName);

            var host = whb.UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(new IPEndPoint(IPAddress.Loopback, 4430), listenOptions =>
                    {
                        var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                        {
                            ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                            SslProtocols = System.Security.Authentication.SslProtocols.Tls,
                            ServerCertificate = certificate
                        };
                        listenOptions.UseHttps(httpsConnectionAdapterOptions);
                    });
                })
                .Build();
            host.Run();
        }

        private static X509Certificate2 GetServiceCertificate(string subjectName)
        {
            using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                                           X509FindType.FindBySubjectDistinguishedName, subjectName, true);
                // Get the first certificate
                X509Certificate2 certificate = null;
                if (certCollection.Count > 0)
                {
                    certificate = certCollection[0];
                }
                return certificate;
            }
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
