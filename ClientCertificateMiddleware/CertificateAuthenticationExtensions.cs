using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace CWiz.ClientCertificateMiddleware
{
    public static class CertificateAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCertificateAuthentication(this AuthenticationBuilder builder, Action<CertficateAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CertficateAuthenticationOptions>, CertificateAuthenticationPostConfigureOptions>());
            return builder.AddScheme<CertficateAuthenticationOptions, CertificateAuthenticationHandler>(CertificateAuthenticationDefaults.AuthenticationScheme, "Certificate Authentication", configureOptions);
        }
    }
}
