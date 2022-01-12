using Microsoft.Extensions.Localization;
using System.Reflection;

namespace AdminHubNC6.Resources
{
    public class AuthLocalizationService
    {
        private readonly IStringLocalizer _localizer;

        public AuthLocalizationService(IStringLocalizerFactory factory)
        {
            var type = typeof(AuthResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("AuthResource", assemblyName.Name);
        }

        public LocalizedString GetLocalizedHtmlString(string key)
        {
            return _localizer[key];
        }

        public LocalizedString GetLocalizedHtmlString(string key, string parameter)
        {
            return _localizer[key, parameter];
        }
    }
}