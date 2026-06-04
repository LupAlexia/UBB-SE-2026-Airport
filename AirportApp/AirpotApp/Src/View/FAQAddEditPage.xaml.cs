using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.Src.View.General;
using AirportApp.Services.Interfaces;
using AirportApp.Src.ViewModel;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace AirportApp.Src.View.Faq
{
    public sealed partial class FAQAddEditPage : Page
    {
        private FAQViewModel viewModel;
        private FAQEntryDTO? editingFaq;
        private bool isEditMode;
        private int currentPersonId;
        private readonly INavigationService navigationService;

        public FAQAddEditPage()
        {
            this.InitializeComponent();

            viewModel = App.Services.GetRequiredService<FAQViewModel>();
            navigationService = App.Services.GetRequiredService<INavigationService>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            base.OnNavigatedTo(arguments);

            var app = (App)Application.Current;
            FAQEntryDTO? faqEntry = null;
            var isAdminContext = false;

            if (arguments.Parameter is FAQAddEditNavigationArgs navArgs)
            {
                viewModel = navArgs.ViewModel;
                currentPersonId = navArgs.CurrentPersonId;
                isAdminContext = navArgs.IsAdmin;
                faqEntry = navArgs.FAQEntry;
            }
            else if (arguments.Parameter is FAQNavigationData legacyNavData)
            {
                currentPersonId = legacyNavData.CurrentPersonId;
                isAdminContext = legacyNavData.IsEmployee;
                faqEntry = legacyNavData.FAQEntry;
            }
            else
            {
                isAdminContext = app.IsEmployee;
            }

            viewModel.IsAdmin = isAdminContext;

            if (!isAdminContext && app.User == null && app.Employee == null)
            {
                navigationService.NavigateTo(typeof(ChoosingPage));
                return;
            }

            if (faqEntry is not null)
            {
                editingFaq = faqEntry;
                isEditMode = true;
                viewModel.SelectedFAQEntry = faqEntry;

                QuestionTextBox.Text = editingFaq.Question;
                AnswerTextBox.Text = editingFaq.Answer;
                CategoryComboBox.SelectedItem = FindCategoryComboBoxItem(editingFaq.Category);

                PageTitleText.Text = "Edit FAQ";
                PageSubtitleText.Text = "Update the selected frequently asked question entry";
                SaveButton.Content = "Save Changes";
            }
            else if (arguments.Parameter is FAQAddEditNavigationArgs or FAQNavigationData)
            {
                editingFaq = null;
                isEditMode = false;
                viewModel.SelectedFAQEntry = null;

                QuestionTextBox.Text = string.Empty;
                AnswerTextBox.Text = string.Empty;
                CategoryComboBox.SelectedItem = null;

                PageTitleText.Text = "Add FAQ";
                PageSubtitleText.Text = "Create a frequently asked question entry";
                SaveButton.Content = "Add FAQ";
            }
        }

        private ComboBoxItem? FindCategoryComboBoxItem(FAQCategoryEnum category)
        {
            foreach (var item in CategoryComboBox.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    comboItem.Content?.ToString() == category.ToString())
                {
                    return comboItem;
                }
            }

            return null;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs arguments)
        {
            await HandleSaveChanges();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs arguments)
        {
            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }

        private async System.Threading.Tasks.Task HandleSaveChanges()
        {
            try
            {
                await viewModel.SaveAsync(
                    QuestionTextBox.Text,
                    AnswerTextBox.Text,
                    (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString());
            }
            catch (Exception exceptionThrown)
            {
                await ShowMessage("Save failed", exceptionThrown.Message);
                return;
            }

            if (navigationService.CanGoBack)
            {
                navigationService.GoBack();
            }
        }
        private async System.Threading.Tasks.Task ShowMessage(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot,
                RequestedTheme = ElementTheme.Dark
            };

            await dialog.ShowAsync();
        }
    }
}
