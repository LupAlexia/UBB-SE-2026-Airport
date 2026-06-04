using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.Src.ViewModel
{
    public class FAQAddEditNavigationArgs
    {
        public FAQViewModel ViewModel { get; init; } = null!;
        public int CurrentPersonId { get; init; }
        public bool IsAdmin { get; init; }
        public FAQEntryDTO? FAQEntry { get; init; }
    }
}
