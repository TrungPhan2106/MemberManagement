using MySql.Data.MySqlClient;
using System;
using Microsoft.Extensions.Hosting;
using MemberManagement.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(AutoMappingProflie).Assembly);

// Add services to the container.
builder.Services.AddDbContext<MyDbContext>(
    options => options.UseMySQL(builder.Configuration.GetConnectionString("Default")/*"server=127.0.0.1;user=root;database=gymxanhpt;password=Tma@2022@T1P;port=3306"*/));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<MyDbContext>();

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
