using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AirportApp.ClassLibrary.Entity.Dto;
using CommunityToolkit.Mvvm.ComponentModel;
using AirportApp.ClassLibrary.Service.Interface;

namespace AirportApp.Src.ViewModel
{
    public partial class LandingViewModel : ObservableObject
    {
        private readonly IReviewService reviewService;
        private readonly IMapper mapper;
        private readonly SemaphoreSlim loadReviewsSemaphore = new (1, 1);

        public ObservableCollection<ReviewDTO> Reviews { get; } = new ();

        public LandingViewModel(IReviewService reviewService, IMapper mapper)
        {
            this.reviewService = reviewService;
            this.mapper = mapper;
        }

        public async Task LoadReviewsAsync()
        {
            await loadReviewsSemaphore.WaitAsync();

            try
            {
                var allReviews = (await reviewService.GetAllAsync())?
                    .OrderByDescending(review => review.Id)
                    .ToList();
                var loadedReviews = new List<ReviewDTO>();

                if (allReviews != null)
                {
                    foreach (var review in allReviews)
                    {
                        string realName = review.User.RetrieveConfiguredDisplayFullNameForBot();

                        float averageRating = await reviewService.CalculateAverageRatingAsync(review);

                        var reviewDateTime = mapper.Map<ReviewDTO>(review);

                        var finalDateTime = reviewDateTime with
                        {
                            userName = realName,
                            overallRating = averageRating
                        };

                        loadedReviews.Add(finalDateTime);
                    }
                }

                Reviews.Clear();
                foreach (var review in loadedReviews)
                {
                    Reviews.Add(review);
                }
            }
            finally
            {
                loadReviewsSemaphore.Release();
            }
        }
    }
}
