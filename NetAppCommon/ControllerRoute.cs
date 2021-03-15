#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NetAppCommon.Models;

#endregion

namespace NetAppCommon
{
    #region public class ControllerRoute

    /// <summary>
    ///     Pomocnik tras routingów kontrolera
    ///     Controller Routing Helper
    /// </summary>
    public class ControllerRoute
    {
        #region private readonly log4net.ILog log4net...

        /// <summary>
        ///     private readonly ILog _log4Net
        /// </summary>
        private static readonly ILog Log4net =
            Log4NetLogger.Log4NetLogger.GetLog4NetInstance(MethodBase.GetCurrentMethod()?.DeclaringType);

        #endregion

        public static void GetA(Assembly assembly, IActionDescriptorCollectionProvider provider,
            IUrlHelper url)
        {
            try
            {
                var controllerRoutingActionsList = new List<ControllerRoutingActions>();
                provider.ActionDescriptors.Items.ToList()
                    .OrderBy(o => o.DisplayName) /*.ThenBy(o => o.RouteValues["Action"])*/.ToList().ForEach(
                        actionDescriptorItem =>
                        {
                            var routeParameters = new Dictionary<string, string>();
                            actionDescriptorItem.Parameters.ToList().ForEach(parameter =>
                            {
                                routeParameters.Add(parameter.Name, parameter.ParameterType.Name);
                            });
                            var routeUrlAction = string.Empty;
                            try
                            {
                                routeUrlAction = null != actionDescriptorItem.AttributeRouteInfo?.Template! &&
                                                 !string.IsNullOrWhiteSpace(
                                                     actionDescriptorItem.AttributeRouteInfo?.Template!)
                                    ? actionDescriptorItem.AttributeRouteInfo?.Template!
                                    : url.Action(actionDescriptorItem.RouteValues["Action"],
                                        actionDescriptorItem.RouteValues["Controller"]);
                            }
                            catch (Exception e)
                            {
                                Log4net.Error(
                                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n",
                                    e);
                            }

                            var controllerRoutingAction = new ControllerRoutingActions
                            {
                                RouteId = actionDescriptorItem.Id,
                                RouteController = actionDescriptorItem.RouteValues["Controller"],
                                RouteAction = actionDescriptorItem.RouteValues["Action"],
                                RouteUrlAction = routeUrlAction,
                                RouteUrlAbsoluteAction =
                                    //$"{controllerBase.Request.Scheme}://{controllerBase.Request.Host}/{routeUrlAction}",
                                    string.Empty,
                                RouteParameters = routeParameters,
                                RouteAttributeInfoTemplate = actionDescriptorItem.AttributeRouteInfo?.Template!
                            };
                            controllerRoutingActionsList.Add(controllerRoutingAction);
                        });
                foreach (var controllerRoutingAction in controllerRoutingActionsList)
                {
                    Console.WriteLine(
                        $"{controllerRoutingAction.RouteController} / {controllerRoutingAction.RouteUrlAction}");
                }
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }
        }

        #region public static List<ControllerRoutingActions> GetRouteAction...

        /// <summary>
        ///     Pobierz listę akcji (tras) dostępnych dla kontrolera
        ///     Get the list of actions (routes) available for the controller
        /// </summary>
        /// <param name="provider">
        ///     Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        ///     Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        ///     Nazwa kontrolera controllerName jako string
        ///     The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        ///     url jako IUrlHelper
        ///     url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        ///     controllerBase jako ControllerBase
        ///     controllerBase as ControllerBase
        /// </param>
        /// <returns>
        ///     Lista dostępnych tras jako List dla Route
        ///     List of available routes as List for Route
        /// </returns>
        public static List<ControllerRoutingActions> GetRouteAction(IActionDescriptorCollectionProvider provider,
            string controllerName, IUrlHelper url, ControllerBase controllerBase)
        {
            try
            {
                var controllerRoutingActionsList = new List<ControllerRoutingActions>();
                provider.ActionDescriptors.Items.ToList().Where(w => w.RouteValues["Controller"] == controllerName)
                    .ToList().ForEach(actionDescriptorItem =>
                    {
                        var routeParameters = new Dictionary<string, string>();
                        actionDescriptorItem.Parameters.ToList().ForEach(parameter =>
                        {
                            routeParameters.Add(parameter.Name, parameter.ParameterType.Name);
                        });
                        var routeUrlAction = null != actionDescriptorItem.AttributeRouteInfo.Template &&
                                             !string.IsNullOrWhiteSpace(
                                                 actionDescriptorItem.AttributeRouteInfo.Template)
                            ? actionDescriptorItem.AttributeRouteInfo.Template
                            : url.Action(actionDescriptorItem.RouteValues["Action"],
                                actionDescriptorItem.RouteValues["Controller"]);
                        var controllerRoutingAction = new ControllerRoutingActions
                        {
                            RouteId = actionDescriptorItem.Id,
                            RouteController = actionDescriptorItem.RouteValues["Controller"],
                            RouteAction = actionDescriptorItem.RouteValues["Action"],
                            RouteUrlAction = routeUrlAction,
                            RouteUrlAbsoluteAction =
                                $"{controllerBase.Request.Scheme}://{controllerBase.Request.Host}/{routeUrlAction}",
                            RouteParameters = routeParameters,
                            RouteAttributeInfoTemplate = actionDescriptorItem.AttributeRouteInfo.Template
                        };
                        controllerRoutingActionsList.Add(controllerRoutingAction);
                    });
                return controllerRoutingActionsList;
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return null;
        }

        #endregion

        #region public static async Task<List<ControllerRoutingActions>> GetRouteActionAsync...

        /// <summary>
        ///     Pobierz listę akcji (tras) dostępnych dla kontrolera asynchronicznie
        ///     Get the list of actions (routes) available for the controller asynchronously
        /// </summary>
        /// <param name="provider">
        ///     Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        ///     Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        ///     Nazwa kontrolera controllerName jako string
        ///     The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        ///     url jako IUrlHelper
        ///     url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        ///     controllerBase jako ControllerBase
        ///     controllerBase as ControllerBase
        /// </param>
        /// <returns>
        ///     Lista dostępnych tras jako List dla Route
        ///     List of available routes as List for Route
        /// </returns>
        public static async Task<List<ControllerRoutingActions>> GetRouteActionAsync(
            IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url,
            ControllerBase controllerBase)
        {
            try
            {
                return await Task.Run(() => GetRouteAction(provider, controllerName, url, controllerBase));
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error($"{e.Message}, {e.StackTrace}.", e));
            }

            return null;
        }

