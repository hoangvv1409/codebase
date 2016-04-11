using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public interface IAdminUserRepository
    {
        bool IsExist(string email, string password);
        IEnumerable<UserClaim> GetAdminUserClaims(string email);
    }
}
