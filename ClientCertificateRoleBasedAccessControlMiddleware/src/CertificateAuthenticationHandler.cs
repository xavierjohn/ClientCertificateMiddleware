// <copyright file="CertificateAuthenticationHandler.cs" company="CWiz Software">
// Copyright (c) CWiz Software. All rights reserved.
// </copyright>

namespace CWiz.ClientCertificateRoleBasedAccessControlMiddlewarej
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography.X509Certificates;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class CertificateAuthenticationHandler : AuthenticationHandler<CertficateAuthenticationOptions>
    {
        public CertificateAuthenticationHandler(IOptionsMonitor<CertficateAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IDataProtectionProvider dataProtection, ISystemClock clock)
            : base(options, logger, encoder, clock)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var certificate = Context.Connection.ClientCertificate;
            if (certificate != null && certificate.Verify())
            {
                var roles = GetRolesFromFirstMatchingCertificate(certificate);
                if (roles?.Length > 0)
                {
                    var claims = new List<Claim>();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var userIdentity = new ClaimsIdentity(claims, Options.Challenge);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    var ticket = new AuthenticationTicket(userPrincipal, new AuthenticationProperties(), Options.Challenge);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return Task.FromResult(AuthenticateResult.NoResult());
        }

        private string[] GetRolesFromFirstMatchingCertificate(X509Certificate2 certificate)
        {
            var roles = Options.CertificatesAndRoles
                .Where(r => r.Issuer == certificate.Issuer && r.Subject == certificate.Subject)
                .Select(r => r.Roles).FirstOrDefault();

            return roles;
        }
    }
}
