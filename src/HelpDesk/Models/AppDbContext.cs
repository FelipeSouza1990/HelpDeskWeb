using HelpDesk.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace HelpDesk.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<FixLog> FixLogs => Set<FixLog>();
}