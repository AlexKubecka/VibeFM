using FootballManager.Utilities;
using FootballManager.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FootballManager.Data.FootballManagerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Clear and seed the database on every startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FootballManagerDbContext>();
    dbContext.Database.EnsureCreated();
    // Remove all data
    dbContext.Players.RemoveRange(dbContext.Players);
    dbContext.Teams.RemoveRange(dbContext.Teams);
    dbContext.StaffMembers.RemoveRange(dbContext.StaffMembers);
    dbContext.SaveChanges();
    // Seed from combined_team_data_wrapped.json
    JsonDbSeeder.SeedFromCombinedTeamData(dbContext, @"../FootballManager/Data/combined_team_data_wrapped.json");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
