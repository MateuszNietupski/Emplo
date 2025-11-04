namespace Emplo.Core.DTOs.Responses;

public class EmployeeVacationUsageDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public double UsedDays { get; set; }
}