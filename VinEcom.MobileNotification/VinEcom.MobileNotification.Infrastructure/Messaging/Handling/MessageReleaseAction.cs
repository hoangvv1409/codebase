using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.MobileNotification.Infrastructure.Messaging.Handling
{
    public class MessageReleaseAction
    {
        private MessageReleaseActionKind messageKind;

        protected MessageReleaseAction(MessageReleaseActionKind actionKind)
        {
        }

        public MessageReleaseActionKind Kind
        {
            get { return this.messageKind; }
            private set
            {
                this.messageKind = value;
            }
        }

        public string DeadLetterReason
        {
            get;
            private set;
        }

        public string DeadLetterDescription
        {
            get;
            private set;
        }

        public static readonly MessageReleaseAction CompleteMessage = new MessageReleaseAction(MessageReleaseActionKind.Complete);
        public static readonly MessageReleaseAction AbandonMessage = new MessageReleaseAction(MessageReleaseActionKind.Abandon);

        public static MessageReleaseAction DeadLetterMessage(string reason, string description)
        {
            return new MessageReleaseAction(MessageReleaseActionKind.DeadLetter)
            {
                DeadLetterReason = reason,
                DeadLetterDescription = description
            };
        }
    }

    public enum MessageReleaseActionKind
    {
        Complete,
        DeadLetter,
        Abandon
    }
}
