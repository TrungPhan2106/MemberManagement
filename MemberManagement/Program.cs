using MySql.Data.MySqlClient;
using System;
using Microsoft.Extensions.Hosting;
using StudioManagement.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Log4Net.AspNetCore;
using System.Configuration;
using StudioManagement.Repository.IRepository;
using StudioManagement.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddLog4Net();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(AutoMappingProflie).Assembly);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("Default")/*"server=127.0.0.1;user=root;database=gymxanhpt;password=Tma@2022@T1P;port=3306"*/));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MyDbContext>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "MemberIndex",
    pattern: "{controller=Member}/{action=MemberIndex}/{id?}");
app.Run();
