using Microsoft.EntityFrameworkCore;
using nwsdb_API.Model;
namespace nwsdb_API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<WaterAccount> Accounts => Set<WaterAccount>();
    public DbSet<MeterReading> Readings => Set<MeterReading>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<PaymentAttempt> Attempts => Set<PaymentAttempt>();
    public DbSet<Payment> Payments => Set<Payment>();
    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<AppUser>().HasKey(x => x.UserId);
        m.Entity<WaterAccount>().HasKey(x => x.AccountId);
        m.Entity<MeterReading>().HasKey(x => x.ReadingId);
        m.Entity<PaymentAttempt>().HasKey(x => x.AttemptId);
        m.Entity<AppUser>().Property(x => x.Role).HasConversion<string>();
        m.Entity<PaymentAttempt>().Property(x => x.Status).HasConversion<string>();
        m.Entity<AppUser>().HasIndex(x => x.Username).IsUnique();
        m.Entity<AppUser>().HasIndex(x => x.CustomerId).IsUnique().HasFilter("[CustomerId] IS NOT NULL");
        m.Entity<AppUser>().ToTable(t => t.HasCheckConstraint("CK_User_Role", "([Role]='Customer' AND [CustomerId] IS NOT NULL AND [PartnerId] IS NULL) OR ([Role]='PartnerCashier' AND [PartnerId] IS NOT NULL AND [CustomerId] IS NULL) OR ([Role]='Staff' AND [CustomerId] IS NULL AND [PartnerId] IS NULL)"));
        m.Entity<WaterAccount>().HasIndex(x => x.AccountNumber).IsUnique();
        m.Entity<WaterAccount>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
        m.Entity<MeterReading>().HasOne<WaterAccount>().WithMany(x => x.Readings).HasForeignKey(x => x.AccountId);
        m.Entity<MeterReading>().HasIndex(x => new { x.AccountId, x.ReadingDate }).IsUnique();
        m.Entity<Bill>().HasOne(x => x.Account).WithMany(x => x.Bills).HasForeignKey(x => x.AccountId);
        m.Entity<Bill>().HasOne<MeterReading>().WithMany().HasForeignKey(x => x.StartReadingId);
        m.Entity<Bill>().HasOne<MeterReading>().WithMany().HasForeignKey(x => x.EndReadingId);
        m.Entity<Bill>().HasIndex(x => x.StartReadingId).IsUnique();
        m.Entity<Bill>().HasIndex(x => x.EndReadingId).IsUnique();
        m.Entity<PaymentAttempt>().HasOne(x => x.Bill).WithMany(x => x.Attempts).HasForeignKey(x => x.BillId);
        m.Entity<PaymentAttempt>().HasOne<Partner>().WithMany().HasForeignKey(x => x.PartnerId);
        m.Entity<PaymentAttempt>().HasIndex(x => new { x.PartnerId, x.ClientReference }).IsUnique();
        m.Entity<PaymentAttempt>().HasIndex(x => x.BillId).IsUnique().HasFilter("[Status] = 'Pending'");
        m.Entity<Payment>().HasOne(x => x.Bill).WithOne(x => x.Payment).HasForeignKey<Payment>(x => x.BillId);
        m.Entity<PaymentAttempt>().HasOne(x => x.Payment).WithOne().HasForeignKey<Payment>(x => x.AttemptId);
        m.Entity<Payment>().HasIndex(x => x.ReceiptNumber).IsUnique();
        m.Entity<Payment>().HasIndex(x => x.BankReference).IsUnique();
        foreach (var entity in m.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                if (property.ClrType == typeof(decimal)) { property.SetPrecision(18); property.SetScale(2); }
                if (property.ClrType == typeof(string)) property.SetMaxLength(200);
            }
            foreach (var fk in entity.GetForeignKeys()) fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
        m.Entity<AppUser>().Property(x => x.PasswordHash).HasMaxLength(500);
    }
}
