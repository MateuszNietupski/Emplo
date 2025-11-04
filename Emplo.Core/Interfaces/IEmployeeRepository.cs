using Emplo.Core.DTOs.Responses;
using Emplo.Core.Entities;

namespace Emplo.Core.Interfaces;

public interface IEmployeeRepository
{
    Task <List<Employee>> GetAllTeamEmployeesWithVacationInYearAsync (string teamName, int year);
    Task<List<EmployeeVacationUsageDto>> GetAllEmployeesVacationUsageInCurrentYearAsync();
    Task<List<Team>> GetTeamsWithoutVacationsInYearAsync(int year);
}