using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Database;
using VinEcom.Mobile.OAuth.Database.Repository;

namespace VinEcom.Mobile.OAuth.Service
{
    public class ApplicationService : IAppService
    {
        private IApplicationRepository applicationRepository;

        public ApplicationService(IApplicationRepository applicationRepository)
        {
            this.applicationRepository = applicationRepository;
        }

        public bool IsExist(Guid appId, string appSecret)
        {
            return this.applicationRepository.IsExist(appId, appSecret);
        }

        public IEnumerable<ClaimModel> GetApplicationClaims(Guid appId)
        {
            var appClaims = this.applicationRepository.GetApplicationClaims(appId);
            var claimModel = appClaims.Select(c => new ClaimModel(c.ClaimType, c.ClaimValue)).ToList();

            return claimModel;
        }
    }
}
