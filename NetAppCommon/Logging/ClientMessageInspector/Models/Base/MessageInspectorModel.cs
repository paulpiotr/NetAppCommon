#region using

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetAppCommon.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

#nullable enable annotations

namespace NetAppCommon.Logging.ClientMessageInspector.Models.Base
{
    public class MessageInspectorModel : BaseEntity
    {
        #region private string? _afterReceiveReplyMessage...

        protected string? _afterReceiveReplyMessage;

        /// <summary>
        ///     Obiekt Message po otrzymaniu odpowiedzi jako json string?
        ///     The Message object after receiving the response as a json string?
        /// </summary>
        [JsonProperty(nameof(AfterReceiveReplyMessage), Order = 2)]
        [Display(Name = "Obiekt Message",
            Prompt = "Wpisz zawartość odpowiedzi Obiekt Message jako string? json",
            Description = "Obiekt Message po otrzymaniu odpowiedzi jako json string?")]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? AfterReceiveReplyMessage
        {
            get => _afterReceiveReplyMessage;
            protected set
            {
                if (value != _afterReceiveReplyMessage)
                {
                    _afterReceiveReplyMessage = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region private string? _afterReceiveReplyMessageAsStringXml...

        protected string? _afterReceiveReplyMessageAsStringXml;

        /// <summary>
        ///     Treść wiadomości Message po otrzymaniu odpowiedzi jako xml string?
        ///     The content of the Message after receiving the response as an xml string?
        /// </summary>
        [JsonProperty(nameof(AfterReceiveReplyMessageAsStringXml), Order = 2)]
        [Display(Name = "Treść wiadomości",
            Prompt = "Wpisz treść wiadomości jako string? xml",
            Description = "Treść wiadomości Message po otrzymaniu odpowiedzi jako xml string?")]
        [MinLength(1)]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? AfterReceiveReplyMessageAsStringXml
        {
            get => _afterReceiveReplyMessageAsStringXml;
            protected set
            {
                if (value != _afterReceiveReplyMessageAsStringXml)
                {
                    _afterReceiveReplyMessageAsStringXml = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region private string? _afterReceiveReplyCorrelationState...

        protected string? _afterReceiveReplyCorrelationState;

        /// <summary>
        ///     Obiekt CorrelationState po otrzymaniu odpowiedzi jako json string?
        ///     CorrelationState object after receiving the response as a json string?
        /// </summary>
        [JsonProperty(nameof(AfterReceiveReplyCorrelationState), Order = 2)]
        [Display(Name = "Obiekt CorrelationState",
            Prompt = "Wpisz zawartość obiektu CorrelationState jako string?",
            Description = "Obiekt CorrelationState po otrzymaniu odpowiedzi jako json string?")]
        [MinLength(1)]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? AfterReceiveReplyCorrelationState
        {
            get => _afterReceiveReplyCorrelationState;
            protected set
            {
                if (value != _afterReceiveReplyCorrelationState)
                {
                    _afterReceiveReplyCorrelationState = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region private string? _beforeSendRequestMessage...

        protected string? _beforeSendRequestMessage;

        /// <summary>
        ///     Obiekt Message przed wysłaniem żądania jako json string?
        ///     The Message object after receiving the response as a json string?
        /// </summary>
        [JsonProperty(nameof(BeforeSendRequestMessage), Order = 2)]
        [Display(Name = "Obiekt Message",
            Prompt = "Wpisz zawartość odpowiedzi Obiekt Message jako string? json",
            Description = "Obiekt Message przed wysłaniem żądania jako json string?")]
        [MinLength(1)]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? BeforeSendRequestMessage
        {
            get => _beforeSendRequestMessage;
            protected set
            {
                if (value != _beforeSendRequestMessage)
                {
                    _beforeSendRequestMessage = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region private string? _beforeSendRequestMessageAsStringXml...

        protected string? _beforeSendRequestMessageAsStringXml;

        /// <summary>
        ///     Treść wiadomości Message przed wysłaniem żądania jako xml string?
        ///     The content of the Message after receiving the response as an xml string?
        /// </summary>
        [JsonProperty(nameof(BeforeSendRequestMessageAsStringXml), Order = 2)]
        [Display(Name = "Treść wiadomości",
            Prompt = "Wpisz treść wiadomości jako string? xml",
            Description = "Treść wiadomości Message przed wysłaniem żądania jako xml string?")]
        [MinLength(1)]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? BeforeSendRequestMessageAsStringXml
        {
            get => _beforeSendRequestMessageAsStringXml;
            protected set
            {
                if (value != _beforeSendRequestMessageAsStringXml)
                {
                    _beforeSendRequestMessageAsStringXml = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region private string? _beforeSendRequestClientChannel...

        protected string? _beforeSendRequestClientChannel;

        /// <summary>
        ///     Obiekt ClientChannel przed wysłaniem żądania jako json string?
        ///     ClientChannel object after receiving the response as a json string?
        /// </summary>
        [JsonProperty(nameof(BeforeSendRequestClientChannel), Order = 2)]
        [Display(Name = "Obiekt ClientChannel",
            Prompt = "Wpisz zawartość obiektu ClientChannel jako string?",
            Description = "Obiekt ClientChannel przed wysłaniem żądania jako json string?")]
        [MinLength(1)]
        [MaxLength(2147483647)]
        [StringLength(2147483647)]
        public virtual string? BeforeSendRequestClientChannel
        {
            get => _beforeSendRequestClientChannel;
            protected set
            {
                if (value != _beforeSendRequestClientChannel)
                {
                    _beforeSendRequestClientChannel = value;
                    OnPropertyChanged(value!);
                }
            }
        }

        #endregion

        #region public virtual TDestination Cast<TDestination>()

        public virtual TDestination Cast<TDestination>() where TDestination : MessageInspectorModel, new()
        {
            try
            {
                var destination = JObject.Parse(JsonConvert.SerializeObject(new TDestination()));
                var source = JObject.Parse(JsonConvert.SerializeObject(this));
                destination.Merge(source, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                return JsonConvert.DeserializeObject<TDestination>(destination.ToString());
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return Helpers.Object.ObjectHelper.GetDefaultValue<TDestination>();
        }

        #endregion
    }
}
