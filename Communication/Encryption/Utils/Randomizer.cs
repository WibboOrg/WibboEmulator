using System;

namespace Buttefly.Utilities
{
    public class Randomizer
    {
        private static readonly Random _rand = new Random();

        public static Random GetRandom => _rand;

        public static int Next()
        {
            return _rand.Next();
        }

        public static int Next(int max)
        {
            return _rand.Next(max);
        }

        public static int Next(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public static double NextDouble()
        {
            return _rand.NextDouble();
        }

        public static byte NextByte()
        {
            return (byte)Next(0, 255);
        }

        public static byte NextByte(int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(0, max);
        }

        public static byte NextByte(int min, int max)
        {
            max = Math.Min(max, 255);
            return (byte)Next(Math.Min(min, max), max);
        }

        public static void NextBytes(byte[] toparse)
        {
            _rand.NextBytes(toparse);
        }
    }
}
