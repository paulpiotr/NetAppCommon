using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace NetAppCommon.Helpers.Xmls
{
    #region public class XmlHelper
    /// <summary>
    /// Klasa pomocnika XML
    /// XML helper class
    /// </summary>
    public class XmlHelper
    {
        #region private readonly log4net.ILog log4net
        /// <summary>
        /// Log4net Logger
        /// Log4net Logger
        /// </summary>
        private static readonly log4net.ILog Log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static T DeserializeXmlFromFile<T>(string filePath)
        public static T DeserializeXmlFromFile<T>(string filePath)
        {
            try
            {
                if (null != filePath && !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                {
                    return DeserializeXmlFromString<T>(File.ReadAllText(filePath));
                }
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
        #endregion

        #region public static T DeserializeXml<T>(string xml)
        /// <summary>
        /// Ogólna metoda deserializacja ciągu xml do typu T
        /// General method to deserialize the xml string to type T
        /// </summary>
        /// <typeparam name="T">
        /// Typ danych T
        /// T data type
        /// </typeparam>
        /// <param name="xml">
        /// Ciąg xml jako string
        /// The xml string as a string
        /// </param>
        /// <returns>
        /// Objekt typu danych T
        /// Object of the data type T
        /// </returns>
        public static T DeserializeXmlFromString<T>(string xml)
        {
            try
            {
                var xmlReader = XmlReader.Create(new StringReader(xml));
                var xmlSerializer = new XmlSerializer(typeof(T));
                var o = (T)xmlSerializer.Deserialize(xmlReader);
                if (null != o)
                {
#if DEBUG
                    //log4net.Debug(string.Format($"T DeserializeXmlFromString<T>(string { xml }) {0} ok", o));
#endif
                    return o;
                }
#if DEBUG
                //log4net.Debug(string.Format($"T DeserializeXmlFromString<T>(string { xml }) {0} fail", o));
#endif
            }
            catch (Exception e)
            {
                Log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
        #endregion
    }
    #endregion
}
