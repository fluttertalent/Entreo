using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using WebApp.Entreo.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ICustomHttpHelper, CustomHttpHelper>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();

builder.Services.AddMudServices();
builder.Services.AddMudLocalization();
builder.Services.AddLocalization(options => options.ResourcesPath = "Localization");

await builder.Build().RunAsync();
