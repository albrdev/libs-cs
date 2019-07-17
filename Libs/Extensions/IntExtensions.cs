using System;

namespace Libs.Extensions
{
    public static class IntExtensions
    {
        public static int Length(this int self, int radix = 10)
        {
            if(radix < 2)
                throw new System.ArgumentException();

            if(self == 0)
                return 1;

            int count = 0;
            while(self != 0)
            {
                self /= radix;
                count++;
            }

            return count;
        }

        public static bool IsPowerOfTwo(this int self)
        {
            self = Math.Abs(self);
            return self != 0 && (self & (self - 1)) == 0;
        }
    }
}
