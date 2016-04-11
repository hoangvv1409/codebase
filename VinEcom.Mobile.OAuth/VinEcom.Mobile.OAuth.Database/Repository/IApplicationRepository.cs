using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public interface IApplicationRepository
    {
        bool IsExist(Guid appId, string appSecret);
        IEnumerable<ApplicationClaim> GetApplicationClaims(Guid appId);
    }
}
