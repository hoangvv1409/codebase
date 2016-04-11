using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core.ReadSideServices
{
    public interface IAppService
    {
        bool IsExist(Guid appId, string appSecret);
        IEnumerable<ClaimModel> GetApplicationClaims(Guid appId);
    }
}
