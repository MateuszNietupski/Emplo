namespace Emplo.Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? SuperiorId { get; set; }
    public virtual Employee Superior { get; set; }
    public int TeamId { get; set; }
    public virtual Team Team { get; set; }
    public int VacationPackageId { get; set; }
    public virtual VacationPackage VacationPackage { get; set; }
    public virtual ICollection<Vacation> Vacations { get; set; } = new List<Vacation>();
}