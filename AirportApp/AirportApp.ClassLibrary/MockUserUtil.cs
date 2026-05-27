namespace AirportLib.Domain.User;

public enum AirportModuleRole
{
    None = 0,
    CompanyRepresentative = 1,
    AirportAdministrator = 2,
    AirportStaffMember = 3
}

public enum DutyFreeModuleRole
{
    Client = 0,
    Manager = 1
}

public sealed record MockUserRoleContext(
    int UserId,
    AirportModuleRole AirportRole,
    int? CompanyId,
    int? EmployeeId,
    DutyFreeModuleRole DutyFreeRole,
    int DutyFreeUserId);

public class MockUserUtility
{
    private const int DefaultEmployeeId = 1;

    public MockUserRoleContext GetRolesForUser(int userId)
    {
        return userId switch
        {
            1 => new MockUserRoleContext(userId, AirportModuleRole.AirportStaffMember, null, 1, DutyFreeModuleRole.Client, 1),
            2 => new MockUserRoleContext(userId, AirportModuleRole.CompanyRepresentative, 1, null, DutyFreeModuleRole.Manager, 1),
            3 => new MockUserRoleContext(userId, AirportModuleRole.AirportAdministrator, null, null, DutyFreeModuleRole.Client, 1),
            4 => new MockUserRoleContext(userId, AirportModuleRole.AirportStaffMember, null, 4, DutyFreeModuleRole.Client, 1),
            5 => new MockUserRoleContext(userId, AirportModuleRole.None, null, null, DutyFreeModuleRole.Client, 1),
            _ => new MockUserRoleContext(userId, AirportModuleRole.AirportStaffMember, null, DefaultEmployeeId, DutyFreeModuleRole.Client, 1)
        };
    }
}