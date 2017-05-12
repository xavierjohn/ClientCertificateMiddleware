using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace ClientCertificateMiddleware
{
    internal class ClientCertificateHandler : AuthenticationHandler<CertficateAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var certificate = Context.Connection.ClientCertificate;
#if NET451
            if (certificate != null && certificate.Verify())
#else
            if (certificate != null)
#endif
            {
                var roles = GetRolesFromFirstMatchingCertificate(certificate);
                if (roles?.Length > 0)
                {
                    var claims = new List<Claim>();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var userIdentity = new ClaimsIdentity(claims, Options.AuthenticationScheme);
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    var ticket = new AuthenticationTicket(userPrincipal, new AuthenticationProperties(), Options.AuthenticationScheme);
                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }
            }

            return Task.FromResult(AuthenticateResult.Skip());
        }

        private string[] GetRolesFromFirstMatchingCertificate(X509Certificate2 certificate)
        {
            var roles = (Options.CertificatesAndRoles
                .Where(r => r.Issuer == certificate.Issuer && r.Subject == certificate.Subject)
                .Select(r => r.Roles)).FirstOrDefault();

            return roles;
        }
    }
}
