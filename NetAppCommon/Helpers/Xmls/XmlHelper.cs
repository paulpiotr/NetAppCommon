#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
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
        private static readonly ILog Log4Net = Logging.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

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
        ///     Ogólna metoda deserializacja ciągu xml do typu T
        ///     General method to deserialize the xml string to type T
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
        ///     Objekt typu danych T
        ///     Object of the data type T
        /// </returns>
        public static T DeserializeXmlFromString<T>(string xml, string tagName = null)
        {
            try
            {

                //var xDocument = XDocument.Parse(xml);
                //XElement xElement = xDocument.Elements("root")
                //    .FirstOrDefault()
                //    ?.Element("dane");
                //Console.WriteLine(xElement?.ToString());

                if (null != tagName && !string.IsNullOrWhiteSpace(tagName))
                {
                    char[] delimiterChars = { '.' };
                    var listOfAttributes = new List<string>(tagName.Split(delimiterChars))
                        .Select(x => x.Trim()).ToList();
                    if (listOfAttributes.Count > 0)
                    {
                        var xDocument = XDocument.Parse(xml);
                        listOfAttributes.ForEach(name =>
                        {
                            XElement xElement = xDocument.Root?.Elements(name).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(xElement?.ToString()))
                            {
                                xml = xElement?.ToString();
                                xDocument = XDocument.Parse(xml);
                                //Console.WriteLine($"{name}");
                                //Console.WriteLine($"{xml}");
                            }
                        });
                    }
                }
                var xmlReader = XmlReader.Create(new StringReader(xml));
                var xmlSerializer = new XmlSerializer(typeof(T));
                var @object = (T)xmlSerializer.Deserialize(xmlReader);
                if (null != @object)
                {
                    return @object;
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
