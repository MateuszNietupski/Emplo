namespace Emplo.Core.Entities;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}