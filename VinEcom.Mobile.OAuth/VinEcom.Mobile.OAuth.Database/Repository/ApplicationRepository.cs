using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public class ApplicationRepository : IApplicationRepository
    {
        protected OAuthDbContext dbContext;

        public ApplicationRepository(Func<OAuthDbContext> dbContextFactory)
        {
            this.dbContext = dbContextFactory.Invoke();
        }

        public bool IsExist(Guid appId, string appSecret)
        {
            return this.dbContext.Applications.FirstOrDefault(u => u.AppId == appId && u.AppSecret == appSecret && u.IsActive) != null;
        }

        public IEnumerable<ApplicationClaim> GetApplicationClaims(Guid appId)
        {
            return this.dbContext.ApplicationClaims.Where(u => u.AppId == appId);
        }
    }
}
