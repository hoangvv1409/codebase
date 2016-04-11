using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core.ReadSideServices
{
    public interface IAdminUserService
    {
        bool IsExist(string email, string password);
        IEnumerable<ClaimModel> GetAdminUserClaims(string email);
    }
}
