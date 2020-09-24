using System.Threading;
using Microsoft.SharePoint.Utilities;

namespace Korus.TestApplication.Common
{
    public static class ResourceAccessor
    {
        public static string GetString(string key)
        {
            const string resourcesFile = "TestApplication";

            var source = $"$Resources:{resourcesFile},{key}";

            return SPUtility.GetLocalizedString(source, resourcesFile, (uint) Thread.CurrentThread.CurrentCulture.LCID);
        }
    }
}
