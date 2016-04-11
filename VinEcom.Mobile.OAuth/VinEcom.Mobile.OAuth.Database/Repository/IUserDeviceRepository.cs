using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.Mobile.OAuth.Core;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public interface IUserDeviceRepository
    {
        IEnumerable<string> GetListClientId(long userId);
        bool IsExist(long userId, string clientId);
        void Save(UserDevice userDevice);
    }
}
