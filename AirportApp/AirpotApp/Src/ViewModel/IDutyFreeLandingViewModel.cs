using System;
using System.Windows.Input;

namespace AirportApp.Src.ViewModel
{
    public interface IDutyFreeLandingViewModel
    {
        bool IsRoleSelected { get; }
        string ErrorMessage { get; }
        ICommand SelectAdminCommand { get; }
        ICommand SelectClientCommand { get; }
    }
}
