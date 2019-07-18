using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities
{
    public static class EnumUtilities
    {
        public static bool IsDefined<TEnum>(object value) where TEnum : struct, IConvertible
        {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, IConvertible
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        public static IEnumerable<string> GetNames<TEnum>() where TEnum : struct, IConvertible
        {
            return Enum.GetNames(typeof(TEnum));
        }

        public static IEnumerable<KeyValuePair<TEnum, string>> GetKeyValuePairs<TEnum>() where TEnum : struct, IConvertible
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(v => new KeyValuePair<TEnum, string>(v, v.ToString()));
        }

        #region Parse
        public static TEnum Parse<TEnum>(string value) where TEnum : struct, IConvertible
        {
            TEnum result = (TEnum)Enum.Parse(typeof(TEnum), value);
            if(!EnumUtilities.IsDefined<TEnum>(value))
                throw new System.ArgumentOutOfRangeException();

            return result;
        }

        public static TEnum Parse<TEnum>(int value) where TEnum : struct, IConvertible
        {
            if(!EnumUtilities.IsDefined<TEnum>(value))
                throw new System.ArgumentOutOfRangeException();

            return (TEnum)((object)value);
        }
        #endregion

        #region TryParse
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
        {
            return Enum.TryParse(value, out result) ? !EnumUtilities.IsDefined<TEnum>(value) : false;
        }

        public static bool TryParse<TEnum>(int value, out TEnum result) where TEnum : struct, IConvertible
        {
            if(EnumUtilities.IsDefined<TEnum>(value))
            {
                result = (TEnum)((object)value);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
        #endregion
    }
}
