using ContactProvider.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactProvider.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<ContactRequestEntity> ContactRequests { get; set; }
}
