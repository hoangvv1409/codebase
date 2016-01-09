using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Events.User;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public class UserNotification : Aggregate
    {
        public int UserId { get; private set; }
        public decimal AdrPoint { get; private set; }
        public long SOID { get; private set; }
        public UserState UserState { get; private set; }

        public UserNotification(int userId, UserState userState)
        {
            this.UserId = userId;
            this.UserState = userState;
        }

        public UserNotification(int userId, UserState userState, decimal adrPoint)
            : this(userId, userState)
        {
            this.AdrPoint = adrPoint;
        }

        public UserNotification(int userId, UserState userState, decimal adrPoint, long soid)
            : this(userId, userState, adrPoint)
        {
            this.SOID = soid;
        }

        protected override IEvent EventFactory()
        {
            switch (this.UserState)
            {
                case UserState.PasswordChanged:
                    return new PasswordChanged()
                    {
                        UserId = this.UserId
                    };
                case UserState.TwoStepsActivated:
                    return new TwoStepsActivated()
                    {
                        UserId = this.UserId
                    };
                case UserState.EmailAdded:
                    return new EmailAdded()
                    {
                        UserId = this.UserId
                    };
                case UserState.MobileAdded:
                    return new MobileAdded()
                    {
                        UserId = UserId
                    };
                case UserState.AdrPointAdded:
                    return new AdrPointAdded()
                    {
                        UserId = this.UserId,
                        AdrPoints = this.AdrPoint
                    };
                case UserState.AdrPointsUsed:
                    return new AdrPointsUsed()
                    {
                        UserId = this.UserId,
                        AdrPoints = this.AdrPoint
                    };
                case UserState.AdrPointsRefunded:
                    return new AdrPointsRefunded()
                    {
                        UserId = this.UserId,
                        AdrPoints = this.AdrPoint
                    };
                default:
                    return null;
            }
        }
    }
}
