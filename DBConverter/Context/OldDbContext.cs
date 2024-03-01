using DBConverter.Migration;
using Microsoft.EntityFrameworkCore;

namespace DBConverter.Context;

public partial class OldDbContext : DbContext
{
    public readonly string m_DatabasePathName = "dbTache.db";

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public OldDbContext(string p_DatabasePathName) => m_DatabasePathName = p_DatabasePathName;

    public OldDbContext(DbContextOptions<OldDbContext> p_Options) : base(p_Options) { }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    public virtual DbSet<ListTache> JobList { get; set; }

    public virtual DbSet<TacheSelectionnee> SelectedJobs { get; set; }

    public virtual DbSet<tache> Tasks { get; set; }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    protected override void OnConfiguring(DbContextOptionsBuilder p_OptionsBuilder)
        => p_OptionsBuilder.UseSqlite($"Data Source={m_DatabasePathName}");

    protected override void OnModelCreating(ModelBuilder p_ModelBuilder)
        => OnModelCreatingPartial(p_ModelBuilder);

    partial void OnModelCreatingPartial(ModelBuilder p_ModelBuilder);
}
