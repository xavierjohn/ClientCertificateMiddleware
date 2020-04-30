// <copyright file="CertificateAuthenticationExtensions.cs" company="CWiz Software">
// Copyright (c) CWiz Software. All rights reserved.
// </copyright>

namespace CWiz.ClientCertificateRoleBasedAccessControlMiddlewarej
{
    using System;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;

    public static class CertificateAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCertificateAuthentication(this AuthenticationBuilder builder, Action<CertficateAuthenticationOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<CertficateAuthenticationOptions>, CertificateAuthenticationPostConfigureOptions>());
            return builder.AddScheme<CertficateAuthenticationOptions, CertificateAuthenticationHandler>(CertificateAuthenticationDefaults.AuthenticationScheme, "Certificate Authentication", configureOptions);
        }
    }
}
