using System;
using AirportApp.Src.ViewModel;
using AirportApp.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AirportApp.Src.View.Chat
{
    public sealed partial class ChatPage : Page
    {
        public ChatViewModel ViewModel { get; }
        private readonly INavigationService navigationService;
        public ChatPage()
        {
            ViewModel = App.Services.GetService<ChatViewModel>();
            this.InitializeComponent();
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        public async void EndChat(object sender, RoutedEventArgs arguments)
        {
            await ViewModel.CloseChatAsync();
            navigationService.NavigateTo(typeof(AirportApp.Src.View.General.LandingPage));
        }
    }
}