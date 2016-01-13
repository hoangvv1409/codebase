using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification.Events
{
    public class PasswordChanged : IEvent
    {
        public PasswordChanged(Guid id)
        {
            this.SourceId = id;
        }

        public Guid SourceId { get; private set; }
        public long UserId { get; set; }

        public UserState UserState
        {
            get
            {
                return UserState.PasswordChanged;
            }
        }
    }
}
