using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Events.User;
using VinEcom.MobileNotification.Infrastructure.Messaging.Handling;

namespace VinEcom.MobileNotification.Handlers
{
    public class UserEventHandler :
        IEventHandler<AdrPointsUsed>,
        IEventHandler<AdrPointAdded>,
        IEventHandler<AdrPointsRefunded>,
        IEventHandler<EmailAdded>,
        IEventHandler<MobileAdded>,
        IEventHandler<PasswordChanged>,
        IEventHandler<TwoStepsActivated>
    {
        public void Handle(AdrPointsUsed @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(AdrPointAdded @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(AdrPointsRefunded @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(EmailAdded @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(MobileAdded @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(PasswordChanged @event)
        {
            throw new NotImplementedException();
        }

        public void Handle(TwoStepsActivated @event)
        {
            throw new NotImplementedException();
        }
    }
}
