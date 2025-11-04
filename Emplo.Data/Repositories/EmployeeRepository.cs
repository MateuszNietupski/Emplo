using System.Net.Http.Headers;
using Emplo.Core.DTOs.Responses;
using Emplo.Core.Entities;
using Emplo.Core.Interfaces;
using Emplot.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace Emplot.Data.Repositories;

public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
{
    public async Task<List<Employee>> GetAllTeamEmployeesWithVacationInYearAsync(string teamName, int year)
    {
        return await context.Employees
            .Include(e => e.Team)
            .Where(e => e.Team.Name.Equals(teamName)
                && e.Vacations.Any(v => v.DateSince.Year == year))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<EmployeeVacationUsageDto>> GetAllEmployeesVacationUsageInCurrentYearAsync()
    {
        var currentYear = DateTime.UtcNow.Year;
        var today = DateTime.UtcNow;
        
        return await context.Employees
            .Select( e => new EmployeeVacationUsageDto
            {
                Id = e.Id,
                Name = e.Name,
                UsedDays = e.Vacations
                    .Where(v => v.DateSince.Year == currentYear && v.DateUntil < today)
                    .Sum(v => v.IsPartialVacation == 1 
                    ? (double)v.NumberOfHours / 8
                    : (v.DateUntil - v.DateSince).Days)
            })
            .AsNoTracking()
            .ToListAsync(); 
    }

    public async Task<List<Team>> GetTeamsWithoutVacationsInYearAsync(int year)
    {
        return await context.Teams
            .Where(t => !t.Employees.Any(e => e.Vacations.Any(v => v.DateSince.Year == year)))
            .AsNoTracking()
            .ToListAsync();
    }
}