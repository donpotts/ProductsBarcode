using System.Threading.Tasks;

namespace AppProducts.Shared.Blazor.Services
{
    public enum AppPlatform
    {
        Blazor,
        Maui,
        Unknown
    }

    public interface ICustomPopupService
    {
        Task<string?> ShowPopup(string message);
        Task<AppPlatform> GetPlatform();
    }
}
