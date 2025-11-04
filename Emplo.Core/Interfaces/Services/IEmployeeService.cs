using Emplo.Core.Entities;

namespace Emplo.Core.Interfaces.Services;

public interface IEmployeeService
{
    public int CountFreeDaysForEmployee(Employee employee, List<Vacation> vacations, VacationPackage vacationPackage);
    public bool IfEmployeeCanRequestVacation(Employee employee,List<Vacation> vacations, VacationPackage vacationPackage,
        DateTime? requestedStart = null, DateTime? requestedEnd = null);
}