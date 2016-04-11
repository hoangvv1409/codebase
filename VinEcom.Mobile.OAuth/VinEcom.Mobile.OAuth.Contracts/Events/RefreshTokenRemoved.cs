using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;

namespace VinEcom.Mobile.OAuth.Contracts.Events
{
    public class RefreshTokenRemoved : IEvent
    {
        public RefreshTokenRemoved()
        {
            this.SourceId = Guid.NewGuid();
        }

        public Guid SourceId { get; set; }
        public Guid RefreshToken { get; set; }
    }
}
