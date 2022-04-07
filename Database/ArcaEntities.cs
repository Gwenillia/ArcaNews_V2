using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ArcaNews_V2.Database;

public partial class ArcaEntities : DbContext
{
    public virtual DbSet<EightBallAnswer> EightBallAnswer { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder {DataSource = "arca.db"};
        var connectionString = connectionStringBuilder.ToString();
        var connection = new SqliteConnection(connectionString);
        optionsBuilder.UseSqlite(connection);
    }
}