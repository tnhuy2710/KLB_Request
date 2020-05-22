using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace CoreApi.Helpers.TagHelpers
{

    /// <summary>
    /// Auto add "active" class name to element.
    /// </summary>
    [HtmlTargetElement(Attributes = "active-actions")]
    [HtmlTargetElement(Attributes = "active-controllers")]
    [HtmlTargetElement(Attributes = "active-class")]
    [HtmlTargetElement(Attributes = "active-route")]
    public class ActiveClassHelper : TagHelper
    {
        private const string ActionsAttributeName = "active-actions";
        private const string ControllersAttributeName = "active-controllers";
        private const string ClassAttributeName = "active-class";
        private const string RouteAttributeName = "active-route";


        [HtmlAttributeName(ControllersAttributeName)]
        public string Controllers { get; set; }

        [HtmlAttributeName(ActionsAttributeName)]
        public string Actions { get; set; }

        [HtmlAttributeName(RouteAttributeName)]
        public string Route { get; set; }

        [HtmlAttributeName(ClassAttributeName)]
        public string Class { get; set; } = "active";

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            RouteValueDictionary routeValues = ViewContext.RouteData.Values;
            string currentAction = routeValues["action"].ToString();
            string currentController = routeValues["controller"].ToString();

            if (string.IsNullOrEmpty(Actions))
                Actions = currentAction;

            if (string.IsNullOrEmpty(Controllers))
                Controllers = currentController;

            string[] acceptedActions = Actions.Trim().Split(',').Distinct().ToArray();
            string[] acceptedControllers = Controllers.Trim().Split(',').Distinct().ToArray();


            if (acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController))
            {
                SetAttribute(output, "class", Class);
            }

            base.Process(context, output);
        }

        private void SetAttribute(TagHelperOutput output, string attributeName, string value, bool merge = true)
        {
            var v = value;
            if (output.Attributes.TryGetAttribute(attributeName, out var attribute))
            {
                if (merge)
                {
                    v = $"{attribute.Value} {value}";
                }
            }
            output.Attributes.SetAttribute(attributeName, v);
        }
    }
}
