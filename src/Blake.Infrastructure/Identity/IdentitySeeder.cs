using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Dynasty.Carrington.Blake.Infrastructure.Identity;

public static class IdentitySeeder
{
    // TODO: usunąć hardcodowane dane przed deploymentem (tylko MVP).
    private const string AdminEmail = "admin@Blake.local";
    private const string AdminPassword = "Admin123!";

    public static async Task SeedAsync(IServiceProvider sp, CancellationToken cancellationToken = default)
    {
        using var scope = sp.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in new[] { "User", "Admin" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                EnsureSucceeded(result, $"rola {role}");
            }
        }

        var admin = await userManager.FindByEmailAsync(AdminEmail);
        if (admin is null)
        {
            admin = new ApplicationUser { UserName = AdminEmail, Email = AdminEmail, EmailConfirmed = true };
            EnsureSucceeded(await userManager.CreateAsync(admin, AdminPassword), "admin");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            EnsureSucceeded(await userManager.AddToRoleAsync(admin, "Admin"), "rola admina");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string what)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Seed ({what}) nieudany: {string.Join("; ", result.Errors.Select(e => e.Description))}");
        }
    }
}
