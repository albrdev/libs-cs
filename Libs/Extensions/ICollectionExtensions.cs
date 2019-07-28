using System;
using System.Collections.Generic;
using System.Linq;

namespace Libs.Extensions
{
    public static class ICollectionExtensions
    {
        public static T RandomElement<T>(this ICollection<T> self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            System.Random random = new System.Random();
            return self.ElementAt(random.Next(self.Count));
        }

        public static T RandomElement<T>(this ICollection<T> self, System.Random random)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.ElementAt(random.Next(self.Count));
        }
    }
}
