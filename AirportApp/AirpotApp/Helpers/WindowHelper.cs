using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace AirportApp.Helpers
{
    public static class WindowHelper
    {
        public static void MaximizeWindow(Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow is not null)
            {
                appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);

                appWindow.Resize(new SizeInt32(displayArea.WorkArea.Width, displayArea.WorkArea.Height));
                appWindow.Move(new PointInt32(displayArea.WorkArea.X, displayArea.WorkArea.Y));
            }
        }
    }
}
