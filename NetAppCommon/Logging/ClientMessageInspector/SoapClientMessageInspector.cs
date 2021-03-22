#region using

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Xml;
using log4net;
using Microsoft.Extensions.Logging;
using NetAppCommon.Helpers.Xmls;

#endregion

namespace NetAppCommon.Logging.ClientMessageInspector
{
    #region public class SoapClientMessageInspector : IClientMessageInspector

    /// <summary>
    ///     Inspektor wiadomości klienta SOAP
    ///     Soap Client Message Inspector
    /// </summary>
    public class SoapClientMessageInspector : IClientMessageInspector
    {
        #region private readonly ILog _log4Net

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private readonly ILog _log4Net = Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public SoapClientMessageInspector()

        /// <summary>
        ///     Konstruktor bez parametrów
        ///     Constructor with no parameters
        /// </summary>
        public SoapClientMessageInspector()
        {
        }

        #endregion

        #region public SoapClientMessageInspector(ILogger<SoapClientMessageInspector> logger)

        /// <summary>
        ///     Konstruktor z parametrem ILogger
        ///     Constructor with the ILogger parameter
        /// </summary>
        /// <param name="logger">
        ///     Interfejs ILogger typu SoapClientMessageInspector
        ///     ILogger interface of type SoapClientMessageInspector
        /// </param>
        public SoapClientMessageInspector(ILogger<SoapClientMessageInspector> logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        # region public ILogger<SoapClientMessageInspector> Logger { get; }

        /// <summary>
        ///     Interfejs ILogger typu SoapClientMessageInspector
        ///     ILogger interface of type SoapClientMessageInspector
        /// </summary>
        public ILogger<SoapClientMessageInspector> Logger { get; }

        #endregion

        #region public void AfterReceiveReply(ref Message reply, object correlationState)

        /// <summary>
        ///     Po otrzymaniu odpowiedzi
        ///     After Receive Reply
        /// </summary>
        /// <param name="reply">
        ///     Referencja do wiadomości odpowiedzi jako Message
        ///     A reference to the reply message as Message
        /// </param>
        /// <param name="correlationState">
        ///     Stan korelacji jako obiekt
        ///     Correlation state as an object
        /// </param>
        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
            try
            {
                using MessageBuffer messageBuffer = reply.CreateBufferedCopy(int.MaxValue);
#if DEBUG
                _log4Net.Debug("virtual AfterReceiveReply");
                XmlDocument xmlDocument = XmlHelper.GetDocument(messageBuffer.CreateMessage());
                xmlDocument.Normalize();
                if (null != Logger)
                {
                    Logger?.LogTrace(HttpUtility.HtmlDecode(xmlDocument?.OuterXml));
                }
                else
                {
                    _log4Net.Debug(HttpUtility.HtmlDecode(xmlDocument?.OuterXml));
                }
#endif
                reply = messageBuffer.CreateMessage();
            }
            catch (Exception e)
            {
                _log4Net.Error(e);
                if (null != e.InnerException)
                {
                    _log4Net.Error(e.InnerException);
                }
            }
        }

        #endregion

        #region public object BeforeSendRequest(ref Message message, IClientChannel clientChannel)

        /// <summary>
        ///     Przed wysłaniem żądania
        ///     Before sending the request
        /// </summary>
        /// <param name="message">
        ///     Referencja do wiadomości w żądaniu
        ///     Reference to the message in the request
        /// </param>
        /// <param name="clientChannel">
        ///     Kanał wymiany
        ///     Exchange channel
        /// </param>
        /// <returns>
        ///     null
        ///     null
        /// </returns>
        public virtual object BeforeSendRequest(ref Message message, IClientChannel clientChannel)
        {
            try
            {
                using MessageBuffer messageBuffer = message.CreateBufferedCopy(int.MaxValue);
#if DEBUG
                _log4Net.Debug("virtual BeforeSendRequest");
                // Override payload
                XmlDocument xmlDocument = XmlHelper.GetDocument(messageBuffer.CreateMessage());
                xmlDocument.Normalize();
                if (null != Logger)
                {
                    Logger?.LogTrace(HttpUtility.HtmlDecode(xmlDocument?.OuterXml));
                }
                else
                {
                    _log4Net.Debug(HttpUtility.HtmlDecode(xmlDocument?.OuterXml));
                }
#endif
                message = messageBuffer.CreateMessage();
                return null;
            }
            catch (Exception e)
            {
                _log4Net.Error(e);
                if (null != e.InnerException)
                {
                    _log4Net.Error(e.InnerException);
                }
            }

            return null;
        }

        #endregion
    }

    #endregion
}
