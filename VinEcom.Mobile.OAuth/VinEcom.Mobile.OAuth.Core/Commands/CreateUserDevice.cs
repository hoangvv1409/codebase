using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure;
using ADR.Mobile.Infrastructure.Messaging;

namespace VinEcom.Mobile.OAuth.Core.Commands
{
    public class CreateUserDevice : ICommand, IMessageSessionProvider
    {
        public CreateUserDevice()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public long UserId { get; set; }

        public string SessionId
        {
            get { return UserId.ToString(); }
        }
    }
}
