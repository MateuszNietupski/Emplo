namespace Emplo.Core.Entities;

public class VacationPackage
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int GrantedDays { get; set; }
    public int Year { get; set; }
    
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}