using System;
using AirportApp.Services.Interfaces;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Services
{
    public class NavigationService : INavigationService
    {
        private Frame? frame;

        public bool CanGoBack => frame?.CanGoBack ?? false;

        public void Initialize(Frame frame)
        {
            this.frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateTo(Type pageType, object? parameter = null)
        {
            if (frame == null)
            {
                throw new InvalidOperationException("NavigationService has not been initialized with a Frame.");
            }

            frame.Navigate(pageType, parameter);
        }

        public void GoBack()
        {
            if (frame != null && frame.CanGoBack)
            {
                frame.GoBack();
            }
        }
    }
}
