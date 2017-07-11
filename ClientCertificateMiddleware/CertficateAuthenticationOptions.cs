using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWiz.ClientCertificateMiddleware
{
    public class CertficateAuthenticationOptions : AuthenticationOptions
    {
        public CertficateAuthenticationOptions()
        {
            AuthenticationScheme = "Certificate";
            AutomaticAuthenticate = true;
            AutomaticChallenge = true;
        }

        public CertificateAndRoles[] CertificatesAndRoles { get; set; }

        public class CertificateAndRoles
        {
            public string Subject { get; set; }
            public string Issuer { get; set; }
            public string[] Roles { get; set; }
        }
    }
}
