using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure
{
    public interface IProcessor
    {
        void Start();
        void Stop();
    }
}
