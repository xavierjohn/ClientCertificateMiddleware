// <copyright file="CertificateAuthenticationPostConfigureOptions.cs" company="CWiz Software">
// Copyright (c) CWiz Software. All rights reserved.
// </copyright>

namespace CWiz.ClientCertificateRoleBasedAccessControlMiddlewarej
{
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Used to setup defaults for all <see cref="CertficateAuthenticationOptions"/>.
    /// </summary>
    public class CertificateAuthenticationPostConfigureOptions : IPostConfigureOptions<CertficateAuthenticationOptions>
    {
        /// <summary>
        /// Invoked to post configure a CertficateAuthenticationOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, CertficateAuthenticationOptions options)
        {

        }
    }
}
