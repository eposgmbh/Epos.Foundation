using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Epos.Utilities
{
    public static class ValueConversionExtensions
    {
        public static bool TryConvert<TDestinationType>(this string value, Type destinationType, out TDestinationType convertedValue) {
            return TryConvert(value, destinationType, CultureInfo.CurrentCulture, out convertedValue);
        }

        public static bool TryConvert<TDestinationType>(this string value, Type destinationType, CultureInfo cultureInfo, out TDestinationType convertedValue) {
            try {
                bool isNullableType = destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>);
                Type theDestinationType = isNullableType ? destinationType.GetGenericArguments().Single() : destinationType;

                TypeConverter theConverter = TypeDescriptor.GetConverter(theDestinationType);
                if (!theConverter.CanConvertFrom(typeof(string))) {
                    convertedValue = default(TDestinationType);
                    return false;
                }

                object theValue = theConverter.ConvertFromString(null, cultureInfo, value);
                convertedValue = (TDestinationType) (isNullableType ? Activator.CreateInstance(destinationType, theValue) : theValue);

                return true;
            } catch (Exception) {
                convertedValue = default(TDestinationType);
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