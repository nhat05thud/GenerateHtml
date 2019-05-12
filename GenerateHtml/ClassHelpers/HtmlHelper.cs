using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GenerateHtml.ClassHelpers
{
    public static class HtmlHelper
    {
        public static string IsSelected(this System.Web.Mvc.HtmlHelper html, string controllers = "", string actions = "", string cssClass = "active")
        {
            var viewContext = html.ViewContext;
            var isChildAction = viewContext.Controller.ControllerContext.IsChildAction;

            if (isChildAction)
                viewContext = html.ViewContext.ParentActionViewContext;

            var routeValues = viewContext.RouteData.Values;
            var currentAction = routeValues["action"].ToString();
            var currentController = routeValues["controller"].ToString();

            if (string.IsNullOrEmpty(actions))
            {
                actions = currentAction;
            }

            if (string.IsNullOrEmpty(controllers))
            {
                controllers = currentController;
            }
            var acceptedActions = actions.Trim().Split(',').Distinct().ToArray();
            var acceptedControllers = controllers.Trim().Split(',').Distinct().ToArray();

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ? cssClass : string.Empty;
        }
    }
}