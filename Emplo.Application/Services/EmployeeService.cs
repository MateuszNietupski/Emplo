using Emplo.Core.Entities;
using Emplo.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Emplo.Application.Services;

public class EmployeeService(ILogger<EmployeeService> logger) : IEmployeeService
{
    private List<EmployeeStructure> _employeeStructure = new();
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

    public List<EmployeeStructure> FillEmployeesStrucutre(List<Employee> employees)
    {
        _employeeStructure.Clear();
        var employeeMap = employees.ToDictionary(e => e.Id, e => e);

        foreach (var employee in employees)
        {
            if (!employee.SuperiorId.HasValue)
            {
                logger.LogWarning("Employee {EmployeeId} has no superior", employee.Id);
                continue;           
            }
            
            var visited = new HashSet<int>();
            var currentSuperiorId = employee.SuperiorId;
            var rank = 1;
            
            while (currentSuperiorId.HasValue)
            {
                if (visited.Contains(currentSuperiorId.Value))
                {
                    logger.LogWarning("Employee {EmployeeId} has circular reference", employee.Id);
                    break;
                }
                visited.Add(currentSuperiorId.Value);
                _employeeStructure.Add(new EmployeeStructure
                {
                    EmployeeId = employee.Id,
                    SuperiorId = currentSuperiorId.Value,
                    Rank = rank
                });
                if (employeeMap.TryGetValue(currentSuperiorId.Value, out var superior))
                {
                    rank++;
                    currentSuperiorId = superior.SuperiorId; 
                }
                else
                {
                    logger.LogWarning("Superior {SuperiorId} not found in list for employee {EmployeeId}",
                        currentSuperiorId.Value, employee.Id );
                    break;           
                }
            }
        }
        return _employeeStructure;
    }

    public int? GetSuperiorRowOfEmployee(int employeeId, int superiorId)
    {
        return _employeeStructure.FirstOrDefault(
            e => e.EmployeeId == employeeId && e.SuperiorId == superiorId)?.Rank;       
    }
}