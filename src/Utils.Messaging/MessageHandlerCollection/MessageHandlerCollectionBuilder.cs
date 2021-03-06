namespace RoRamu.Utils.Messaging
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A helper class for building new instances of <see cref="RoRamu.Utils.Messaging.IMessageHandlerCollection" />.
    /// </summary>
    public class MessageHandlerCollectionBuilder
    {
        /// <summary>
        /// The method signature for asynchronous message handler implementations.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        public delegate Task HandlerAsyncDelegate(Message message);

        /// <summary>
        /// The method signature for synchronous message handler implementations.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        public delegate void HandlerDelegate(Message message);

        private MessageHandlerCollection MessageHandlerCollection { get; } = new MessageHandlerCollection();

        private MessageHandlerCollectionBuilder() { }

        /// <summary>
        /// Creates a new <see cref="RoRamu.Utils.Messaging.MessageHandlerCollectionBuilder" />.
        /// </summary>
        public static MessageHandlerCollectionBuilder Create()
        {
            return new MessageHandlerCollectionBuilder();
        }

        /// <summary>
        /// Sets the default handler to use when a message has a type which has no registered handlers.
        /// </summary>
        /// <param name="messageHandler">The default handler implementation.</param>
        public MessageHandlerCollectionBuilder SetDefaultHandler(HandlerAsyncDelegate messageHandler)
        {
            this.MessageHandlerCollection.FallbackMessageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));

            return this;
        }

        /// <summary>
        /// Sets the default handler to use when a message has a type which has no registered handlers.
        /// </summary>
        /// <param name="messageHandler">The default handler implementation.</param>
        public MessageHandlerCollectionBuilder SetDefaultHandler(HandlerDelegate messageHandler)
        {
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.MessageHandlerCollection.FallbackMessageHandler = (m) =>
            {
                messageHandler(m);
                return Task.CompletedTask;
            };

            return this;
        }

        /// <summary>
        /// Resets the default message handler to the default implementation (i.e. to throw a
        /// <see cref="RoRamu.Utils.Messaging.UnknownMessageTypeException" />).
        /// </summary>
        public MessageHandlerCollectionBuilder RemoveDefaultHandler()
        {
            this.MessageHandlerCollection.FallbackMessageHandler = null;

            return this;
        }

        /// <summary>
        /// Registers a message handler if one doesn't exist for the given message type, otherwise
        /// overwrites the previously registered handler for that message type.
        /// </summary>
        /// <param name="messageType">The message type to set the handler for.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns></returns>
        public MessageHandlerCollectionBuilder SetHandler(string messageType, HandlerAsyncDelegate messageHandler)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }

            this.MessageHandlerCollection[messageType] = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));

            return this;
        }

        /// <summary>
        /// Registers a message handler if one doesn't exist for the given message type, otherwise
        /// overwrites the previously registered handler for that message type.
        /// </summary>
        /// <param name="messageType">The message type to set the handler for.</param>
        /// <param name="messageHandler">The message handler.</param>
        /// <returns></returns>
        public MessageHandlerCollectionBuilder SetHandler(string messageType, HandlerDelegate messageHandler)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }
            if (messageHandler == null)
            {
                throw new ArgumentNullException(nameof(messageHandler));
            }

            this.MessageHandlerCollection[messageType] = (m) =>
            {
                messageHandler(m);
                return Task.CompletedTask;
            };

            return this;
        }

        /// <summary>
        /// Unregisters a message handler for the given message type.
        /// </summary>
        /// <param name="messageType">The message type for which to remove the handler.</param>
        public MessageHandlerCollectionBuilder RemoveHandler(string messageType)
        {
            if (messageType == null)
            {
                throw new ArgumentNullException(nameof(messageType));
            }

            this.MessageHandlerCollection.Remove(messageType);

            return this;
        }

        /// <summary>
        /// Builds a new instance of an <see cref="RoRamu.Utils.Messaging.IMessageHandlerCollection" />
        /// using the current state of this
        /// <see cref="RoRamu.Utils.Messaging.MessageHandlerCollectionBuilder" />.
        /// </summary>
        /// <returns>An <see cref="RoRamu.Utils.Messaging.IMessageHandlerCollection" />.</returns>
        public IMessageHandlerCollection Build()
        {
            return this.MessageHandlerCollection;
        }
    }
}