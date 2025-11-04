using Emplo.Application.Services;
using Emplo.Core.Entities;
using Emplo.Tests.Common;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Emplo.Tests.Services;

public class EmployeeServiceTest : TestBase
{
    private EmployeeService _service;
    private Employee _employee;
    private VacationPackage _vacationPackage;
    
    [SetUp]
    public void Setup()
    {
        var logger = NullLogger<EmployeeService>.Instance;
        _service = new EmployeeService(logger);
        _employee = new Employee {Id = 1, Name = "Jan Nowak"};
        _vacationPackage = new VacationPackage{GrantedDays = 26, Year = 2025};
    }

    #region CountFreeDaysForEmployeeClass
    
    [Test]
    public void CountFreeDaysForEmployee_WhenNoVacations()
    {
        var vacations = new List<Vacation>();
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(26));
    }

    [Test]
    public void CountFreeDaysForEmployee_WhenHasNonPartialVacation()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = new DateTime(2025, 1, 1),
                DateUntil = new DateTime(2025, 1, 6),
                IsPartialVacation = 0,
                NumberOfHours = 40
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(20));
    }
    
    [Test]
    public void CountFreeDaysForEmployee_WhenHasPartialVacation()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = new DateTime(2025, 1, 1),
                DateUntil = new DateTime(2025, 1, 2),
                IsPartialVacation = 1,
                NumberOfHours = 14
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(24));
    }
    
    [Test]
    public void CountFreeDaysForEmployee_WhenVacationInFuture()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = DateTime.UtcNow.AddDays(10),
                DateUntil = DateTime.UtcNow.AddDays(15),
                IsPartialVacation = 0,
                NumberOfHours = 40
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(20));
    }
    [Test]
    public void CountFreeDaysForEmployee_WhenVacationInPast()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = DateTime.UtcNow.AddDays(-28),
                DateUntil = DateTime.UtcNow.AddDays(-1)
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(0));
    }
    
    [Test]
    public void CountFreeDaysForVacationCrossingYearBoundary()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = new DateTime(2024, 12, 29),
                DateUntil = new DateTime(2025, 1, 3),
                IsPartialVacation = 0,
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(23));
    }
    [Test]
    public void CountFreeDaysForVacationCrossingEndYearBoundary()
    {
        var vacations = new List<Vacation>
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = new DateTime(2025, 12, 29),
                DateUntil = new DateTime(2026, 1, 2),
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(23));
    }
    #endregion

    #region IfEmployeeCanRequestVacationClass

    [Test]
    public void Employee_Can_Request_Vacation()
    {
        var vacations = new List<Vacation>();
        
        var result = _service.IfEmployeeCanRequestVacation(_employee, vacations, _vacationPackage, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        Assert.That(result, Is.True);
    }
    
    [Test]
    public void Employee_Cant_Request_Vacation()
    {
        var vacations = new List<Vacation>()
        {
            new Vacation
            {
                EmployeeId = 1,
                DateSince = DateTime.UtcNow.AddDays(1),
                DateUntil = DateTime.UtcNow.AddDays(5),
                IsPartialVacation = 0,
                NumberOfHours = 40
            }
        };
        
        var result = _service.IfEmployeeCanRequestVacation(_employee, vacations, _vacationPackage, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));
        Assert.That(result, Is.False);
    }

    #endregion
}