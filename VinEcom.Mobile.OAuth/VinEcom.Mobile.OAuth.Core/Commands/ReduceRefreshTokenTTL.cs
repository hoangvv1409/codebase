using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;

namespace VinEcom.Mobile.OAuth.Core.Commands
{
    public class ReduceRefreshTokenTTL : ICommand, IMessageSessionProvider
    {
        public ReduceRefreshTokenTTL()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string AuthenticationTicket { get; set; }
        public int TTL { get; set; }

        public string SessionId
        {
            get { return RefreshToken.ToString(); }
        }
    }
}
