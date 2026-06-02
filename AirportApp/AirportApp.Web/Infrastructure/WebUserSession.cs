using AirportLib.Domain.User;

namespace AirportApp.Web.Infrastructure
{
    public class WebUserSession(IConfiguration configuration)
    {
        public int CurrentUserId => configuration.GetValue<int>("UserID");
        private MockUserRoleContext CurrentUserRoles => new MockUserUtility().GetRolesForUser(CurrentUserId);

        public int UserId => CurrentUserRoles.UserId;

        public DutyFreeModuleRole DutyFreeRole => CurrentUserRoles.DutyFreeRole;

        public int? CompanyId => CurrentUserRoles.CompanyId;

        public int? EmployeeId => CurrentUserRoles.EmployeeId;

        public int DutyFreeUserId => CurrentUserRoles.DutyFreeUserId;

        public bool IsAirportAdmin => CurrentUserRoles.AirportRole == AirportModuleRole.AirportAdministrator;

        public bool IsCompanyRepresentative => CurrentUserRoles.AirportRole == AirportModuleRole.CompanyRepresentative;

        public bool IsAirportStaffMember => CurrentUserRoles.AirportRole == AirportModuleRole.AirportStaffMember;

        public bool HasNoAirportRole => CurrentUserRoles.AirportRole == AirportModuleRole.None;

        public bool IsDutyFreeManager => CurrentUserRoles.DutyFreeRole == DutyFreeModuleRole.Manager;

        public bool IsDutyFreeClient => CurrentUserRoles.DutyFreeRole == DutyFreeModuleRole.Client;

        public AirportModuleRole AirportRole
        {
            get
            {
                string mapping = configuration[$"_userRoleMappings:{CurrentUserId}"];
                return ParseRoleFromMapping(mapping);
            }
        }

        private AirportModuleRole ParseRoleFromMapping(string? rawMappingText)
        {
            if (string.IsNullOrWhiteSpace(rawMappingText))
            {
                return AirportModuleRole.None;
            }

            string[] mappingSections = rawMappingText.Split(';', StringSplitOptions.TrimEntries);

            foreach (string section in mappingSections)
            {
                if (section.StartsWith("Airport Management:", StringComparison.OrdinalIgnoreCase))
                {
                    string roleContent = section.Replace("Airport Management:", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

                    if (roleContent.Contains("Airport Administrator"))
                    {
                        return AirportModuleRole.AirportAdministrator;
                    }

                    if (roleContent.Contains("Company Representative"))
                    {
                        return AirportModuleRole.CompanyRepresentative;
                    }

                    if (roleContent.Contains("Airport Staff Member"))
                    {
                        return AirportModuleRole.AirportStaffMember;
                    }
                }
            }

            return AirportModuleRole.None;
        }
    }
}

