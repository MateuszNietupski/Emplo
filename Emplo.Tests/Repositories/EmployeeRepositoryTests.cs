using Emplo.Core.Entities;
using Emplo.Tests.Common;
using Emplot.Data.Repositories;
using NUnit.Framework;

namespace Emplo.Tests.Repositories;

[TestFixture]
public class EmployeeRepositoryTests : TestBase
{
    private EmployeeRepository _repository;

    [SetUp]
    public void Setup()
    {
        _repository = new EmployeeRepository(Context);
        SeedData();
    }
    
    [Test]
    public void GetAllTeamEmployeesWithVacationInYearAsync_ShouldReturnAllEmployeesWithVacationIn2019r()
    {
        var result = _repository.GetAllTeamEmployeesWithVacationInYearAsync(".NET",2019).Result;
        
        Assert.That(result, Is.Not.Null);
        Assert.That(1, Is.EqualTo(result.Count));
        Assert.That(result.All(e => e.Team.Name.Equals(".NET")));
    }
    
    [Test]
    public void GetAllEmployeesVacationUsageInCurrentYearAsync_ShouldReturnAllEmployeesVacationUsageInCurrentYear()
    {
        var result = _repository.GetAllEmployeesVacationUsageInCurrentYearAsync().Result;
        
        Assert.That(result, Is.Not.Null);
        Assert.That(6, Is.EqualTo(result.Count));
        Assert.That(2, Is.EqualTo(result.Count(e => e.UsedDays > 0)));
    }
    [Test]
    public void GetTeamsWithoutVacationsInYearAsync_ShouldReturnAllTeamsWithoutVacationsIn2019r()
    {
        var result = _repository.GetTeamsWithoutVacationsInYearAsync(2019).Result;
        
        Assert.That(result, Is.Not.Null);
        Assert.That(1, Is.EqualTo(result.Count));
        
    }
    
    
    
    
    private void SeedData()
    {
        var teamDotNet = new Team { Id = 1, Name = ".NET" };
        var teamJava = new Team { Id = 2, Name = "Java" };
        var teamPython = new Team { Id = 3, Name = "Python" };
        
        var packageStandard = new VacationPackage { Id = 1, Name = "Standard", GrantedDays = 26, Year = 2025 };
        var packageExtended = new VacationPackage { Id = 2, Name = "Extended", GrantedDays = 30, Year = 2025 };
        
        
        var employees = new List<Employee>
        {
            new Employee { Id = 1, Name = "Jan Nowak", TeamId = 1, VacationPackageId = 1 },
            new Employee { Id = 2, Name = "Anna Kowalska", TeamId = 1, VacationPackageId = 1 },
            new Employee { Id = 3, Name = "Adam Java", TeamId = 2, VacationPackageId = 2 },
            new Employee { Id = 4, Name = "Ewa Python", TeamId = 3, VacationPackageId = 1 },
            new Employee { Id = 5, Name = "Piotr Nowak", TeamId = 2, VacationPackageId = 2 },
            new Employee { Id = 6, Name = "Kasia Kowalska", TeamId = 1, VacationPackageId = 1 } 
        };

        var vacations = new List<Vacation>
        {
            // .NET
            new Vacation { Id = 1, EmployeeId = 1, DateSince = new DateTime(2019, 6, 1), DateUntil = new DateTime(2019, 6, 5), NumberOfHours = 40, IsPartialVacation = 0 },
            new Vacation { Id = 2, EmployeeId = 2, DateSince = new DateTime(2025, 7, 10), DateUntil = new DateTime(2025, 7, 12), NumberOfHours = 14, IsPartialVacation = 1 },
            
            
            new Vacation { Id = 3, EmployeeId = 3, DateSince = new DateTime(2025, 3, 15), DateUntil = new DateTime(2025, 3, 20), NumberOfHours = 40, IsPartialVacation = 0 },
            new Vacation { Id = 4, EmployeeId = 5, DateSince = new DateTime(2019, 8, 5), DateUntil = new DateTime(2019, 8, 10), NumberOfHours = 40, IsPartialVacation = 0 },
        };

        Context.Teams.AddRange(teamDotNet, teamJava,teamPython);
        Context.VacationPackages.AddRange(packageStandard, packageExtended);
        Context.Employees.AddRange(employees);
        Context.Vacations.AddRange(vacations);
        Context.SaveChanges();
    }
}