namespace AirportLib.Domain.User
{
    public class UserSession
    {
        public int UserId { get; private set; }

        public bool IsAdmin { get; private set; }

        public void SetAdmin(int managerId)
        {
            UserId = managerId;
            IsAdmin = true;
        }

        public void SetClient(int clientId)
        {
            UserId = clientId;
            IsAdmin = false;
        }
    }
}
