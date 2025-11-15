using ManagerAPI.Models;

namespace ManagerAPI.Data
{
    public class SeedData
    {
        public static async Task EnsureSeedAsync(ApplicationDbContext context)
        {
            //sedear un usuario admin para pruebas
            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                    Role = "admin"
                };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
