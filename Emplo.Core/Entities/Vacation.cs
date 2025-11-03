namespace Emplo.Core.Entities;

public class Vacation
{
    public int Id { get; set; }
    public DateTime DateSince { get; set; }
    public DateTime DateUntil { get; set; }
    public int NumberOfHours { get; set; }
    public int IsPartialVacation { get; set; }
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; }
    
}