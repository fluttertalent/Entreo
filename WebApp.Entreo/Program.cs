using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;
using System.Diagnostics;
using WebApp.Entreo.Client.Services;
using WebApp.Entreo.Components;
using WebApp.Entreo.Data;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Services;
using WebApp.Entreo.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IUserAccessor, UserAccessor>();
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped(sp => new HttpClient());
builder.Services.AddScoped<ICustomHttpHelper, CustomHttpHelper>();

builder.Services.AddScoped<IFirebaseService, FirebaseService>();

var app = builder.Build();

ApplicationDbContext.InitializeDatabase(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();

//Download the Seq UI here: https://datalust.co/seq
Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
            .Enrich.WithMachineName()
            .Enrich.WithClientIp()
            .Enrich.WithRequestHeader("User-Agent")
#if DEBUG
            .Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached) // Used to filter out data due to debugging
            .WriteTo.Console()
#endif
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

//app.UseMiddleware<EndpointLoggingMiddleware>();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(WebApp.Entreo.Client._Imports).Assembly);

Log.Debug($"Started site '{typeof(Program).Assembly.GetName().Name}'");

app.Run();
