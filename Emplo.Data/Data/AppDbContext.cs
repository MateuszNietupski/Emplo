using Microsoft.EntityFrameworkCore;

namespace Emplot.Data.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
}