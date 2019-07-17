using System;
using System.Collections.Generic;

namespace Libs.Extensions
{
    public static class RandomExtensions
    {
        public static byte[] GetBits(this System.Random self, int count)
        {
            if(count < 0)
                throw new System.ArgumentOutOfRangeException();
            else if(count == 0)
                return new byte[] { 0 };

            int mod = count % 8;
            count = (int)Math.Ceiling(count / 8D);
            byte[] buffer = new byte[count];
            self.NextBytes(buffer);

            if(mod != 0)
            {
                buffer[0] = (byte)(buffer[0] & ~(0xFF << mod));
            }

            return buffer;
        }

        public static byte[] GetBytes(this System.Random self, int count)
        {
            byte[] buffer = new byte[count];
            self.NextBytes(buffer);
            return buffer;
        }

        public static IEnumerable<byte> GetByteSequence(this System.Random self)
        {
            byte[] buffer = new byte[sizeof(byte)];
            while(true)
            {
                self.NextBytes(buffer);
                yield return buffer[0];
            }
        }

        public static bool NextBool(this System.Random self)
        {
            byte[] buffer = new byte[sizeof(bool)];
            self.NextBytes(buffer);

            return (buffer[0] & 0x1) != 0;
        }

        public static double NextDoubleInclusive(this System.Random self)
        {
            byte[] buffer = new byte[sizeof(uint)];
            self.NextBytes(buffer);

            return BitConverter.ToUInt32(buffer, 0) * (1D / uint.MaxValue);
        }

        public static double NextGaussian(this System.Random self, double mu = 0D, double sigma = 1D)
        {
            var u1 = 1D - self.NextDouble();
            var u2 = self.NextDouble();

            var standardNormal = Math.Sqrt(-2D * Math.Log(u1)) * Math.Sin(2D * Math.PI * u2);
            var normal = mu + sigma * standardNormal;

            return normal;
        }
    }
}
