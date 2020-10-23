using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace NetAppCommon.Logging
{
    #region public class SoapClientMessageInspector : IClientMessageInspector
    /// <summary>
    /// Inspektor wiadomości klienta SOAP
    /// Soap Client Message Inspector
    /// </summary>
    public class SoapClientMessageInspector : IClientMessageInspector
    {
        #region private static readonly log4net.ILog log4net
        /// <summary>
        /// 
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
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
                    XmlDocument document = GetDocument(buffer.CreateMessage());
                    log4net.Debug(document.OuterXml);
                    //Logger.LogTrace(document.OuterXml);
                    reply = buffer.CreateMessage();
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                throw e;
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
                    XmlDocument document = GetDocument(buffer.CreateMessage());
                    //Oryginalnie
                    //Logger.LogTrace(document.OuterXml);
                    log4net.Debug(document.OuterXml);
                    message = buffer.CreateMessage();
                    return null;
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                throw e;
            }
        }
        #endregion

        #region private XmlDocument GetDocument(Message message)
        /// <summary>
        /// Pobierz treść dokumentu jako obiekt XML
        /// Get the body of the document as an XML object
        /// </summary>
        /// <param name="message">
        /// Referencja do wiadomości w żądaniu
        /// Reference to the message in the request
        /// </param>
        /// <returns>
        /// Treść dokumentu jako obiekt XML
        /// The body of the document as an XML object
        /// </returns>
        private XmlDocument GetDocument(Message message)
        {
            try
            {
                XmlDocument document = new XmlDocument();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // write message to memory stream
                    XmlWriter writer = XmlWriter.Create(memoryStream);
                    message.WriteMessage(writer);
                    writer.Flush();
                    memoryStream.Position = 0;
                    // load memory stream into a document
                    document.Load(memoryStream);
                }
                return document;
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}", e.Message, e.StackTrace), e);
                throw e;
            }
        }
        #endregion
    }
    #endregion
}