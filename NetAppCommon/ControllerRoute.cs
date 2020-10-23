using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NetAppCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetAppCommon
{
    #region public class ControllerRoute
    /// <summary>
    /// Pomocnik tras routingów kontrolera
    /// Controller Routing Helper
    /// </summary>
    public class ControllerRoute
    {
        #region private static readonly log4net.ILog log4net...
        /// <summary>
        /// Log4 Net Logger
        /// </summary>
        private static readonly log4net.ILog log4net = Log4netLogger.Log4netLogger.GetLog4netInstance(MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region public static List<ControllerRoutingActions> GetRouteAction...
        /// <summary>
        /// Pobierz listę akcji (tras) dostępnych dla kontrolera
        /// Get the list of actions (routes) available for the controller
        /// </summary>
        /// <param name="provider">
        /// Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        /// Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        /// Nazwa kontrolera controllerName jako string
        /// The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        /// url jako IUrlHelper
        /// url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        /// controllerBase jako ControllerBase
        /// controllerBase as ControllerBase
        /// </param>
        /// <returns>
        /// Lista dostępnych tras jako List dla Route
        /// List of available routes as List for Route
        /// </returns>
        public static List<ControllerRoutingActions> GetRouteAction(IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url, ControllerBase controllerBase)
        {
            try
            {
                return provider.ActionDescriptors.Items.ToList().Where(w => w.RouteValues["Controller"] == controllerName).Select(x => new
                ControllerRoutingActions
                {
                    RouteId = x.Id,
                    RouteController = x.RouteValues["Controller"],
                    RouteAction = x.RouteValues["Action"],
                    RouteUrlAction = url.Action(x.RouteValues["Action"], x.RouteValues["Controller"]),
                    RouteUrlAbsoluteAction = string.Format("{0}://{1}{2}", controllerBase.Request.Scheme, controllerBase.Request.Host, url.Action(x.RouteValues["Action"], x.RouteValues["Controller"])),
                }).ToList();
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static async Task<List<ControllerRoutingActions>> GetRouteActionAsync...
        /// <summary>
        /// Pobierz listę akcji (tras) dostępnych dla kontrolera asynchronicznie
        /// Get the list of actions (routes) available for the controller asynchronously
        /// </summary>
        /// <param name="provider">
        /// Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        /// Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        /// Nazwa kontrolera controllerName jako string
        /// The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        /// url jako IUrlHelper
        /// url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        /// controllerBase jako ControllerBase
        /// controllerBase as ControllerBase
        /// </param>
        /// <returns>
        /// Lista dostępnych tras jako List dla Route
        /// List of available routes as List for Route
        /// </returns>
        public static async Task<List<ControllerRoutingActions>> GetRouteActionAsync(IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url, ControllerBase controllerBase)
        {
            try
            {
                return await Task.Run(() => GetRouteAction(provider, controllerName, url, controllerBase));
            }
            catch (Exception e)
            {
                await Task.Run(() => log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
            return null;
        }
        #endregion

        #region public static KendoGrid<List<ControllerRoutingActions>> GetRouteActionForKendoGrid...
        /// <summary>
        /// Pobierz listę akcji (tras) dostępnych dla kontrolera i zwróć listę dla widoku Kendo
        /// Get the list of actions (routes) available for the controller and return the list for the Kendo view
        /// </summary>
        /// <param name="provider">
        /// Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        /// Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        /// Nazwa kontrolera controllerName jako string
        /// The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        /// url jako IUrlHelper
        /// url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        /// controllerBase jako ControllerBase
        /// controllerBase as ControllerBase
        /// </param>
        /// <returns>
        /// Lista dostępnych tras routingu jako List dla KendoGrid
        /// List of available routing routes as List for KendoGrid
        /// </returns>
        public static KendoGrid<List<ControllerRoutingActions>> GetRouteActionForKendoGrid(IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url, ControllerBase controllerBase)
        {
            try
            {
                List<ControllerRoutingActions> routes = GetRouteAction(provider, controllerName, url, controllerBase);
                if (null != routes && routes.Count > 0)
                {
                    return new KendoGrid<List<ControllerRoutingActions>>
                    {
                        Total = routes.Count,
                        Data = routes,
                    };
                }
            }
            catch (Exception e)
            {
                log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e);
            }
            return null;
        }
        #endregion

        #region public static async Task<KendoGrid<List<Route>>> GetRouteActionForKendoGridAsync...
        /// <summary>
        /// Pobierz listę akcji (tras) dostępnych dla kontrolera i zwróć listę dla widoku Kendo
        /// Get the list of actions (routes) available for the controller and return the list for the Kendo view
        /// </summary>
        /// <param name="provider">
        /// Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        /// Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        /// Nazwa kontrolera controllerName jako string
        /// The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        /// url jako IUrlHelper
        /// url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        /// controllerBase jako ControllerBase
        /// controllerBase as ControllerBase
        /// </param>
        /// <returns>
        /// Lista dostępnych tras routingu jako List dla KendoGrid
        /// List of available routing routes as List for KendoGrid
        /// </returns>
        public static async Task<KendoGrid<List<ControllerRoutingActions>>> GetRouteActionForKendoGridAsync(IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url, ControllerBase controllerBase)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    List<ControllerRoutingActions> routes = await GetRouteActionAsync(provider, controllerName, url, controllerBase);
                    if (null != routes && routes.Count > 0)
                    {
                        return new KendoGrid<List<ControllerRoutingActions>>
                        {
                            Total = routes.Count,
                            Data = routes,
                        };
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                await Task.Run(() => log4net.Error(string.Format("{0}, {1}.", e.Message, e.StackTrace), e));
            }
            return null;
        }
        #endregion
    }
    #endregion
}