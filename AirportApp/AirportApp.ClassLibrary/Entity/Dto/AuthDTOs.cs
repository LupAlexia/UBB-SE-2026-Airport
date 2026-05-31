namespace AirportApp.ClassLibrary.Entity.Dto
{
    public class LoginRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? CurrentUserId { get; set; }
    }

    public class RegisterRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Unified login request for all roles: customer (email+password), or
    /// user / admin / employee / manager / client (ID-based).
    /// </summary>
    public class UnifiedLoginRequestDTO
    {
        /// <summary>One of: customer, user, admin, employee, manager, client.</summary>
        public string Role { get; set; } = string.Empty;

        // Customer login fields
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? CurrentUserId { get; set; }

        // ID-based login field (user, admin, employee, manager, client)
        public int? Id { get; set; }
    }

    public class LoginResponseDTO
    {
        public int Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
