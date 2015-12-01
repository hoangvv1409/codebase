using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace GossipSharp
{
    public static class GossipMessageFactory
    {
        private static readonly Func<int, byte[], GossipMessage> _default = BuildMessageFactory();
        private static Func<int, byte[], GossipMessage> _createMessage = _default;
        public static Func<int, byte[], GossipMessage> CreateMessage
        {
            get { return _createMessage; }
            set { _createMessage = value ?? _default; }
        }

        public static Func<int, byte[], GossipMessage> BuildMessageFactory()
        {
            var dict = new Dictionary<int, Func<int, byte[], GossipMessage>>();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                                          .Where(a => a != Assembly.GetExecutingAssembly())
                                          .SelectMany(a => a.GetTypes())
                                          .Where(t => t.IsSubclassOf(typeof(GossipMessage))))
            {
                var constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null) continue;

                var msg = (GossipMessage)constructor.Invoke(new object[0]);

                var messageType = Expression.Parameter(typeof(int), "messageType");
                var data = Expression.Parameter(typeof(byte[]), "data");
                var result = Expression.Parameter(typeof(GossipMessage), "result");

                var expressions = new List<Expression>();

                var methodInfo = typeof(GossipMessage).GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
                methodInfo = methodInfo.MakeGenericMethod(type);
                expressions.Add(Expression.Assign(result, Expression.Call(methodInfo, data)));

                var block = Expression.Block(new[] { result }, expressions);
                dict[msg.MessageType] = Expression.Lambda<Func<int, byte[], GossipMessage>>(block, messageType, data).Compile();
            }

            return (msgType, data) =>
                       {
                           Func<int, byte[], GossipMessage> func;
                           if (dict.TryGetValue(msgType, out func))
                               return func(msgType, data);
                           return new RawGossipMessage(msgType, data);
                       };
        }
    }
}
