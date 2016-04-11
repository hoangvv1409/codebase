using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;
using VinEcom.Mobile.OAuth.Core.ReadSideServices;
using VinEcom.Mobile.OAuth.Database.Repository;

namespace VinEcom.Mobile.OAuth.Service
{
    public class AdminUserService : IAdminUserService
    {
        private IAdminUserRepository adminRepository;

        public AdminUserService(IAdminUserRepository adminRepository)
        {
            this.adminRepository = adminRepository;
        }

        public bool IsExist(string email, string password)
        {
            return this.adminRepository.IsExist(email, password);
        }

        public IEnumerable<ClaimModel> GetAdminUserClaims(string email)
        {
            var adminClaims = this.adminRepository.GetAdminUserClaims(email);
            var claimModel = adminClaims.Select(c => new ClaimModel(c.ClaimType, c.ClaimValue)).ToList();
            claimModel.Add(new ClaimModel("Email", email));

            return claimModel;
        }
    }
}