        #endregion

        #region public static KendoGrid<List<ControllerRoutingActions>> GetRouteActionForKendoGrid...

        /// <summary>
        ///     Pobierz listę akcji (tras) dostępnych dla kontrolera i zwróć listę dla widoku Kendo
        ///     Get the list of actions (routes) available for the controller and return the list for the Kendo view
        /// </summary>
        /// <param name="provider">
        ///     Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        ///     Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        ///     Nazwa kontrolera controllerName jako string
        ///     The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        ///     url jako IUrlHelper
        ///     url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        ///     controllerBase jako ControllerBase
        ///     controllerBase as ControllerBase
        /// </param>
        /// <returns>
        ///     Lista dostępnych tras routingu jako List dla KendoGrid
        ///     List of available routing routes as List for KendoGrid
        /// </returns>
        public static KendoGrid<List<ControllerRoutingActions>> GetRouteActionForKendoGrid(
            IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url,
            ControllerBase controllerBase)
        {
            try
            {
                var routes = GetRouteAction(provider, controllerName, url, controllerBase);
                if (null != routes && routes.Count > 0)
                {
                    return new KendoGrid<List<ControllerRoutingActions>> {Total = routes.Count, Data = routes};
                }
            }
            catch (Exception e)
            {
                Log4net.Error(
                    $"\n{e.GetType()}\n{e.InnerException?.GetType()}\n{e.Message}\n{e.StackTrace}\n", e);
            }

            return null;
        }

        #endregion

        #region public static async Task<KendoGrid<List<Route>>> GetRouteActionForKendoGridAsync...

        /// <summary>
        ///     Pobierz listę akcji (tras) dostępnych dla kontrolera i zwróć listę dla widoku Kendo
        ///     Get the list of actions (routes) available for the controller and return the list for the Kendo view
        /// </summary>
        /// <param name="provider">
        ///     Dostawca kolekcji deskryptorów akcji provider jako IActionDescriptorCollectionProvider
        ///     Provider's action descriptor collection provider as IActionDescriptorCollectionProvider
        /// </param>
        /// <param name="controllerName">
        ///     Nazwa kontrolera controllerName jako string
        ///     The name of controller controllerName as a string
        /// </param>
        /// <param name="Url">
        ///     url jako IUrlHelper
        ///     url as IUrlHelper
        /// </param>
        /// <param name="controllerBase">
        ///     controllerBase jako ControllerBase
        ///     controllerBase as ControllerBase
        /// </param>
        /// <returns>
        ///     Lista dostępnych tras routingu jako List dla KendoGrid
        ///     List of available routing routes as List for KendoGrid
        /// </returns>
        public static async Task<KendoGrid<List<ControllerRoutingActions>>> GetRouteActionForKendoGridAsync(
            IActionDescriptorCollectionProvider provider, string controllerName, IUrlHelper url,
            ControllerBase controllerBase)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var routes =
                        await GetRouteActionAsync(provider, controllerName, url, controllerBase);
                    if (null != routes && routes.Count > 0)
                    {
                        return new KendoGrid<List<ControllerRoutingActions>> {Total = routes.Count, Data = routes};
                    }

                    return null;
                });
            }
            catch (Exception e)
            {
                await Task.Run(() => Log4net.Error($"{e.Message}, {e.StackTrace}.", e));
            }

            return null;
        }

        #endregion
    }

    #endregion
}