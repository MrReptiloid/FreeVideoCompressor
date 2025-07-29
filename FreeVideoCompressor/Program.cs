using FreeVideoCompressor.Application.Services;
using FreeVideoCompressor.DataAccess;
using FreeVideoCompressor.DataAccess.Repositories;
using FreeVideoCompressor.Domain.Abstractions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FreeVideoCompressorDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfire(cfg =>
    cfg.UseMemoryStorage());
builder.Services.AddHangfireServer();

builder.Services.AddScoped<CompressVideoFlowRepository>();

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IVideoProcessingService, VideoProcessingService>();
builder.Services.AddScoped<IVideoValidationService, VideoValidationService>();
builder.Services.AddScoped<CompressService>();
builder.Services.AddScoped<FfmpegService>();


var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseStaticFiles();
app.UseHangfireDashboard();

app.Run();