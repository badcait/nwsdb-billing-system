using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nwsdb_API.Model;
namespace nwsdb_API.Data;
public static class DemoData
{
    public static async Task SeedAsync(AppDbContext db, string password)
    {
        if (await db.Users.AnyAsync()) return;
        await using var tx = await db.Database.BeginTransactionAsync();
        var customer = new Customer { FullName = "Nimal Perera", Email = "nimal@example.com", Phone = "0770000001" };
        var other = new Customer { FullName = "Amali Silva", Email = "amali@example.com", Phone = "0770000002" };
        var partner = new Partner { Name = "City Collection Centre", IsActive = true };
        var partner2 = new Partner { Name = "Lake Collection Centre", IsActive = true };
        db.AddRange(customer, other, partner, partner2); await db.SaveChangesAsync();
        var a = new WaterAccount { CustomerId = customer.CustomerId, AccountNumber = "W-1001", ServiceAddress = "12 Lake Road", IsActive = true };
        var a2 = new WaterAccount { CustomerId = other.CustomerId, AccountNumber = "W-1002", ServiceAddress = "45 Park Road", IsActive = true };
        db.AddRange(a, a2); await db.SaveChangesAsync();
        foreach (var account in new[] { a, a2 })
        {
            var start = new MeterReading { AccountId = account.AccountId, ReadingDate = DateTime.UtcNow.Date.AddDays(-60), MeterValue = 100 };
            var end = new MeterReading { AccountId = account.AccountId, ReadingDate = DateTime.UtcNow.Date.AddDays(-30), MeterValue = 112 };
            db.AddRange(start, end); await db.SaveChangesAsync();
            db.Bills.Add(new Bill { AccountId = account.AccountId, StartReadingId = start.ReadingId, EndReadingId = end.ReadingId,
                PeriodStart = start.ReadingDate, PeriodEnd = end.ReadingDate, DueDate = DateTime.UtcNow.Date.AddDays(14), Units = 12, UnitRate = 50, FixedCharge = 100, Amount = 700 });
        }
        var users = new[] { new AppUser { Username = "staff", Role = UserRole.Staff },
            new AppUser { Username = "customer", Role = UserRole.Customer, CustomerId = customer.CustomerId },
            new AppUser { Username = "customer2", Role = UserRole.Customer, CustomerId = other.CustomerId },
            new AppUser { Username = "cashier", Role = UserRole.PartnerCashier, PartnerId = partner.PartnerId },
            new AppUser { Username = "cashier2", Role = UserRole.PartnerCashier, PartnerId = partner2.PartnerId } };
        foreach (var u in users) { u.IsActive = true; u.PasswordHash = new PasswordHasher<AppUser>().HashPassword(u, password); db.Users.Add(u); }
        await db.SaveChangesAsync(); await tx.CommitAsync();
    }
}
