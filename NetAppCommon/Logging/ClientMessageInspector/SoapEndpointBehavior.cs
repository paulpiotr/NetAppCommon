#region using

using System;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using log4net;

#endregion

namespace NetAppCommon.Logging.ClientMessageInspector
{
    #region public class SoapEndpointBehavior : IEndpointBehavior

    /// <summary>
    ///     Zachowanie SOAP w punkcie końcowym
    ///     Soap Endpoint Behavior
    /// </summary>
    public class SoapEndpointBehavior : IEndpointBehavior
    {
        #region private readonly log4net.ILog log4net...

        /// <summary>
        ///     Log4net Logger
        ///     Log4net Logger
        /// </summary>
        private readonly ILog _log4Net = Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        #region public SoapEndpointBehavior()

        /// <summary>
        ///     Konstruktor bez parametrów - ustawia nowy obiekt Inspektor wiadomości klienta SOAP
        ///     Constructor with no parameters - Sets a new SOAP client Message Inspector object
        /// </summary>
        public SoapEndpointBehavior()
        {
            SoapClientMessageInspector = new SoapClientMessageInspector();
        }

        #endregion

        #region public SoapEndpointBehavior(SoapClientMessageInspector soapClientMessageInspector)

        /// <summary>
        ///     Konstruktor przyjmujący argument obiektu Inspektor wiadomości klienta SOAP
        ///     A constructor that takes an argument of the SOAP client's Message Inspector object
        /// </summary>
        /// <param name="soapClientMessageInspector">
        ///     Inspektor wiadomości klienta SOAP jako SoapClientMessageInspector
        ///     The SOAP client message inspector as the SoapClientMessageInspector
        /// </param>
        public SoapEndpointBehavior(SoapClientMessageInspector soapClientMessageInspector)
        {
            SoapClientMessageInspector = soapClientMessageInspector ??
                                         throw new ArgumentNullException(nameof(soapClientMessageInspector));
        }

        #endregion

        #region public SoapClientMessageInspector SoapClientMessageInspector { get; }

        /// <summary>
        ///     Inspektor wiadomości klienta SOAP
        ///     Soap Client Message Inspector
        /// </summary>
        public SoapClientMessageInspector SoapClientMessageInspector { get; }

        #endregion

        #region public virtual void AddBindingParameters...

        /// <summary>
        ///     Dodaj parametry powiązania
        ///     Add Binding Parameters
        /// </summary>
        /// <param name="endpoint">
        ///     Punkt końcowy jako ServiceEndpoint
        ///     Endpoint as a ServiceEndpoint
        /// </param>
        /// <param name="bindingParameters">
        ///     Kolekcja parametrów wiązania jako BindingParameterCollection
        ///     Collection of binding parameters as BindingParameterCollection
        /// </param>
        public virtual void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        #endregion

        #region public virtual void ApplyClientBehavior...

        /// <summary>
        ///     Zastosuj zachowanie klienta
        ///     Apply Client Behavior
        /// </summary>
        /// <param name="endpoint">
        ///     Punkt końcowy jako ServiceEndpoint
        ///     Endpoint as a ServiceEndpoint
        /// </param>
        /// <param name="clientRuntime">
        ///     Środowisko wykonawcze klienta jako ClientRuntime
        ///     Client Runtime as ClientRuntime
        /// </param>
        public virtual void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) =>
            clientRuntime.ClientMessageInspectors.Add(SoapClientMessageInspector);

        #endregion

        #region public virtual void ApplyDispatchBehavior...

        /// <summary>
        ///     Zastosuj zachowanie wysyłki
        ///     Apply Dispatch Behavior
        /// </summary>
        /// <param name="endpoint">
        ///     Punkt końcowy jako ServiceEndpoint
        ///     Endpoint as a ServiceEndpoint
        /// </param>
        /// <param name="endpointDispatcher">
        ///     Dyspozytor punktów końcowych jako EndpointDispatcher
        ///     Endpoint Dispatcher as EndpointDispatcher
        /// </param>
        public virtual void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        #endregion

        #region public virtual void Validate...

        /// <summary>
        ///     Sprawdź poprawność
        ///     Validate
        /// </summary>
        /// <param name="endpoint">
        ///     Punkt końcowy jako ServiceEndpoint
        ///     Endpoint as a ServiceEndpoint
        /// </param>
        public virtual void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }

    #endregion
}
