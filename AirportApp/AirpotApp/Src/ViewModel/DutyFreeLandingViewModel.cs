using System.Windows.Input;

using AirportApp.ClassLibrary.Service.Interface;
using AirportApp.Src.ViewModel;

using UserSession = AirportLib.Domain.User.UserSession;

using CommunityToolkit.Mvvm.Input;

namespace AirportApp.Src.ViewModel
{
    public class DutyFreeLandingViewModel : IDutyFreeLandingViewModel
    {
        private readonly IClientService clientService;
        private readonly IManagerService managerService;
        private readonly UserSession session;

        public bool IsRoleSelected { get; private set; }

        public string ErrorMessage { get; private set; } = string.Empty;

        public ICommand SelectAdminCommand { get; }
        public ICommand SelectClientCommand { get; }

        public DutyFreeLandingViewModel(IClientService clientService, IManagerService managerService, UserSession session)
        {
            this.clientService = clientService;
            this.managerService = managerService;
            this.session = session;

            SelectAdminCommand = new RelayCommand(SetAdmin);
            SelectClientCommand = new RelayCommand(SetClient);
        }

        private void SetAdmin()
        {
            try
            {
                var manager = Task.Run(() => managerService.GetAnyManagerAsync()).GetAwaiter().GetResult();
                if (manager == null)
                {
                    ErrorMessage = "No admin found.";
                    return;
                }

                session.SetAdmin(manager.Id);
                IsRoleSelected = true;
            }
            catch (Exception)
            {
                ErrorMessage = "No admin found.";
            }
        }

        private void SetClient()
        {
            try
            {
                var client = Task.Run(() => clientService.GetAnyClientAsync()).GetAwaiter().GetResult();
                session.SetClient(client.Id);
                IsRoleSelected = true;
            }
            catch (Exception)
            {
                ErrorMessage = "No client found.";
                //ErrorMessage = $"Debug Error: {ex.Message} | Stack: {ex.StackTrace}";
            }
        }
    }
}
