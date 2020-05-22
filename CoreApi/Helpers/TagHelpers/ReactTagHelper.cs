using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CoreApi.Helpers.TagHelpers
{
    /// <summary>
    /// This Tag Helper will translate reactjs script to Javascript.
    /// </summary>
    [HtmlTargetElement("script", Attributes = "type,react")]
    public class ReactTagHelper : TagHelper
    {
        private readonly INodeServices _nodeServices;
        private readonly IHostingEnvironment _environment;

        public ReactTagHelper(INodeServices nodeServices, IHostingEnvironment environment)
        {
            _nodeServices = nodeServices;
            _environment = environment;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Check tag type is text/babel
            if (context.AllAttributes.TryGetAttribute("type", out var attribute))
            {
                if (attribute.Value.ToString().ToLower().Trim().Equals("text/babel"))
                {
                    // Only auto render at server side when in Staging or Production
                    output.PreElement.AppendHtml("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/react/16.0.0/umd/react.production.min.js\"></script>\r\n\t");
                    output.PreElement.AppendHtml("<script src=\"https://cdnjs.cloudflare.com/ajax/libs/react-dom/16.0.0/umd/react-dom.production.min.js\"></script>\r\n\t");

                    // Get content of Tag
                    var content = (await output.GetChildContentAsync()).GetContent();
                    var codeRendered = await _nodeServices.InvokeAsync<string>("Scripts/babel-engine.js", content);

                    output.Attributes.SetAttribute("type", "text/javascript");
                    output.Content.SetHtmlContent(codeRendered);
                }
            }
        }
    }
}
