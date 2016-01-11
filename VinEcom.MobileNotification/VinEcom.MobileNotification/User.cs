using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.Events;
using VinEcom.MobileNotification.Infrastructure.Messaging;

namespace VinEcom.MobileNotification
{
    public class User : Aggregate
    {
        public long UserId { get; private set; }
        public decimal? AdrPoint { get; private set; }
        public long SOID { get; private set; }
        public UserState UserState { get; private set; }
        public DateTime CreatedDate { get; private set; }

        protected User()
            : this(Guid.NewGuid())
        { }

        protected User(Guid id)
            : base(id)
        { }

        public User(long userId, UserState userState)
            : this()
        {
            this.UserId = userId;
            this.UserState = userState;
            this.CreatedDate = DateTime.Now;
        }

        public User(long userId, UserState userState, decimal adrPoint)
            : this(userId, userState)
        {
            this.AdrPoint = adrPoint;
        }

        public User(long userId, UserState userState, decimal adrPoint, long soid)
            : this(userId, userState, adrPoint)
        {
            this.SOID = soid;
        }

        protected override IEvent EventFactory()
        {
            switch (this.UserState)
            {
                case UserState.PasswordChanged:
                    return new PasswordChanged(this.Id)
                    {
                        UserId = this.UserId
                    };
                case UserState.TwoStepsActivated:
                    return new TwoStepsActivated(this.Id)
                    {
                        UserId = this.UserId
                    };
                case UserState.EmailAdded:
                    return new EmailAdded(this.Id)
                    {
                        UserId = this.UserId
                    };
                case UserState.MobileAdded:
                    return new MobileAdded(this.Id)
                    {
                        UserId = UserId
                    };
                case UserState.AdrPointAdded:
                    return new AdrPointAdded(this.Id)
                    {
                        UserId = this.UserId,
                        AdrPoints = (decimal)this.AdrPoint
                    };
                case UserState.AdrPointsUsed:
                    return new AdrPointsUsed(this.Id)
                    {
                        UserId = this.UserId,
                        AdrPoints = (decimal)this.AdrPoint,
                        SOID = this.SOID
                    };
                case UserState.AdrPointsRefunded:
                    return new AdrPointsRefunded(this.Id)
                    {
                        UserId = this.UserId,
                        AdrPoints = (decimal)this.AdrPoint,
                        SOID = this.SOID
                    };
                default:
                    return null;
            }
        }
    }
}
