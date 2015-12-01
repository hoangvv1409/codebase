using System;

namespace GossipSharp
{
    public static class GossipTimestampProvider
    {
        private static Func<DateTime> _getCurrentTimestamp;
        public static Func<DateTime> GetCurrentTimestamp
        {
            get { return _getCurrentTimestamp; }
            set
            {
                if (value == null) value = () => DateTime.UtcNow;
                _getCurrentTimestamp = value;
            }
        }

        public static DateTime CurrentTimestamp
        {
            get { return _getCurrentTimestamp(); }
        }

        static GossipTimestampProvider()
        {
            _getCurrentTimestamp = () => DateTime.UtcNow;
        }
    }
}
