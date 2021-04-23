#region using

using System;
using System.IO;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Serialization;
using log4net;

#endregion

namespace NetAppCommon.Helpers.Xmls
{
    #region public class XmlHelper

    /// <summary>
    ///     Klasa pomocnika XML
    ///     XML helper class
    /// </summary>
    public class XmlHelper
    {
        #region private static readonly ILog Log4Net

        /// <summary>
        ///     private static readonly ILog Log4Net
        ///     private static readonly ILog Log4Net
        /// </summary>
        private static readonly ILog Log4Net =
            Logging.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

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
                Log4Net.Error(e);
                if (null != e.InnerException)
                {
                    Log4Net.Error(e.InnerException);
                }
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        #endregion

        #region public static T DeserializeXml<T>(string xml)

        /// <summary>
        ///     Deserializacja ciągu xml do typu T
        ///     Deserialization of the xml string to type T
        /// </summary>
        /// <typeparam name="T">
        ///     Typ danych T
        ///     T data type
        /// </typeparam>
        /// <param name="xml">
        ///     Ciąg xml jako string
        ///     The xml string as a string
        /// </param>
        /// <returns>
        ///     Obiekt typu danych T
        ///     Object of the data type T
        /// </returns>
        public static T DeserializeXmlFromString<T>(string xml)
        {
            XmlReader xmlReader = null;
            try
            {
                xmlReader = XmlReader.Create(new StringReader(xml));

                try
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    var @object = (T)xmlSerializer.Deserialize(xmlReader);
                    if (null != @object)
                    {
                        return @object;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Log4Net.Warn(e);
#else
                    Log4Net.Warn(e);
#endif
                }

                while (xmlReader.Read())
                {
                    // First element is the root element
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        try
                        {
                            var xmlSerializer = new XmlSerializer(typeof(T));
                            var @object = (T)xmlSerializer.Deserialize(xmlReader);
                            if (null != @object)
                            {
                                return @object;
                            }
                        }
                        catch (Exception e)
                        {
#if DEBUG
                            Log4Net.Warn(e);
#else
                            Log4Net.Warn(e);
#endif
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log4Net.Error(e);
                if (null != e.InnerException)
                {
                    Log4Net.Error(e.InnerException);
                }
            }
            finally
            {
                xmlReader?.Close();
            }

            return (T)Convert.ChangeType(null, typeof(T));
        }

        #endregion

        #region public static XmlDocument GetDocument(Message message)

        /// <summary>
        ///     Pobierz treść dokumentu jako obiekt XML
        ///     Get the body of the xmlDocument as an XML object
        /// </summary>
        /// <param name="message">
        ///     Referencja do wiadomości w żądaniu
        ///     Reference to the message in the request
        /// </param>
        /// <returns>
        ///     Treść dokumentu jako obiekt XML
        ///     The body of the xmlDocument as an XML object
        /// </returns>
        public static XmlDocument GetDocument(Message message)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                using var memoryStream = new MemoryStream();
                // write message to memory stream
                var xmlWriter = XmlWriter.Create(memoryStream);
                message.WriteMessage(xmlWriter);
                xmlWriter.Flush();
                memoryStream.Position = 0;
                // load memory stream into a xmlDocument
                xmlDocument.Load(memoryStream);

                return xmlDocument;
            }
            catch (Exception e)
            {
                Log4Net.Error(e);
                if (null != e.InnerException)
                {
                    Log4Net.Error(e.InnerException);
                }
            }

            return null;
        }

        #endregion
    }

    #endregion
}
