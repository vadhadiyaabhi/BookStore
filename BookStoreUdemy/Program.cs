using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repositories;
using BookStore.DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookStore.Utilities;
using Stripe;
using BookStore.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(          // Remember the diff between AddDbContext & AddDbContextPool
    builder.Configuration.GetConnectionString("BookStoreUdemyDB")
    ));

// Bind Striope settingd with values of appSetting.json file
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// AddDefaultTokenProviders - is already included in default identity, but now we're using custome identity, so we need to specify them manually
// options => options.SignIn.RequireConfirmedAccount = true             - For verifying email while user registers
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<AppDbContext>();

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();             // Whenever we request the object of ICategoryRepository, It will give us the object of CategoryRepository class, And ref will be in var of type ICategoryRepository 
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();             // This will handle all the dependency Inject, No need to inject for each repository
builder.Services.AddSingleton<IEmailSender, EmailSender>();             
builder.Services.AddScoped<IDbInitializer, DbInitializer>();             


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


builder.Services.AddMvc().AddSessionStateTempDataProvider();
builder.Services.AddSession();

// Default paths overridden because we have Identity as Area, so we need to add them
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

var app = builder.Build();

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

// Configuration for stripe = assigned global API key inside our pipiline
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

SeedDatabase();

app.UseAuthentication();

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();                            // Required for mapping razor pages
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}"
);

app.Run();


void SeedDatabase()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initializer();
    }
}