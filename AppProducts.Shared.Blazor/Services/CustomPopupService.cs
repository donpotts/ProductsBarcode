using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppProducts.Shared.Blazor.Services
{
    public class CustomPopupService : ICustomPopupService
    {
        private string resultValue;

        public async Task<string> ShowPopup(string message)
        {
            return null;
        }

        public async Task<AppPlatform> GetPlatform()
        {
            if (OperatingSystem.IsBrowser())
            {
                return AppPlatform.Blazor;
            }
            else if (OperatingSystem.IsMacCatalyst() || OperatingSystem.IsIOS() || OperatingSystem.IsAndroid())
            {
                return AppPlatform.Maui;
            }
            else
            {
                return AppPlatform.Unknown;
            }
        }
    }
}
