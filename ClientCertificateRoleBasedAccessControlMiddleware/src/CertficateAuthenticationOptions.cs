// <copyright file="CertficateAuthenticationOptions.cs" company="CWiz Software.">
// Copyright (c) CWiz Software. All rights reserved.
// </copyright>

namespace CWiz.ClientCertificateRoleBasedAccessControlMiddlewarej
{
    using Microsoft.AspNetCore.Authentication;

    public class CertficateAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = CertificateAuthenticationDefaults.AuthenticationScheme;

        public CertificateAndRoles[] CertificatesAndRoles { get; set; }

        public class CertificateAndRoles
        {
            public string Subject { get; set; }
            public string Issuer { get; set; }
            public string[] Roles { get; set; }
        }
    }
}
