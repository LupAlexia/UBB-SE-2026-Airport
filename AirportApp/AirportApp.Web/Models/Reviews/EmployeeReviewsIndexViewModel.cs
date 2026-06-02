using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Dto;

namespace AirportApp.Web.Models.Reviews
{
    public class EmployeeReviewsIndexViewModel
    {
        public int TotalReviews { get; set; }

        public double AverageOverallRating { get; set; }

        public double AverageDutyFree { get; set; }

        public double AverageFlightExperience { get; set; }

        public double AverageStaffFriendliness { get; set; }

        public double AverageCleanliness { get; set; }

        public List<ReviewDTO> Reviews { get; set; } = new List<ReviewDTO>();
    }
}

