using Microsoft.EntityFrameworkCore;
using WebApplication11.Data;
using WebApplication11.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<StockService>();
builder.Services.AddHttpClient<FinnhubService>();

builder.Services.AddDbContext<StockContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("StockHistoryDB")));

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
