using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;

namespace VinEcom.Mobile.OAuth.Core.Commands
{
    public class ForceUserLogin : ICommand, IMessageSessionProvider
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string ClientId { get; set; }
        public string SessionId
        {
            get { return UserId.ToString(); }
        }

        public ForceUserLogin()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
