using Microsoft.EntityFrameworkCore;
using proyectofinal_septimo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApps", 
        policy => policy.WithOrigins(
                            "http://localhost:5173", // React
                            "http://localhost:4200"  // Angular (¡NUEVO!)
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
// ----------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Asegúrate de usar el nuevo nombre de la política
app.UseCors("AllowApps");

app.UseAuthorization();
app.MapControllers();
app.Run();