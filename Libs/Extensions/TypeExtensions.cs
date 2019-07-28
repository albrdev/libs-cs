using System;

namespace Libs.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNullable(this System.Type self)
        {
            if(self == null)
                throw new System.ArgumentNullException($@"self");

            return self.IsClass || (self.IsGenericType ? self.GetGenericTypeDefinition() == typeof(Nullable<>) : Nullable.GetUnderlyingType(self) != null);
        }
    }
}
