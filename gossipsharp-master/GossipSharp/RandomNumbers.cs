using System;
using System.Threading;

namespace GossipSharp
{
    public static class RandomNumbers
    {
        private static int _seed = new Random().Next();
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _seed)));

        public static ulong Simple()
        {
            var rnd1 = (uint)_random.Value.Next();
            var rnd2 = (uint)_random.Value.Next();
            return (ulong)rnd1 << 32 | rnd2;
        }

        public static ulong Complex(int maxIterations)
        {
            if (maxIterations < 1)
                throw new ArgumentOutOfRangeException("maxIterations", "Must be greater than 0");

            var random = _random.Value;
            uint rnd1 = 0;
            for (int i = 0; i < random.Next(maxIterations) + 1; i++)
                rnd1 ^= (uint)random.Next();
            uint rnd2 = 0;
            for (int i = 0; i < random.Next(maxIterations) + 1; i++)
                rnd2 ^= (uint)random.Next();
            return (ulong)rnd1 << 32 | rnd2;
        }
    }
}
