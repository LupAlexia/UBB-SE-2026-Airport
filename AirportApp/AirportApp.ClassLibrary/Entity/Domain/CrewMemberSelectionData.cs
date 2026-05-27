namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class CrewMemberSelectionData
    {
        public Employee Employee { get; set; } = new();
        public bool IsSelected { get; set; }
        public bool IsFirstInRoleGroup { get; set; }
        public string RoleHeader { get; set; } = string.Empty;
    }
}
