using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADR.Mobile.Infrastructure.Messaging.Handling
{
    public interface ICommandHandlerRegistry
    {
        void Register(ICommandHandler handler);
    }
}
