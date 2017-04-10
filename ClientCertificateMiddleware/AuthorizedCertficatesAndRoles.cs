using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCertificateMiddleware
{
    public class AuthorizedCertficatesAndRoles
    {
        public CertificateAndRoles[] CertificatesAndRoles { get; set; }

        public class CertificateAndRoles
        {
            public string Subject { get; set; }
            public string Issuer { get; set; }
            public string[] Roles { get; set; }
        }
    }
}
