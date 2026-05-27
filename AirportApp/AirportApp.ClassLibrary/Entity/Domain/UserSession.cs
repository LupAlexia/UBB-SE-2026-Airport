using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [NotMapped]
    public static class UserSession
    {
        public static Customer? CurrentUser { get; set; }

        // These are from 921's UserSession class
        // The new UserSession class i think will be modified to fit the merge when the authentification part is done.

        //public int UserId { get; private set; }

        //public bool IsAdmin { get; private set; }

        //public void SetAdmin(int managerId)
        //{
        //    UserId = managerId;
        //    IsAdmin = true;
        //}

        //public void SetClient(int clientId)
        //{
        //    UserId = clientId;
        //    IsAdmin = false;
        //}

#pragma warning disable SA1011
        public static object[]? PendingBookingParameters { get; set; }
#pragma warning restore SA1011
    }
}
