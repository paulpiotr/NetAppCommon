using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace NetAppCommon.Logging
{
    #region public class SoapClientMessageInspector : IClientMessageInspector
    /// <summary>
    /// Inspektor wiadomości klienta SOAP
    /// Soap Client Message Inspector
    /// </summary>
    public class SoapClientMessageInspector : IClientMessageInspector
    {
        #region private readonly log4net.ILog log4net
        /// <summary>
        /// 
        /// </summary>
        private readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        # region public ILogger<SoapClientMessageInspector> Logger { get; }
        /// <summary>
        /// Interfejs ILogger typu SoapClientMessageInspector
        /// ILogger interface of type SoapClientMessageInspector
        /// </summary>
        public ILogger<SoapClientMessageInspector> Logger { get; }
        #endregion

        #region public SoapClientMessageInspector()
        /// <summary>
        /// Konstruktor bez parametrów
        /// Constructor with no parameters
        /// </summary>
        public SoapClientMessageInspector()
        {
            //Logger = (ILogger<SoapClientMessageInspector>)log4net.Logger;
        }
        #endregion

        #region public SoapClientMessageInspector(ILogger<SoapClientMessageInspector> logger)
        /// <summary>
        /// Konstruktor z parametrem ILogger
        /// Constructor with the ILogger parameter
        /// </summary>
        /// <param name="logger">
        /// Interfejs ILogger typu SoapClientMessageInspector
        /// ILogger interface of type SoapClientMessageInspector
        /// </param>
        public SoapClientMessageInspector(ILogger<SoapClientMessageInspector> logger)
        {
            Logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }
        #endregion

        #region public void AfterReceiveReply(ref Message reply, object correlationState)
        /// <summary>
        /// Po otrzymaniu odpowiedzi
        /// After Receive Reply
        /// </summary>
        /// <param name="reply">
        /// Referencja do wiadomości odpowiedzi jako Message
        /// A reference to the reply message as Message
        /// </param>
        /// <param name="correlationState">
        /// Stan korelacji jako obiekt
        /// Correlation state as an object
        /// </param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            try
            {
                log4net.Debug("AfterReceiveReply(ref Message reply, object correlationState)");
                using (MessageBuffer buffer = reply.CreateBufferedCopy(int.MaxValue))
                {
                    /// Override payload
                    XmlDocument xmlDocument = GetDocument(buffer.CreateMessage());
                    using (var stringWriter = new StringWriter())
                    {
                        using (var xmlWriter = XmlWriter.Create(stringWriter))
                        {
                            xmlDocument.WriteTo(xmlWriter);
                            xmlWriter.Flush();
                            var pattern = @"http\:\/\/ICASA-GROUP\.WebServices";
                            var replacement = @"http://ICASA.WebServices";
                            var stringReplace = Regex.Replace(stringWriter.GetStringBuilder().ToString(), pattern, replacement);
                            using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(stringReplace)))
                            {
                                xmlDocument.Load(memoryStream);
                            }
                        }
                    }
                    log4net.Debug(xmlDocument.OuterXml);
                    /// Oryginal
                    /// Logger.LogTrace(xmlDocument.OuterXml);
                    /// Oryginal
                    /// reply = buffer.CreateMessage();
                    var envelopeReader = XmlReader.Create(new StringReader(xmlDocument.OuterXml));
                    // Create the message using the reader
                    var replacedMessage = Message.CreateMessage(envelopeReader, int.MaxValue, reply.Version);
                    reply = replacedMessage;
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
        }
        #endregion

        #region public object BeforeSendRequest(ref Message message, IClientChannel clientChannel)
        /// <summary>
        /// Przed wysłaniem
        /// </summary>
        /// <param name="message">
        /// Referencja do wiadomości w żądaniu
        /// Reference to the message in the request
        /// </param>
        /// <param name="clientChannel">
        /// Kanał wymiany
        /// Exchange channel
        /// </param>
        /// <returns>
        /// null
        /// mull
        /// </returns>
        public object BeforeSendRequest(ref Message message, IClientChannel clientChannel)
        {
            try
            {
                log4net.Debug("BeforeSendRequest(ref Message message, IClientChannel clientChannel)");
                using (MessageBuffer buffer = message.CreateBufferedCopy(int.MaxValue))
                {
                    XmlDocument xmlDocument = GetDocument(buffer.CreateMessage());
                    //Oryginalnie
                    //Logger.LogTrace(xmlDocument.OuterXml);
                    log4net.Debug(xmlDocument.OuterXml);
                    message = buffer.CreateMessage();
                    return null;
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region private XmlDocument GetDocument(Message message)
        /// <summary>
        /// Pobierz treść dokumentu jako obiekt XML
        /// Get the body of the xmlDocument as an XML object
        /// </summary>
        /// <param name="message">
        /// Referencja do wiadomości w żądaniu
        /// Reference to the message in the request
        /// </param>
        /// <returns>
        /// Treść dokumentu jako obiekt XML
        /// The body of the xmlDocument as an XML object
        /// </returns>
        private XmlDocument GetDocument(Message message)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                using (var memoryStream = new MemoryStream())
                {
                    // write message to memory stream
                    var writer = XmlWriter.Create(memoryStream);
                    message.WriteMessage(writer);
                    writer.Flush();
                    memoryStream.Position = 0;
                    // load memory stream into a xmlDocument
                    xmlDocument.Load(memoryStream);
                }
                return xmlDocument;
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion
    }
    #endregion
}
