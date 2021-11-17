#region using

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NetAppCommon.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

#endregion

#region namespace

namespace NetAppCommon.Logging.RestClient.Models.Base;

/// <summary>
///     Historia żądań i odpowiedzi, rozszerzenie do RestSharp
///     Request and response history, RestSharp extension
/// </summary>
[Table("RequestAndResponseHistory", Schema = "dbo")]
[Index(nameof(RequestUrl))]
[Index(nameof(ResponseStatusCode))]
[Index(nameof(RequestDateTime))]
[Index(nameof(RequestDateTimeAsDateTime))]
[JsonObject("RequestAndResponseHistory")]
[Display(Name = "Historia żądań i odpowiedzi", Description = "Historia żądań i odpowiedzi, rozszerzenie RestSharp")]
public class RequestAndResponseHistoryBase : BaseEntity
{
    #region private RestClient RestClient { get; set; }

    /// <summary>
    ///     RestClient RestClient
    ///     RestClient RestClient
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    private RestSharp.RestClient RestClient { get; set; }

    #endregion

    #region private RestRequest RestRequest { get; set; }

    /// <summary>
    ///     RestRequest RestRequest
    ///     RestRequest RestRequest
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    private RestRequest RestRequest { get; set; }

    #endregion

    #region private IRestResponse RestResponse { get; set; }

    /// <summary>
    ///     T RestResponse
    ///     T RestResponse
    /// </summary>
    [NotMapped]
    [JsonIgnore]
    private IRestResponse RestResponse { get; set; }

    #endregion

    #region public string RequestUrl { get; private set; }

