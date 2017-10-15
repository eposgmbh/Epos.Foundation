using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Epos.Utilities
{
    public static class ValueConversionExtensions
    {
        public static bool TryConvert(this string value, Type destinationType, out object convertedValue) {
            return TryConvert(value, destinationType, CultureInfo.CurrentCulture, out convertedValue);
        }

        public static bool TryConvert(this string value, Type destinationType, CultureInfo cultureInfo, out object convertedValue) {
            try {
                bool isNullableType = destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>);
                Type theDestinationType = isNullableType ? destinationType.GetGenericArguments().Single() : destinationType;

                TypeConverter theConverter = TypeDescriptor.GetConverter(theDestinationType);
                if (!theConverter.CanConvertFrom(typeof(string))) {
                    convertedValue = null;
                    return false;
                }

                object theValue = theConverter.ConvertFromString(null, cultureInfo, value);
                convertedValue = isNullableType ? Activator.CreateInstance(destinationType, theValue) : theValue;

                return true;
            } catch (Exception) {
                convertedValue = null;
                return false;
            }
        }

        public static bool TryConvert<TDestinationType>(this string value, out TDestinationType convertedValue) {
            return TryConvert(value, CultureInfo.CurrentCulture, out convertedValue);
        }

        public static bool TryConvert<TDestinationType>(this string value, CultureInfo cultureInfo, out TDestinationType convertedValue) {
            bool theResult = TryConvert(value, typeof(TDestinationType), cultureInfo, out object theConvertedValue);

            if (theResult) {
                convertedValue = (TDestinationType) theConvertedValue;
            } else {
                convertedValue = default(TDestinationType);
            }

            return theResult;
        }
    }
}