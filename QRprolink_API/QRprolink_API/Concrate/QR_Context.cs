using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QRprolink_API.Entity;

namespace QRprolink_API.Concrate
{
    public class QR_Context:IdentityDbContext<AppUser,AppRole,string>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-CH9SD0T;initial Catalog=QRprolinkDB;Integrated Security=true; TrustServerCertificate=True");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
