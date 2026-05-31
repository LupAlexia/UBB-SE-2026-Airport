using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [NotMapped]
    public static class UserSession
    {
        // Legacy customer session — kept for backward compatibility with auth service / proxy code.
        public static Customer? CurrentUser { get; set; }

        // Unified multi-role session state.
        public static int? CurrentUserId { get; private set; }
        public static string? CurrentUserRole { get; private set; }
        public static string? CurrentUserName { get; private set; }
        public static string? CurrentUserEmail { get; private set; }

        public static bool IsLoggedIn => CurrentUserId.HasValue;
        public static bool IsCustomer => string.Equals(CurrentUserRole, "customer", StringComparison.OrdinalIgnoreCase);
        public static bool IsAdmin => string.Equals(CurrentUserRole, "admin", StringComparison.OrdinalIgnoreCase);
        public static bool IsEmployee => string.Equals(CurrentUserRole, "employee", StringComparison.OrdinalIgnoreCase);
        public static bool IsManager => string.Equals(CurrentUserRole, "manager", StringComparison.OrdinalIgnoreCase);
        public static bool IsUser => string.Equals(CurrentUserRole, "user", StringComparison.OrdinalIgnoreCase);
        public static bool IsClient => string.Equals(CurrentUserRole, "client", StringComparison.OrdinalIgnoreCase);

        public static void SetCurrentUser(int id, string role, string name, string email = "", Customer? customer = null)
        {
            CurrentUserId = id;
            CurrentUserRole = role.ToLowerInvariant();
            CurrentUserName = name;
            CurrentUserEmail = email;
            CurrentUser = customer;
        }

        public static void Clear()
        {
            CurrentUser = null;
            CurrentUserId = null;
            CurrentUserRole = null;
            CurrentUserName = null;
            CurrentUserEmail = null;
            PendingBookingParameters = null;
        }

#pragma warning disable SA1011
        public static object[]? PendingBookingParameters { get; set; }
#pragma warning restore SA1011
    }
}
