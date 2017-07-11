
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;

namespace CWiz.ClientCertificateMiddleware
{
    public class ClientCertificateMiddleware : AuthenticationMiddleware<CertficateAuthenticationOptions>
    {
        public ClientCertificateMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, UrlEncoder encoder, IOptions<CertficateAuthenticationOptions> options)
            : base(next, options, loggerFactory, encoder)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));
            if (options == null)
                throw new ArgumentNullException(nameof(options));
        }


        protected override AuthenticationHandler<CertficateAuthenticationOptions> CreateHandler()
        {
            return new ClientCertificateHandler();
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ClientCertificateMiddlewareExtensions
    {
        public static IApplicationBuilder UseClientCertificateMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ClientCertificateMiddleware>();
        }

        public static IApplicationBuilder UseClientCertMiddleware(this IApplicationBuilder builder, IOptions<CertficateAuthenticationOptions> options)
        {

            return builder.UseMiddleware<ClientCertificateMiddleware>(options);
        }
    }
}
