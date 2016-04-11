using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Core.ReadSideServices
{
    public interface IDeviceService
    {
        UserDevice GetDevice(long userId, string deviceId);
    }
}
