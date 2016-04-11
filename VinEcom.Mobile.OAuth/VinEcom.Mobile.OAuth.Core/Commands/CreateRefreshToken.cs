using ADR.Mobile.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Messaging;

namespace VinEcom.Mobile.OAuth.Core.Commands
{
    public class CreateRefreshToken : ICommand, IMessageSessionProvider
    {
        public CreateRefreshToken()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public Guid RefreshToken { get; set; }
        public string ClientId { get; set; }
        public string AuthenticationTicket { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset ExpiredTime { get; set; }
        public TimeSpan ExpiresIn
        {
            get
            {
                return this.ExpiredTime.Subtract(this.CreatedTime);
            }
        }

        public string SessionId
        {
            get { return RefreshToken.ToString(); }
        }
    }
}
