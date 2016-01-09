using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VinEcom.MobileNotification.Infrastructure.Messaging.Handling
{
    public abstract class MessageProcessor : IProcessor, IDisposable
    {
        private const int MaxProcessingRetries = 5;
        private bool disposed;
        private bool started = false;
        private readonly IMessageReceiver receiver;
        private readonly ITextSerializer serializer;
        private readonly object lockObject = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        protected MessageProcessor(IMessageReceiver receiver, ITextSerializer serializer)
        {
            this.receiver = receiver;
            this.serializer = serializer;
        }

        protected ITextSerializer Serializer { get { return this.serializer; } }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public virtual void Start()
        {
            ThrowIfDisposed();
            lock (this.lockObject)
            {
                if (!this.started)
                {
                    this.receiver.Start(this.OnMessageReceived);
                    this.started = true;
                }
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public virtual void Stop()
        {
            lock (this.lockObject)
            {
                if (this.started)
                {
                    this.receiver.Stop();
                    this.started = false;
                }
            }
        }

        /// <summary>
        /// Disposes the resources used by the processor.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Processes the message.
        /// </summary>
        /// <param name="traceIdentifier">The identifier that can be used to track the source message in the logs.</param>
        /// <param name="payload">The typed message payload.</param>
        /// <param name="messageId">The message id.</param>
        /// <param name="correlationId">The message correlation id.</param>
        /// <param name="messageInformation">The information of brokered message</param>
        protected abstract void ProcessMessage(string traceIdentifier, object payload, string messageId, string correlationId, BrokerdMessageInformation messageInformation);

        /// <summary>
        /// Disposes the resources used by the processor.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Stop();
                    this.disposed = true;

                    using (this.receiver as IDisposable)
                    {
                        // Dispose receiver if it's disposable.
                    }
                }
            }
        }

        ~MessageProcessor()
        {
            Dispose(false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "By design.")]
        private MessageReleaseAction OnMessageReceived(BrokeredMessage message)
        {
            // NOTE: type information does not belong here. It's a responsibility 
            // of the serializer to be self-contained and put any information it 
            // might need for rehydration.

            object payload;
            using (var stream = message.GetBody<Stream>())
            using (var reader = new StreamReader(stream))
            {
                try
                {
                    payload = this.serializer.Deserialize(reader);
                }
                catch (SerializationException e)
                {
                    return MessageReleaseAction.DeadLetterMessage(e.Message, e.ToString());
                }
            }

            // TODO: have a better trace correlation mechanism (that is used in both the sender and receiver).
            string traceIdentifier = BuildTraceIdentifier(message);
            BrokerdMessageInformation messageInformation = GetMessageInformation(message);

            try
            {
                ProcessMessage(traceIdentifier, payload, message.MessageId, message.CorrelationId, messageInformation);
            }
            catch (Exception e)
            {
                return HandleProcessingException(message, traceIdentifier, e);
            }

            return CompleteMessage(message, traceIdentifier);
        }

        private MessageReleaseAction CompleteMessage(BrokeredMessage message, string traceIdentifier)
        {
            // Trace.WriteLine("The message" + traceIdentifier + " has been processed and will be completed.");
            return MessageReleaseAction.CompleteMessage;
        }

        private MessageReleaseAction HandleProcessingException(BrokeredMessage message, string traceIdentifier, Exception e)
        {
            if (message.DeliveryCount > MaxProcessingRetries)
            {
                Trace.TraceError("An error occurred while processing the message" + traceIdentifier + " and will be dead-lettered:\r\n{0}", e);
                return MessageReleaseAction.DeadLetterMessage(e.Message, e.ToString());
            }
            else
            {
                Trace.TraceWarning("An error occurred while processing the message" + traceIdentifier + " and will be abandoned:\r\n{0}", e);
                return MessageReleaseAction.AbandonMessage;
            }
        }

        // TODO: remove once we have a better trace correlation mechanism (that is used in both the sender and receiver).
        private static string BuildTraceIdentifier(BrokeredMessage message)
        {
            try
            {
                var messageId = message.MessageId;
                return string.Format(CultureInfo.InvariantCulture, " (MessageId: {0})", messageId);
            }
            catch (ObjectDisposedException)
            {
                // if there is any kind of exception trying to build a trace identifier, ignore, as it is not important.
            }

            return string.Empty;
        }

        private BrokerdMessageInformation GetMessageInformation(BrokeredMessage message)
        {
            return new BrokerdMessageInformation()
            {
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                SessionId = message.SessionId,
                DeliveryCount = message.DeliveryCount,
                EnqueuedTime = message.EnqueuedTimeUtc
            };
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException("MessageProcessor");
        }
    }
}
