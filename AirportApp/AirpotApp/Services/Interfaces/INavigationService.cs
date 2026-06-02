using System;

namespace AirportApp.Services.Interfaces
{
    public interface INavigationService
    {
        bool CanGoBack { get; }
        void Initialize(Microsoft.UI.Xaml.Controls.Frame frame);
        void NavigateTo(Type pageType, object? parameter = null);
        void GoBack();
    }
}
