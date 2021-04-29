#region using

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Object
{
    #region public class ObjectHelper

    /// <summary>
    ///     Klasa pomocnika obiektów
    ///     Object helper class
    /// </summary>
    public class ObjectHelper
    {
        #region private readonly log4net.ILog log4net

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private static readonly ILog _log4Net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public static string GetValuesToString(object o, string separator = null)

        /// <summary>
        ///     Pobierz wartości właściwości obiektu i zbuduj ciąg tekstowy rozdzielonych wartości właściwości separatorem
        ///     Get the object property values and build a text string separated by a property value with a separator
        /// </summary>
        /// <param name="o">
        ///     Obiekt o jako object
        ///     Object o as object
        /// </param>
        /// <param name="separator">
        ///     Separator rozdzielający wartości właściwości obiektu jako string
        ///     Separator separating object property values as a string
        /// </param>
        /// <returns>
        ///     Wartości właściwości obiektu rozdzielone separatorem jako string
        ///     Object property values separated by a separator as a string
        /// </returns>
        public static string GetValuesToString(object o, string separator = null)
        {
            try
            {
                if (null != o)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (PropertyInfo propertyInfo in o.GetType().GetProperties().OrderBy(x => x.Name))
                    {
                        switch (propertyInfo.PropertyType.ToString())
                        {
                            default:
                                stringBuilder.Append(propertyInfo.Name).Append(separator)
                                    .Append(propertyInfo.GetValue(o, null) ?? string.Empty).Append(separator);
                                break;
                            case "Decimal":
                            case "System.Decimal":
                                var decimalValue = (decimal)(propertyInfo.GetValue(o, null) ?? 0);
                                stringBuilder.Append(propertyInfo.Name).Append(separator)
                                    .Append(decimalValue.ToString("N", CultureInfo.InvariantCulture)).Append(separator);
                                break;
                            case "Double":
                            case "System.Double":
                                var doubleValue = (double)(propertyInfo.GetValue(o, null) ?? 0);
                                stringBuilder.Append(propertyInfo.Name).Append(separator)
                                    .Append(doubleValue.ToString("N", CultureInfo.InvariantCulture)).Append(separator);
                                break;
                            case "System.DateTime":
                            case "System.Nullable`1[System.DateTime]":
                                if (DateTime.TryParse((propertyInfo.GetValue(o, null) ?? string.Empty).ToString(),
                                    out DateTime dateTime))
                                {
                                    stringBuilder.Append(propertyInfo.Name).Append(separator)
                                        .Append(dateTime.ToString(CultureInfo.InvariantCulture)).Append(separator);
                                }

                                break;
                        }
                    }

                    return stringBuilder.ToString();
                }
            }
            catch (Exception e)
            {
#if DEBUG
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                Console.WriteLine(e);
#endif
            }

            return null;
        }

        #endregion

        #region public async static Task<string> GetValuesToStringAsync(object o, string separator = null)

        /// <summary>
        ///     Pobierz wartości właściwości obiektu i zbuduj ciąg tekstowy rozdzielonych wartości właściwości separatorem
        ///     Get the object property values and build a text string separated by a property value with a separator
        /// </summary>
        /// <param name="o">
        ///     Obiekt o jako object
        ///     Object o as object
        /// </param>
        /// <param name="separator">
        ///     Separator rozdzielający wartości właściwości obiektu jako string
        ///     Separator separating object property values as a string
        /// </param>
        /// <returns>
        ///     Wartości właściwości obiektu rozdzielone separatorem jako string
        ///     Object property values separated by a separator as a string
        /// </returns>
        public static async Task<string> GetValuesToStringAsync(object o, string separator = null) =>
            await Task.Run(() => GetValuesToString(o, separator));

        #endregion

        #region public string ConvertObjectValuesToMD5Hash(object o, string separator = null)

        /// <param name="o">
        ///     Obiekt o jako object
        ///     Object o as object
        /// </param>
        /// <param name="separator">
        ///     Separator rozdzielający wartości właściwości obiektu jako string
        ///     Separator separating object property values as a string
        /// </param>
        /// <returns>
        ///     Skrót MD5 wartości właściwości obiektu jako string
        ///     MD5 hash of the object property value as a string
        /// </returns>
        public static string ConvertObjectValuesToMD5Hash(object o, string separator = null)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create()
                    .ComputeHash(Encoding.ASCII.GetBytes(GetValuesToString(o, separator))));
            }
            catch (Exception e)
            {
#if DEBUG
                _log4Net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
#else
                Console.WriteLine(e);
#endif
            }

            return null;
        }

        #endregion

        public static Guid ConvertObjectValuesToMD5HashGuid(object o, string separator = null) =>
            new(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(GetValuesToString(o, separator))));

        public static Guid GuidFromString(string s) => new(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s)));

        #region public async Task<string> ConvertObjectValuesToMD5HashAsync(object o, string separator = null)

        /// <param name="o">
        ///     Obiekt o jako object
        ///     Object o as object
        /// </param>
        /// <param name="separator">
        ///     Separator rozdzielający wartości właściwości obiektu jako string
        ///     Separator separating object property values as a string
        /// </param>
        /// <returns>
        ///     Skrót MD5 wartości właściwości obiektu jako string
        ///     MD5 hash of the object property value as a string
        /// </returns>
        public static async Task<string> ConvertObjectValuesToMD5HashAsync(object o, string separator = null) =>
            await Task.Run(() => ConvertObjectValuesToMD5Hash(o, separator));

        #endregion

        #region public static TValue GetDefaultValue<TValue>()

        /// <summary>
        ///     Pobierz domyślną wartość określonego typu określonego w parametrze TValue
        ///     Get the default value of the specified type specified in the TValue parameter
        /// </summary>
        /// <typeparam name="TValue">
        ///     Typ zwracanej wartości
        ///     The type of the return value
        /// </typeparam>
        /// <returns>
        ///     Domyślna wartość określonego typu określonego w parametrze TValue
        ///     The default value of the specified type specified in the TValue parameter
        /// </returns>
        public static TValue GetDefaultValue<TValue>() =>
            typeof(TValue).FullName switch
            {
                "System.Boolean" => (TValue)Convert.ChangeType(false, typeof(TValue)),
                "System.Int32" => (TValue)Convert.ChangeType(int.MinValue, typeof(TValue)),
                "System.DateTime" => (TValue)Convert.ChangeType(DateTime.MinValue, typeof(TValue)),
                _ => (TValue)Convert.ChangeType(null, typeof(TValue))
            };

        #endregion

        # region public static string ConvertObjectValuesToSHA512Hash...

        /// <summary>
        ///     Konwersja wartości właściwości obiektu do skrótu SHA512
        ///     Convert object property value to SHA512 hash
        /// </summary>
        /// <param name="@object">
        ///     object @object
        /// </param>
        /// <param name="separator">
        ///     Separator rozdzielający wartości właściwości obiektu jako string
        ///     Separator separating object property values as a string
        /// </param>
        /// <returns>
        ///     Skrót MD5 wartości właściwości obiektu jako string
        ///     MD5 hash of the object property value as a string
        /// </returns>
        public static string ConvertObjectValuesToSHA512Hash(object @object, string separator = null)
        {
            try
            {
                return Convert.ToBase64String(SHA512.Create()
                    .ComputeHash(Encoding.ASCII.GetBytes(GetValuesToString(@object, separator))));
            }
            catch (Exception e)
            {
#if DEBUG
                _log4Net.Error(e);
                if (null != e.InnerException)
                {
                    _log4Net.Error(e.InnerException);
                }
#else
                Console.WriteLine(e);
                if (null != e.InnerException)
                {
                    Console.WriteLine(e.InnerException);
                }
#endif
            }

            return null;
        }

        #endregion
    }

    #endregion
}
