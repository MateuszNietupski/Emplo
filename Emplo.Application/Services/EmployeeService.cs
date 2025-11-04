using Emplo.Core.Entities;
using Emplo.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Emplo.Application.Services;

public class EmployeeService(ILogger<EmployeeService> logger) : IEmployeeService
{
    public int CountFreeDaysForEmployee(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage)
    {
        try
        {
            var today = DateTime.UtcNow;
            var usedDays = vacations
                .Where(v => v.EmployeeId == employee.Id && v.DateUntil < today)
                .Sum(v => v.IsPartialVacation == 1
                    ? (double)v.NumberOfHours / 8
                    : (v.DateUntil - v.DateSince).Days);
            var remainingDays = vacationPackage.GrantedDays - usedDays;
            if (remainingDays < 0)
            {
                logger.LogWarning("Employee {EmployeeId} has negative remaining days ({RemainingDays})", employee.Id,remainingDays);
                return 0;
            }
            return (int)Math.Floor(remainingDays);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while counting free days for employee {EmployeeId}", employee.Id);
            throw;
        }
    }
}