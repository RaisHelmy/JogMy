using Microsoft.AspNetCore.Mvc.Razor;

namespace JogMy.Extensions
{
    public class FeatureViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // This method is required but we don't need to populate any values
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var featureViewLocations = new[]
            {
                "/Features/{1}/Views/{0}.cshtml",
                "/Features/{1}/Views/Shared/{0}.cshtml",
                "/Features/Shared/Views/{0}.cshtml"
            };

            return featureViewLocations.Concat(viewLocations);
        }
        
        public IEnumerable<string> ExpandViewImportsLocations(ViewLocationExpanderContext context, IEnumerable<string> viewImportsLocations)
        {
            var featureViewImportsLocations = new[]
            {
                "/Features/{0}/_ViewImports.cshtml",
                "/Features/_ViewImports.cshtml"
            };

            return featureViewImportsLocations.Concat(viewImportsLocations);
        }
        
        public IEnumerable<string> ExpandViewStartLocations(ViewLocationExpanderContext context, IEnumerable<string> viewStartLocations)
        {
            var featureViewStartLocations = new[]
            {
                "/Features/{0}/_ViewStart.cshtml",
                "/Features/_ViewStart.cshtml"
            };

            return featureViewStartLocations.Concat(viewStartLocations);
        }
    }
}