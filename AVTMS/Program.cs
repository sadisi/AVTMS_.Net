using AVTMS.Data;
using AVTMS.Models;
using AVTMS.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Email invoice services
builder.Services.AddScoped<EmailServices>();

//---------Connect Database -----
var ConnectionString = builder.Configuration.GetConnectionString("MySqlConn");

builder.Services.AddDbContext<AppDbContext>(option =>
{
   option.UseMySql(ConnectionString,ServerVersion.AutoDetect(ConnectionString));

});
//------Connect Database  endpoint------

builder.Services.AddIdentity<Users, IdentityRole>(Options =>
{
    //Configure password Requirements
    Options.Password.RequireNonAlphanumeric = false;
    Options.Password.RequiredLength = 8;
    Options.Password.RequireUppercase = false;
    Options.Password.RequireLowercase = false;
    Options.User.RequireUniqueEmail = true;
    Options.SignIn.RequireConfirmedAccount = false;
    Options.SignIn.RequireConfirmedEmail = false;
    Options.SignIn.RequireConfirmedPhoneNumber = false;

})
    
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();


// Configure file size limits for multipart/form-data (i.e., file uploads)

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});

// Configure HttpClient with a custom timeout
builder.Services.AddHttpClient("YoloApiClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(100); // Set timeout to 30 seconds
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

// Serve static files from the "wwwroot" folder
app.UseStaticFiles(); 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    //when run the code Login apge open firstly
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
