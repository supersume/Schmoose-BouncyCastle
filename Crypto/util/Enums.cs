using System;
using System.Text;

#if NETCF_1_0 || NETCF_2_0 || SILVERLIGHT
using System.Collections;
using System.Reflection;
#endif

using Org.BouncyCastle.Utilities.Date;

namespace Org.BouncyCastle.Utilities
{
    internal static class Enums
    {
        internal static Enum GetEnumValue(System.Type enumType, string s)
        {
#if !NETFX_CORE
			if (!enumType.IsEnum)
				throw new ArgumentException("Not an enumeration type", "enumType");
#endif
            // We only want to parse single named constants
            if (s.Length > 0 && char.IsLetter(s[0]) && s.IndexOf(',') < 0)
            {
                s = s.Replace('-', '_');

#if NETCF_1_0
				FieldInfo field = enumType.GetField(s, BindingFlags.Static | BindingFlags.Public);
				if (field != null)
				{
					return (Enum)field.GetValue(null);
				}
#else
                return (Enum)Enum.Parse(enumType, s, false);
#endif
            }

            throw new ArgumentException();
        }

        internal static Array GetEnumValues(System.Type enumType)
        {
#if !NETFX_CORE
            if (!enumType.IsEnum)
                throw new ArgumentException("Not an enumeration type", "enumType");
#endif
#if NETCF_1_0 || NETCF_2_0 || SILVERLIGHT
            IList result = Platform.CreateArrayList();
			FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo field in fields)
			{
				result.Add(field.GetValue(null));
			}
            object[] arr = new object[result.Count];
            result.CopyTo(arr, 0);
            return arr;
#else
            return Enum.GetValues(enumType);
#endif
        }

        internal static Enum GetArbitraryValue(System.Type enumType)
        {
            Array values = GetEnumValues(enumType);
            int pos = (int)(DateTimeUtilities.CurrentUnixMs() & int.MaxValue) % values.Length;
            return (Enum)values.GetValue(pos);
        }
    }
}
