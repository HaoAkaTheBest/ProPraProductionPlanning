using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SupportLibrary.CreateProduction;
using SupportLibrary.Data;
using SupportLibrary.DataAccess;
using SupportLibrary.Models;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mjk2MDUwNUAzMjMzMmUzMDJlMzBoeHNJMENZTWpDUlhIcVBPZzlBcHNQU3lMS0ZBRHlvbWlSSDQ5OFRYb0ZBPQ==");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<ISqlDataAccess, SqlDataAccess>();
builder.Services.AddScoped<IMachineSqlDataService,MachineSqlDataService>();
builder.Services.AddScoped<IOrderSqlDataService, OrderSqlDataService>();
builder.Services.AddScoped<IRoutingSqlDataService, RoutingSqlDataService>();
builder.Services.AddScoped<IMachineAvailabilitySqlDataService, MachineAvailabilitySqlDataService>();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddScoped<IProductionPlanning, ProductionPlanning>();
builder.Services.AddScoped<IMachineUsedSqlDataService,MachineUsedSqlDataService>();
builder.Services.AddScoped<IOptimizedProductionPlanning, OptimizedProductionPlanning>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
