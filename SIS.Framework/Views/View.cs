using System;
using System.Collections.Generic;
using System.IO;
using SIS.Framework.ActionResults.Contracts;
using System.Linq;

namespace SIS.Framework.Views
{
    public class View : IRenderable
    {
        private readonly string fullyQualifiedTemplateName;

        private readonly IDictionary<string, object> viewData;

        public View(string fullyQualifiedTemplateName, IDictionary<string, object> viewData)
        {
            this.fullyQualifiedTemplateName = fullyQualifiedTemplateName;
            this.viewData = viewData;
        }

        private string ReadFile()
        {
            if (!File.Exists(this.fullyQualifiedTemplateName))
            {
                throw new FileNotFoundException($"View does not exist at {fullyQualifiedTemplateName}");
            }

            return File.ReadAllText(this.fullyQualifiedTemplateName);
        }

        public string Render()
        {
            var fullHtml = this.ReadFile();
            string renderHtml = this.RenderHtml(fullHtml);
            return renderHtml;
        }

        private string RenderHtml(string fullHtml)
        {
            string renderedHtml = fullHtml;

            if (this.viewData.Any())
            {
                foreach (var parameter in this.viewData)
                {
                    renderedHtml = renderedHtml
                            .Replace(
                                $"{{{{{{{parameter.Key}}}}}}}", 
                                    parameter.Value.ToString());
                }
            }

            return renderedHtml;
        }
    }
}
