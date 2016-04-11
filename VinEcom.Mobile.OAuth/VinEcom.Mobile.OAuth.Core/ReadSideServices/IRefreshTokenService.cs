using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core.ReadSideServices
{
    public interface IRefreshTokenService
    {
        string GetAuthenticationTicket(Guid refreshToken, string clientId);
    }
}
