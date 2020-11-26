using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetAppCommon.Helpers.Object
{
    #region public class ObjectHelper
    /// <summary>
    /// Klasa pomocnika obiektów
    /// Object helper class
    /// </summary>
    public class ObjectHelper
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// Log4net Logger
        /// Log4net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static string GetValuesToString(object o, string separator = null)
        /// <summary>
        /// Pobierz wartości właściwości obiektu i zbuduj ciąg tekstowy rozdzielonych wartości właściwości separatorem
        /// Get the object property values and build a text string separated by a property value with a separator
        /// </summary>
        /// <param name="o">
        /// Obiekt o jako object
        /// Object o as object
        /// </param>
        /// <param name="separator">
        /// Separator rozdzielający wartości właściwości obiektu jako string
        /// Separator separating object property values as a string
        /// </param>
        /// <returns>
        /// Wartości właściwości obiektu rozdzielone separatorem jako string
        /// Object property values separated by a separator as a string
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
                                stringBuilder.Append(propertyInfo.Name).Append(separator).Append(propertyInfo.GetValue(o, null) ?? string.Empty).Append(separator);
                                //log4net.Debug($"{ propertyInfo.PropertyType } { propertyInfo.GetValue(o, null) ?? string.Empty }");
                                break;
                            case "Decimal":
                            case "System.Decimal":
                                var decimalValue = (decimal)(propertyInfo.GetValue(o, null) ?? 0);
                                stringBuilder.Append(propertyInfo.Name).Append(separator).Append(decimalValue.ToString("N", CultureInfo.InvariantCulture)).Append(separator);
                                //log4net.Debug($"{ propertyInfo.PropertyType } { propertyInfo.GetValue(o, null) ?? string.Empty } { decimalValue } { decimalValue.ToString("N", CultureInfo.InvariantCulture) } ");
                                break;
                            case "Double":
                            case "System.Double":
                                var doubleValue = (double)(propertyInfo.GetValue(o, null) ?? 0);
                                stringBuilder.Append(propertyInfo.Name).Append(separator).Append(doubleValue.ToString("N", CultureInfo.InvariantCulture)).Append(separator);
                                //log4net.Debug($"{ propertyInfo.PropertyType } { propertyInfo.GetValue(o, null) ?? string.Empty } { doubleValue } { doubleValue.ToString("N", CultureInfo.InvariantCulture) }");
                                break;
                            case "System.DateTime":
                            case "System.Nullable`1[System.DateTime]":
                                DateTime dateTime;
                                if (DateTime.TryParse((propertyInfo.GetValue(o, null) ?? string.Empty).ToString(), out dateTime))
                                {
                                    stringBuilder.Append(propertyInfo.Name).Append(separator).Append(dateTime.ToString()).Append(separator);
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
                log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
#endif
            }
            return null;
        }
        #endregion

        #region public async static Task<string> GetValuesToStringAsync(object o, string separator = null)
        /// <summary>
        /// Pobierz wartości właściwości obiektu i zbuduj ciąg tekstowy rozdzielonych wartości właściwości separatorem
        /// Get the object property values and build a text string separated by a property value with a separator
        /// </summary>
        /// <param name="o">
        /// Obiekt o jako object
        /// Object o as object
        /// </param>
        /// <param name="separator">
        /// Separator rozdzielający wartości właściwości obiektu jako string
        /// Separator separating object property values as a string
        /// </param>
        /// <returns>
        /// Wartości właściwości obiektu rozdzielone separatorem jako string
        /// Object property values separated by a separator as a string
        /// </returns>
        public static async Task<string> GetValuesToStringAsync(object o, string separator = null)
        {
            return await Task.Run(() =>
            {
                return GetValuesToString(o, separator);
            });
        }
        #endregion

        #region public string ConvertObjectValuesToMD5Hash(object o, string separator = null)
        /// <param name="o">
        /// Obiekt o jako object
        /// Object o as object
        /// </param>
        /// <param name="separator">
        /// Separator rozdzielający wartości właściwości obiektu jako string
        /// Separator separating object property values as a string
        /// </param>
        /// <returns>
        /// Skrót MD5 wartości właściwości obiektu jako string
        /// MD5 hash of the object property value as a string
        /// </returns>
        public static string ConvertObjectValuesToMD5Hash(object o, string separator = null)
        {
            try
            {
                return Convert.ToBase64String(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(GetValuesToString(o, separator))));
            }
            catch (Exception e)
            {
#if DEBUG
                log4net.Error(string.Format("\n{0}\n{1}\n{2}\n{3}\n", e.GetType(), e.InnerException?.GetType(), e.Message, e.StackTrace), e);
#endif
            }
            return null;
        }
        #endregion

        public static Guid ConvertObjectValuesToMD5HashGuid(object o, string separator = null)
        {
            return new Guid(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(GetValuesToString(o, separator))));
        }

        public static Guid GuidFromString(string s)
        {
            return new Guid(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(s)));
        }

        #region public async Task<string> ConvertObjectValuesToMD5HashAsync(object o, string separator = null)
        /// <param name="o">
        /// Obiekt o jako object
        /// Object o as object
        /// </param>
        /// <param name="separator">
        /// Separator rozdzielający wartości właściwości obiektu jako string
        /// Separator separating object property values as a string
        /// </param>
        /// <returns>
        /// Skrót MD5 wartości właściwości obiektu jako string
        /// MD5 hash of the object property value as a string
        /// </returns>
        public static async Task<string> ConvertObjectValuesToMD5HashAsync(object o, string separator = null)
        {
            return await Task.Run(() =>
            {
                return ConvertObjectValuesToMD5Hash(o, separator);
            });
        }
        #endregion
    }
    #endregion
}
