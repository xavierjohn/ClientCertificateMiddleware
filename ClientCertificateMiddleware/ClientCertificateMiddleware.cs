
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Linq;

namespace ClientCertificateMiddleware
{
    public class ClientCertificateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuthorizedCertficatesAndRoles _authorizedCertficatesAndRoles;
        private readonly ILogger _logger;

        public ClientCertificateMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<AuthorizedCertficatesAndRoles> options)
        {
            _next = next;
            _authorizedCertficatesAndRoles = options.Value;
            _logger = loggerFactory.CreateLogger("ClientCertificateMiddleware");
        }

        public async Task Invoke(HttpContext context)
        {
            //Validate the cert here
            var certificate = context.Connection.ClientCertificate;

            var roles = IfValidCertificateGetRoles(certificate);
            if (roles.Length > 0)
            {
                //TODO: Create principle from roles.

                //Invoke the next middleware in the pipeline
                await _next.Invoke(context);
            }
            else
            {
                //Stop the pipeline here.
                _logger.LogInformation("Certificate with thumbprint " + certificate.Thumbprint + " is not valid");
                context.Response.StatusCode = 403;
            }
        }


        private string[] IfValidCertificateGetRoles(X509Certificate2 certificate)
        {
            // This example does NOT test that this certificate is chained to a Trusted Root Authority (or revoked) on the server 
            // and it allows for self signed certificates
            //
            if (null == certificate) return new string[0];
            if (TimeValidityOfCertificateExpired(certificate)) return new string[0];
            var roles = (_authorizedCertficatesAndRoles.CertificatesAndRoles
                .Where(r => r.Issuer == certificate.Issuer && r.Subject == certificate.Subject)
                .Select(r => r.Roles)).FirstOrDefault();

            return roles;
        }

        private bool TimeValidityOfCertificateExpired(X509Certificate2 certificate)
        {
            if (DateTime.Compare(DateTime.UtcNow, certificate.NotBefore) < 0 || DateTime.Compare(DateTime.UtcNow, certificate.NotAfter) > 0)
            {
                _logger.LogDebug("Certificate with thumbprint " + certificate.Thumbprint + " is not within a valid time window.");
                return true;
            }
            return false;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ClientCertificateMiddlewareExtensions
    {
        public static IApplicationBuilder UseClientCertificateMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientCertificateMiddleware>();
        }

        public static IApplicationBuilder UseClientCertMiddleware(this IApplicationBuilder builder, IOptions<AuthorizedCertficatesAndRoles> options)
        {

            return builder.UseMiddleware<ClientCertificateMiddleware>(options);
        }
    }
}
