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
        Assert.That(result, Is.EqualTo(21));
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
        Assert.That(result, Is.EqualTo(26));
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
                DateUntil = DateTime.UtcNow.AddDays(-1),
            }
        };
        var result = _service.CountFreeDaysForEmployee(_employee, vacations, _vacationPackage);
        Assert.That(result, Is.EqualTo(0));
    }
    
}