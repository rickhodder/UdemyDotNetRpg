using Microsoft.EntityFrameworkCore;

public class DataContext:DbContext
{
    public DataContext(DbContextOptions options):base(options)
    {
        
    }

    public DbSet<Character> Characters => Set<Character>();
}
