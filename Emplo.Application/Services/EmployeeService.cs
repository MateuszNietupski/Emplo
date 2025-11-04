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
            var yearStart = new DateTime(vacationPackage.Year, 1, 1);
            var yearEnd = new DateTime(vacationPackage.Year, 12, 31);

            var usedDays = vacations
                .Where(v => v.EmployeeId == employee.Id &&
                            v.DateUntil >= yearStart &&
                            v.DateSince <= yearEnd)
                .Sum(v =>
                {
                    if (v.IsPartialVacation == 1)
                        return (double)v.NumberOfHours / 8;
                    var start = v.DateSince < yearStart ? yearStart : v.DateSince;
                    var end = v.DateUntil > yearEnd ? yearEnd : v.DateUntil;
                    return (end - start).Days + 1;
                });
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

    public bool IfEmployeeCanRequestVacation(
        Employee employee,
        List<Vacation> vacations,
        VacationPackage vacationPackage,
        DateTime? requestedStart = null,
        DateTime? requestedEnd = null)
    {
        var freeDays = CountFreeDaysForEmployee(employee, vacations, vacationPackage);

        if (freeDays <= 0)
        {
            logger.LogWarning("Employee {EmployeeId} has no remaining free days", employee.Id);
            return false;
        }

        if (requestedStart.HasValue && requestedEnd.HasValue)
        {
            var overlap = vacations.Any(v =>
                v.EmployeeId == employee.Id &&
                v.DateSince <= requestedEnd &&
                v.DateUntil >= requestedStart);
            
            if (overlap)
            {
                logger.LogWarning("Employee {EmployeeId} has overlapping vacations", employee.Id);
                return false;           
            }
            
            var requestedDays = (requestedEnd - requestedStart).Value.Days;
            
            if (requestedDays > freeDays)
            {
                logger.LogWarning("Employee {EmployeeId} has requested more days ({RequestedDays}) than free days ({FreeDays})",
                    employee.Id, requestedDays, freeDays);
                return false;           
            }     
        }
        return true;
    }
}