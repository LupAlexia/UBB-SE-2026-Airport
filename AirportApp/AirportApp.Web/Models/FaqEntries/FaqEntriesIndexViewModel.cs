using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.FaqEntries
{
    public class FaqEntriesIndexViewModel
    {
        public List<FAQEntry> PopularFAQs { get; set; } = new List<FAQEntry>();
        public List<FAQEntry> FilteredFAQs { get; set; } = new List<FAQEntry>();
        public FAQCategoryEnum SelectedCategory { get; set; } = FAQCategoryEnum.All;
        public string? SearchQuery { get; set; }
    }
}

