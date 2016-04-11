using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Serialization;

namespace ADR.Mobile.Infrastructure.Messaging.Sql.Handling
{
    /// <summary>
    /// Provides basic common processing code for components that handle 
    /// incoming messages from a receiver.
    /// </summary>
    public abstract class MessageProcessor : IProcessor, IDisposable
    {
        private readonly IMessageReceiver receiver;
        private readonly ITextSerializer serializer;
        private readonly object lockObject = new object();
        private bool disposed;
        private bool started = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageProcessor"/> class.
        /// </summary>
        protected MessageProcessor(IMessageReceiver receiver, ITextSerializer serializer)
        {
            this.receiver = receiver;
            this.serializer = serializer;
        }

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
                    this.receiver.MessageReceived += OnMessageReceived;
                    this.receiver.Start();
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
                    this.receiver.MessageReceived -= OnMessageReceived;
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

        protected abstract void ProcessMessage(object payload, string correlationId);

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

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Trace.WriteLine(new string('-', 100));

            try
            {
                var body = Deserialize(args.Message.Body);

                TracePayload(body);
                Trace.WriteLine("");

                ProcessMessage(body, args.Message.CorrelationId);

                Trace.WriteLine(new string('-', 100));
            }
            catch (Exception e)
            {
                // NOTE: we catch ANY exceptions as this is for local 
                // development/debugging. The Windows Azure implementation 
                // supports retries and dead-lettering, which would 
                // be totally overkill for this alternative debug-only implementation.
                Trace.TraceError("An exception happened while processing message through handler/s:\r\n{0}", e);
                Trace.TraceWarning("Error will be ignored and message receiving will continue.");
            }
        }

        protected object Deserialize(string serializedPayload)
        {
            using (var reader = new StringReader(serializedPayload))
            {
                return this.serializer.Deserialize(reader);
            }
        }

        protected string Serialize(object payload)
        {
            using (var writer = new StringWriter())
            {
                this.serializer.Serialize(writer, payload);
                return writer.ToString();
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
                throw new ObjectDisposedException("MessageProcessor");
        }


        [Conditional("TRACE")]
        private void TracePayload(object payload)
        {
            Trace.WriteLine(this.Serialize(payload));
        }
    }
}
