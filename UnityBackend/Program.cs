using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDb>(option=> option.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);
var app = builder.Build();

app.UseHttpsRedirection();


app.Run();

