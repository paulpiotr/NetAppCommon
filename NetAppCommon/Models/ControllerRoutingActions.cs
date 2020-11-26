using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace NetAppCommon.Models
{
    #region public class ControllerRoutingActions
    /// <summary>
    /// Akcje routingu kontrolera
    /// Controller routing actions
    /// </summary>
    public class ControllerRoutingActions
    {
        #region public string RouteId { get; set; }
        /// <summary>
        /// Identyfikator trasy routera jako string
        /// The router's route ID as a string
        /// </summary>
        [JsonProperty(nameof(RouteId))]
        [Display(Name = "Identyfikator trasy routera", Prompt = "Wpisz identyfikator trasy routera", Description = "Identyfikator trasy routera")]
        public string RouteId { get; set; }
        #endregion

        #region public string RouteController { get; set; }
        /// <summary>
        /// Nazwa kontrolera jako string
        /// Controller name as a string
        /// </summary>
        [JsonProperty(nameof(RouteController))]
        [Display(Name = "Nazwa kontrolera", Prompt = "Wpisz nazwę kontrolera", Description = "Nazwa kontrolera")]
        public string RouteController { get; set; }
        #endregion

        #region public string RouteAction { get; set; }
        /// <summary>
        /// Nazwa akcji kontrolera jako string
        /// Controller action name as a string
        /// </summary>
        [JsonProperty(nameof(RouteAction))]
        [Display(Name = "Nazwa akcji kontrolera", Prompt = "Wpisz nazwę akcji kontrolera", Description = "Nazwa akcji kontrolera")]
        public string RouteAction { get; set; }
        #endregion

        #region public string RouteUrlAction { get; set; }
        /// <summary>
        /// Adres URL dla akcji kontrolera jako string
        /// The URL for the controller action as a string
        /// </summary>
        [JsonProperty(nameof(RouteUrlAction))]
        [Display(Name = "Adres URL dla akcji kontrolera", Prompt = "Wpisz adres URL dla akcji kontrolera", Description = "Adres URL dla akcji kontrolera")]
        public string RouteUrlAction { get; set; }
        #endregion

        #region public string RouteUrlAbsoluteAction { get; set; }
        /// <summary>
        /// Absolutny adres URL dla akcji kontrolera jako string
        /// Absolute URL for a controller action as a string
        /// </summary>
        [JsonProperty(nameof(RouteUrlAbsoluteAction))]
        [Display(Name = "Absolutny adres URL dla akcji kontrolera", Prompt = "Wpisz absolutny adres URL dla akcji kontrolera", Description = "Absolutny adres URL dla akcji kontrolera")]
        public string RouteUrlAbsoluteAction { get; set; }
        #endregion
    }
    #endregion
}