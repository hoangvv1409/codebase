using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging
{
    public interface IProcessor
    {
        void Start();
        void Stop();
    }
}
