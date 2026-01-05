using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniShop.Data;
using MiniShop.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// OVO JE BITNO (Identity)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// SEED: migracije + admin + demo data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var db = services.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";

    var adminSection = app.Configuration.GetSection("AdminUser");
    var adminEmail = adminSection["Email"];
    var adminPassword = adminSection["Password"];

    if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        throw new Exception("Admin user credentials are not configured.");

    // 1) Role
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // 2) User
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            throw new Exception($"Admin user creation failed: {errors}");
        }
    }

    // 3) Add to role
    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }

    // 4) DEMO DATA (only if empty)
    if (!await db.Categories.AnyAsync())
    {
        var classics = new Category { Name = "Classics", Slug = "classics" };
        var scifi = new Category { Name = "Science Fiction", Slug = "science-fiction" };
        var programming = new Category { Name = "Programming", Slug = "programming" };
        var fantasy = new Category { Name = "Fantasy", Slug = "fantasy" };

        db.Categories.AddRange(classics, scifi, programming, fantasy);
        await db.SaveChangesAsync();

        db.Products.AddRange(
            new Product
            {
                Name = "1984",
                Price = 14.99m,
                CategoryId = classics.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780451524935-L.jpg",
                DescriptionHtml = "<p><strong>George Orwell</strong> — dystopian classic about surveillance and control.</p>"
            },
            new Product
            {
                Name = "Brave New World",
                Price = 13.99m,
                CategoryId = classics.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780060850524-L.jpg",
                DescriptionHtml = "<p><strong>Aldous Huxley</strong> — classic novel about a controlled society and engineered happiness.</p>"
            },
            new Product
            {
                Name = "Dune",
                Price = 19.99m,
                CategoryId = scifi.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780441172719-L.jpg",
                DescriptionHtml = "<p><strong>Frank Herbert</strong> — epic science fiction saga set on Arrakis.</p>"
            },
            new Product
            {
                Name = "Foundation",
                Price = 16.99m,
                CategoryId = scifi.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780553293357-L.jpg",
                DescriptionHtml = "<p><strong>Isaac Asimov</strong> — the start of a legendary series about the fall and rise of empires.</p>"
            },
            new Product
            {
                Name = "Clean Code",
                Price = 34.99m,
                CategoryId = programming.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg",
                DescriptionHtml = "<p>Praktični vodič za pisanje čitljivog i održivog koda.</p><ul><li>Readability</li><li>Refactoring</li><li>Best practices</li></ul>"
            },
            new Product
            {
                Name = "The Pragmatic Programmer",
                Price = 32.99m,
                CategoryId = programming.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780201616224-L.jpg",
                DescriptionHtml = "<p>Klasična knjiga o praktičnom pristupu razvoju softvera.</p>"
            },
            new Product
            {
                Name = "The Hobbit",
                Price = 15.99m,
                CategoryId = fantasy.Id,
                IsActive = true,
                ImagePath = "https://covers.openlibrary.org/b/isbn/9780345339683-L.jpg",
                DescriptionHtml = "<p><strong>J. R. R. Tolkien</strong> — avantura koja uvodi u svijet Međuzemlja.</p>"
            }
        );

        await db.SaveChangesAsync();
    }
}

app.Run();
