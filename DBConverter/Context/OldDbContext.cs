using DBConverter.Migration;
using Microsoft.EntityFrameworkCore;

namespace DBConverter.Context;

public class OldDbContext : DbContext
{
    public readonly string m_DatabasePathName = "dbTache.db";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public OldDbContext(string databasePathName) => m_DatabasePathName = databasePathName;

    public OldDbContext(DbContextOptions<OldDbContext> options) : base(options) { }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public virtual DbSet<ListTache> JobList { get; set; }

    public virtual DbSet<TacheSelectionnee> SelectedJobs { get; set; }

    public virtual DbSet<Tache> Tasks { get; set; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={m_DatabasePathName}");
}