    /// <summary>
    ///     Adres URL żądania jako string
    ///     URL of the request as a string
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(RequestUrl), Order = 2)]
    [Display(Name = "Adres URL żądania", Prompt = "Wpisz adres URL żądania", Description = "Adres URL żądania")]
    [StringLength(512)]
    [Required]
    public string RequestUrl { get; private set; }

    #endregion

    #region public string RequestParameters { get; private set; }

    /// <summary>
    ///     Parametry żądania jako tekst json
    ///     Request parameters as json text
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(RequestParameters), Order = 2)]
    [Display(Name = "Parametry żądania", Prompt = "Wpisz Parametry żądania", Description = "Parametry żądania")]
    [Required]
    [MaxLength(2147483647)]
    [StringLength(2147483647)]
    public string RequestParameters { get; private set; }

    #endregion

    #region public string RequestBody { get; private set; }

    /// <summary>
    ///     Treść, ciało żądania
    ///     The content (body) of the request
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(RequestBody), Order = 2)]
    [Display(Name = "Treść, ciało żądania", Prompt = "Wpisz treść, ciało żądania",
        Description = "Treść, ciało żądania")]
    [Required]
    [MaxLength(2147483647)]
    [StringLength(2147483647)]
    public string RequestBody { get; private set; }

    #endregion

    #region public HttpStatusCode ResponseStatusCode { get; private set; }

    /// <summary>
    ///     Status odpowiedzi HttpStatusCode
    ///     The HttpStatusCode response status
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(ResponseStatusCode), Order = 2)]
    [Display(Name = "Status odpowiedzi HttpStatusCode", Prompt = "Wpisz status odpowiedzi HttpStatusCode",
        Description = "Status odpowiedzi HttpStatusCode")]
    [Required]
    [StringLength(32)]
    public string ResponseStatusCode { get; private set; }

    #endregion

    #region public string ResponseHeaders { get; private set; }

    /// <summary>
    ///     Zawartość nagłówków odpowiedzi jako json tekst z obiektu RestResponse.Headers
    ///     The contents of the response headers as json text from the RestResponse.Headers object
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(ResponseHeaders), Order = 2)]
    [Display(Name = "Zawartość nagłówków odpowiedzi", Prompt = "Wpisz zawartość nagłówków odpowiedzi",
        Description = "Zawartość nagłówków odpowiedzi")]
    [Required]
    [MaxLength(2147483647)]
    [StringLength(2147483647)]
    public string ResponseHeaders { get; private set; }

    #endregion

    #region public string ResponseContent { get; private set; }

    /// <summary>
    ///     Zawartość treści odpowiedzi jako string
    ///     The content of the response body as a string
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(ResponseContent), Order = 2)]
    [Display(Name = "Treść, ciało żądania", Prompt = "Wpisz treść, ciało żądania",
        Description = "Treść, ciało żądania")]
    [Required]
    [MaxLength(2147483647)]
    [StringLength(2147483647)]
    public string ResponseContent { get; private set; }

    #endregion

    #region public string RequestDateTime { get; set; }

    /// <summary>
    ///     Data wysłania żądania
    ///     Date the request was sent
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(RequestDateTime), Order = 2)]
    [Display(Name = "Data wysłania żądania", Prompt = "Wpisz lub wybierz datę wysłania żądania",
        Description = "Data wysłania żądania")]
    [StringLength(32)]
    [Required]
    public string RequestDateTime { get; private set; }

    #endregion

    #region DateTime? RequestDateTimeAsDateTime { get; private set; }

    /// <summary>
    ///     Data wysłania żądania
    ///     Date the request was sent
    /// </summary>
    [Column(Order = 2)]
    [JsonProperty(nameof(RequestDateTimeAsDateTime), Order = 2)]
    [Display(Name = "Data wysłania żądania", Prompt = "Wpisz lub wybierz datę wysłania żądania",
        Description = "Data wysłania żądania")]
    [DataType(DataType.DateTime)]
    public DateTime? RequestDateTimeAsDateTime { get; private set; }

    #endregion

    #region RequestAndResponseHistory SetRequestAndResponseHistory<T>(RestClient client, RestRequest request, T restResponse) where T : IRestResponse

    /// <summary>
    ///     Konstruktor
    ///     Constructor
    /// </summary>
    /// <param name="client">
    ///     RestClient client
    ///     RestClient client
    /// </param>
    /// <param name="request">
    ///     RestRequest request
    ///     RestRequest request
    /// </param>
    /// <param name="restResponse">
    ///     T restResponse
    ///     T restResponse
    /// </param>
    public RequestAndResponseHistoryBase SetRequestAndResponseHistory<T>(RestSharp.RestClient client,
        RestRequest request,
        T restResponse) where T : IRestResponse
    {
        RestClient = client;
        RestRequest = request;
        RestResponse = restResponse;
        SetRequestUrl();
        SetRequestDateTime();
        SetRequestDateTimeAsDateTime();
        SetRequestParameters();
        SetRequestBody();
        SetResponseStatusCode();
        SetResponseHeaders();
        SetResponseContent();
        return this;
    }

    #endregion

    #region public void SetRequestUrl()

    /// <summary>
    ///     Ustaw Adres URL żądania
    ///     Set the Request URL
    /// </summary>
    public void SetRequestUrl()
    {
        try
        {
            RequestUrl = RestClient.BuildUri(RestRequest).AbsoluteUri;
        }
        catch (Exception)
        {
            RequestUrl = null;
        }
    }

    #endregion

    #region public void SetRequestParameters()

    /// <summary>
    ///     Ustaw Parametry żądania z RequestParameters jako tekst json
    ///     Set the Request Parameters from RequestParameters as json text
    /// </summary>
    public void SetRequestParameters()
    {
        try
        {
            RequestParameters = JsonConvert.SerializeObject(RestRequest.Parameters);
        }
        catch (Exception)
        {
            RequestParameters = null;
        }
    }

    #endregion

    #region public void SetRequestBody()

    /// <summary>
    ///     Ustaw Treść, ciało żądania z obiektu RestRequest jako tekst json
    ///     Set the Body of the request from the RestRequest object as json text
    /// </summary>
    public void SetRequestBody()
    {
        try
        {
            RequestBody = JsonConvert.SerializeObject(RestRequest.Body);
        }
        catch (Exception)
        {
            RequestBody = null;
        }
    }

    #endregion

    #region public void SetResponseStatusCode()

    /// <summary>
    ///     Ustaw status odpowiedzi HttpStatusCode
    ///     Set the HttpStatusCode response status
    /// </summary>
    public void SetResponseStatusCode()
    {
        try
        {
            ResponseStatusCode = RestResponse.StatusCode.ToString();
        }
        catch (Exception)
        {
            ResponseStatusCode = HttpStatusCode.NotFound.ToString();
            ;
        }
    }

    #endregion

    #region public void SetResponseHeaders()

    /// <summary>
    ///     Ustaw zawartość nagłówków odpowiedzi jako json tekst z obiektu RestResponse.Headers
    ///     Set the contents of the response headers as json text from the RestResponse.Headers object
    /// </summary>
    public void SetResponseHeaders()
    {
        try
        {
            ResponseHeaders = JsonConvert.SerializeObject(RestResponse.Headers);
        }
        catch (Exception)
        {
            ResponseHeaders = null;
        }
    }

    #endregion

    #region public void SetResponseContent()

    /// <summary>
    ///     Ustaw zawartość treści odpowiedzi jako string
    ///     Set the content of the response body as a string
    /// </summary>
    public void SetResponseContent()
    {
        try
        {
            ResponseContent = RestResponse.Content;
        }
        catch (Exception)
        {
            ResponseContent = null;
        }
    }

    #endregion

    #region public void SetRequestDateTime()

    /// <summary>
    ///     Ustaw datę wysłania żądania
    ///     Set the date on which the request was sent
    /// </summary>
    public void SetRequestDateTime()
    {
        try
        {
            var jObject = (JObject)JsonConvert.DeserializeObject(RestResponse.Content);
            if (null != jObject)
            {
                RequestDateTime = jObject["result"]["requestDateTime"].Value<string>();
            }
        }
        catch (Exception)
        {
            RequestDateTime = string.Empty;
        }
    }

    #endregion

    #region public void SetRequestDateTimeAsDateTime()

    /// <summary>
    ///     Ustaw datę wysłania żądania z parametru string RequestDateTime
    ///     Set the date on which the request was sent from the string RequestDateTime parameter
    /// </summary>
    /// <returns>
    ///     this jako EntityItem
    ///     this as EntityItem
    /// </returns>
    public void SetRequestDateTimeAsDateTime()
    {
        try
        {
            DateTime.TryParse(RequestDateTime, out DateTime outRequestDateTimeAsDateTime);
            RequestDateTimeAsDateTime = outRequestDateTimeAsDateTime != DateTime.MinValue
                ? outRequestDateTimeAsDateTime
                : (DateTime?)DateTime.Now;
        }
        catch (Exception)
        {
            RequestDateTimeAsDateTime = DateTime.Now;
        }
    }

    #endregion
}

#endregion
