using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interface;
using AirportApp.ClassLibrary.Service;
using AirportApp.ClassLibrary.Service.Interface;
using NSubstitute;

namespace AirportApp.Tests.Unit_Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
    private const int    ValidEmployeeId = 5;
    private const string ValidName       = "John Doe";
    private const int    ValidSalary     = 3000;

    private static readonly DateOnly ValidBirthday   = new DateOnly(1990, 1, 1);
    private static readonly DateOnly ValidHiringDate = new DateOnly(2020, 6, 15);

    private static Employee MakeEmployee(int id = ValidEmployeeId, string name = ValidName) =>
        new Employee(name, EmployeeRoleEnum.Pilot)
        {
            Id         = id,
            Salary     = ValidSalary,
            Birthday   = ValidBirthday,
            HiringDate = ValidHiringDate
        };

    private static (EmployeeService service, IEmployeeRepository repo, IEmployeeFlightService flights)
        MakeService(Employee? repoEmployee = null)
    {
        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        if (repoEmployee != null)
            repo.GetByIdAsync(repoEmployee.Id).Returns(Task.FromResult<Employee?>(repoEmployee));
        return (new EmployeeService(repo, flights), repo, flights);
    }

    [Test]
    public async Task GetEmployeeByIdAsync_ZeroId_ReturnsNull()
    {
        var (service, _, _) = MakeService();
        var result = await service.GetEmployeeByIdAsync(0);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetEmployeeByIdAsync_NegativeId_ReturnsNull()
    {
        var (service, _, _) = MakeService();
        var result = await service.GetEmployeeByIdAsync(-1);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetEmployeeByIdAsync_PositiveIdWithExistingEmployee_ReturnsEmployee()
    {
        var employee        = MakeEmployee();
        var (service, _, _) = MakeService(employee);

        var result = await service.GetEmployeeByIdAsync(ValidEmployeeId);

        Assert.That(result, Is.SameAs(employee));
    }

    [Test]
    public async Task GetEmployeesByRoleAsync_MixedRoles_ReturnsOnlyMatchingRole()
    {
        var pilot     = MakeEmployee(1, "Pilot Pete");
        var attendant = new Employee("Jane", EmployeeRoleEnum.FlightAttendant)
            { Id = 2, Salary = 2000, Birthday = ValidBirthday, HiringDate = ValidHiringDate };

        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(
            new List<Employee> { pilot, attendant }));

        var service = new EmployeeService(repo, flights);
        var result  = (await service.GetPilotsAsync()).ToList();

        Assert.That(result.Count,    Is.EqualTo(1));
        Assert.That(result.Single(), Is.SameAs(pilot));
    }

    [Test]
    public async Task GetEmployeesByRoleAsync_NoMatchingRole_ReturnsEmptyCollection()
    {
        var attendant = new Employee("Jane", EmployeeRoleEnum.FlightAttendant)
            { Id = 2, Salary = 2000, Birthday = ValidBirthday, HiringDate = ValidHiringDate };

        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(
            new List<Employee> { attendant }));

        var result = (await new EmployeeService(repo, flights).GetCoPilotsAsync()).ToList();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetFlightAttendantsAsync_SeveralRoles_ReturnsOnlyFlightAttendants()
    {
        var pilot      = MakeEmployee(1, "Pilot A");
        var attendant1 = new Employee("FA-1", EmployeeRoleEnum.FlightAttendant)
            { Id = 3, Salary = 2000, Birthday = ValidBirthday, HiringDate = ValidHiringDate };
        var attendant2 = new Employee("FA-2", EmployeeRoleEnum.FlightAttendant)
            { Id = 4, Salary = 2100, Birthday = ValidBirthday, HiringDate = ValidHiringDate };

        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(
            new List<Employee> { pilot, attendant1, attendant2 }));

        var result = (await new EmployeeService(repo, flights).GetFlightAttendantsAsync()).ToList();

        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task GetFlightDispatchersAsync_SeveralRoles_ReturnsOnlyDispatchers()
    {
        var dispatcher = new Employee("Disp", EmployeeRoleEnum.FlightDispatcher)
            { Id = 10, Salary = 3500, Birthday = ValidBirthday, HiringDate = ValidHiringDate };
        var pilot = MakeEmployee();

        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetAsync().Returns(Task.FromResult<IEnumerable<Employee>>(
            new List<Employee> { pilot, dispatcher }));

        var result = (await new EmployeeService(repo, flights).GetFlightDispatchersAsync()).ToList();

        Assert.That(result.Count,    Is.EqualTo(1));
        Assert.That(result.Single(), Is.SameAs(dispatcher));
    }

    [Test]
    public void AddEmployeeAsync_NullName_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddEmployeeAsync(null!, EmployeeRoleEnum.Pilot, ValidBirthday, ValidSalary, ValidHiringDate));
    }

    [Test]
    public void AddEmployeeAsync_WhitespaceName_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddEmployeeAsync("   ", EmployeeRoleEnum.Pilot, ValidBirthday, ValidSalary, ValidHiringDate));
    }

    [Test]
    public void AddEmployeeAsync_NegativeSalary_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddEmployeeAsync(ValidName, EmployeeRoleEnum.Pilot, ValidBirthday, -1, ValidHiringDate));
    }

    [Test]
    public void AddEmployeeAsync_FutureBirthday_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        var futureBirthday  = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddEmployeeAsync(ValidName, EmployeeRoleEnum.Pilot, futureBirthday, ValidSalary, ValidHiringDate));
    }

    [Test]
    public void AddEmployeeAsync_FutureHiringDate_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        var futureHiringDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.AddEmployeeAsync(ValidName, EmployeeRoleEnum.Pilot, ValidBirthday, ValidSalary, futureHiringDate));
    }

    [Test]
    public void SaveEmployeeAsync_NullEmployee_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.SaveEmployeeAsync(null!, DateTimeOffset.Now.AddYears(-20), DateTimeOffset.Now, "3000"));
    }

    [Test]
    public void SaveEmployeeAsync_NullBirthday_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.SaveEmployeeAsync(MakeEmployee(), null, DateTimeOffset.Now, "3000"));
    }

    [Test]
    public void SaveEmployeeAsync_NullHiringDate_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.SaveEmployeeAsync(MakeEmployee(), DateTimeOffset.Now.AddYears(-20), null, "3000"));
    }

    [Test]
    public void SaveEmployeeAsync_NonNumericSalaryText_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.SaveEmployeeAsync(MakeEmployee(), DateTimeOffset.Now.AddYears(-20), DateTimeOffset.Now, "abc"));
    }

    [Test]
    public void SaveEmployeeAsync_FutureBirthday_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        var futureBirthday  = DateTimeOffset.Now.AddDays(1);

        Assert.ThrowsAsync<ArgumentException>(() =>
            service.SaveEmployeeAsync(MakeEmployee(), futureBirthday, DateTimeOffset.Now.AddYears(-1), "3000"));
    }

    [Test]
    public async Task SaveEmployeeAsync_EmployeeIdIsZero_RoutesToAdd()
    {
        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.AddAsync(Arg.Any<Employee>()).Returns(Task.FromResult(1));

        var service  = new EmployeeService(repo, flights);
        var employee = MakeEmployee(id: 0);

        await service.SaveEmployeeAsync(
            employee,
            DateTimeOffset.Now.AddYears(-30),
            DateTimeOffset.Now.AddYears(-2),
            "4000");

        await repo.Received(1).AddAsync(Arg.Any<Employee>());
        await repo.DidNotReceive().UpdateAsync(Arg.Any<Employee>());
    }

    [Test]
    public async Task SaveEmployeeAsync_EmployeeIdIsNonZero_RoutesToUpdate()
    {
        var employee = MakeEmployee();
        var repo     = Substitute.For<IEmployeeRepository>();
        var flights  = Substitute.For<IEmployeeFlightService>();
        repo.GetByIdAsync(ValidEmployeeId).Returns(Task.FromResult<Employee?>(employee));

        var service = new EmployeeService(repo, flights);
        await service.SaveEmployeeAsync(
            employee,
            DateTimeOffset.Now.AddYears(-30),
            DateTimeOffset.Now.AddYears(-2),
            "4000");

        await repo.Received(1).UpdateAsync(Arg.Any<Employee>());
        await repo.DidNotReceive().AddAsync(Arg.Any<Employee>());
    }

    [Test]
    public void DeleteWithAssignmentsAsync_ZeroId_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() => service.DeleteWithAssignmentsAsync(0));
    }

    [Test]
    public void DeleteWithAssignmentsAsync_NegativeId_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() => service.DeleteWithAssignmentsAsync(-1));
    }

    [Test]
    public async Task DeleteWithAssignmentsAsync_ValidId_RemovesFlightAssignmentsBeforeDeletingEmployee()
    {
        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        flights.RemoveAllFlightsAssignmentsForEmployeeAsync(ValidEmployeeId).Returns(Task.CompletedTask);
        repo.DeleteAsync(ValidEmployeeId).Returns(Task.CompletedTask);

        var service = new EmployeeService(repo, flights);
        await service.DeleteWithAssignmentsAsync(ValidEmployeeId);

        await flights.Received(1).RemoveAllFlightsAssignmentsForEmployeeAsync(ValidEmployeeId);
        await repo.Received(1).DeleteAsync(ValidEmployeeId);
    }

    [Test]
    public async Task DeleteEmployeeUsingIdAsync_ZeroId_DoesNotCallRepository()
    {
        var (service, repo, _) = MakeService();
        await service.DeleteEmployeeUsingIdAsync(0);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public async Task DeleteEmployeeUsingIdAsync_NegativeId_DoesNotCallRepository()
    {
        var (service, repo, _) = MakeService();
        await service.DeleteEmployeeUsingIdAsync(-5);
        await repo.DidNotReceive().DeleteAsync(Arg.Any<int>());
    }

    [Test]
    public void ParseRole_NullInput_ReturnsOther()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole(null!), Is.EqualTo(EmployeeRoleEnum.Other));
    }

    [Test]
    public void ParseRole_WhitespaceInput_ReturnsOther()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole("   "), Is.EqualTo(EmployeeRoleEnum.Other));
    }

    [Test]
    public void ParseRole_RecognisedRoleName_ReturnsMappedRole()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole("Pilot"), Is.EqualTo(EmployeeRoleEnum.Pilot));
    }

    [Test]
    public void ParseRole_RoleNameWithDashAndSpace_StripsBothAndMaps()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole("Co-Pilot"), Is.EqualTo(EmployeeRoleEnum.CoPilot));
    }

    [Test]
    public void ParseRole_LowercaseRoleName_ReturnsMappedRole()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole("pilot"), Is.EqualTo(EmployeeRoleEnum.Pilot));
    }

    [Test]
    public void ParseRole_UnrecognisedRoleName_ReturnsOther()
    {
        var (service, _, _) = MakeService();
        Assert.That(service.ParseRole("Astronaut"), Is.EqualTo(EmployeeRoleEnum.Other));
    }

    [Test]
    public void LoginAsync_NonNumericIdText_ThrowsArgumentException()
    {
        var (service, _, _) = MakeService();
        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync("abc"));
    }

    [Test]
    public void LoginAsync_UnknownEmployeeId_ThrowsArgumentException()
    {
        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetByIdAsync(ValidEmployeeId).Returns(Task.FromResult<Employee?>(null));

        var service = new EmployeeService(repo, flights);
        Assert.ThrowsAsync<ArgumentException>(() => service.LoginAsync(ValidEmployeeId.ToString()));
    }

    [Test]
    public async Task LoginAsync_ExistingEmployeeId_ReturnsEmployeeId()
    {
        var employee = MakeEmployee();
        var (service, _, _) = MakeService(employee);

        var result = await service.LoginAsync(ValidEmployeeId.ToString());

        Assert.That(result, Is.EqualTo(ValidEmployeeId));
    }

    [Test]
    public async Task UpdateEmployeeAsync_EmployeeNotFound_DoesNotCallRepositoryUpdate()
    {
        var repo    = Substitute.For<IEmployeeRepository>();
        var flights = Substitute.For<IEmployeeFlightService>();
        repo.GetByIdAsync(ValidEmployeeId).Returns(Task.FromResult<Employee?>(null));

        var service = new EmployeeService(repo, flights);
        await service.UpdateEmployeeAsync(ValidEmployeeId, name: "New Name");

        await repo.DidNotReceive().UpdateAsync(Arg.Any<Employee>());
    }

    [Test]
    public async Task UpdateEmployeeAsync_OnlyNameProvided_UpdatesNameLeavesOtherFieldsUnchanged()
    {
        var employee     = MakeEmployee();
        var originalRole = employee.Role;
        var (service, repo, _) = MakeService(employee);

        await service.UpdateEmployeeAsync(ValidEmployeeId, name: "Updated Name");

        Assert.That(employee.Name, Is.EqualTo("Updated Name"));
        Assert.That(employee.Role, Is.EqualTo(originalRole));
        await repo.Received(1).UpdateAsync(employee);
    }

    [Test]
    public async Task UpdateEmployeeAsync_OnlyRoleProvided_UpdatesRoleLeavesNameUnchanged()
    {
        var employee     = MakeEmployee();
        var originalName = employee.Name;
        var (service, repo, _) = MakeService(employee);

        await service.UpdateEmployeeAsync(ValidEmployeeId, role: EmployeeRoleEnum.CoPilot);

        Assert.That(employee.Name, Is.EqualTo(originalName));
        Assert.That(employee.Role, Is.EqualTo(EmployeeRoleEnum.CoPilot));
        await repo.Received(1).UpdateAsync(employee);
    }

    [Test]
    public async Task UpdateEmployeeAsync_AllFieldsProvided_UpdatesEveryField()
    {
        var employee = MakeEmployee();
        var (service, repo, _) = MakeService(employee);

        var newBirthday   = new DateOnly(1985, 5, 20);
        var newHiringDate = new DateOnly(2019, 3, 10);

        await service.UpdateEmployeeAsync(
            ValidEmployeeId,
            name:       "New Name",
            role:       EmployeeRoleEnum.FlightAttendant,
            salary:     5000,
            birthday:   newBirthday,
            hiringDate: newHiringDate);

        Assert.That(employee.Name,       Is.EqualTo("New Name"));
        Assert.That(employee.Role,       Is.EqualTo(EmployeeRoleEnum.FlightAttendant));
        Assert.That(employee.Salary,     Is.EqualTo(5000));
        Assert.That(employee.Birthday,   Is.EqualTo(newBirthday));
        Assert.That(employee.HiringDate, Is.EqualTo(newHiringDate));
        await repo.Received(1).UpdateAsync(employee);
    }
}
