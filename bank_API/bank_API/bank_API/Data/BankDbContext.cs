using bank_API.Model;
using Microsoft.EntityFrameworkCore;
namespace bank_API.Data;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
{
    public DbSet<BankTransaction> Transactions => Set<BankTransaction>();
    protected override void OnModelCreating(ModelBuilder model)
    {
        var t = model.Entity<BankTransaction>();
        t.HasKey(x => x.AttemptId);
        t.Property(x => x.Amount).HasPrecision(18, 2);
        t.Property(x => x.ServiceClientId).HasMaxLength(80);
        t.Property(x => x.Currency).HasMaxLength(3);
        t.Property(x => x.MockPaymentToken).HasMaxLength(30);
        t.Property(x => x.Outcome).HasConversion<string>().HasMaxLength(20);
        t.Property(x => x.BankReference).HasMaxLength(50);
        t.HasIndex(x => x.BankReference).IsUnique();
    }
}
