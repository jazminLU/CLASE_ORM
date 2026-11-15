using CLASE_ORM.Infrastructure;
using CLASE_ORM.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:4201")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorization();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Para empezar a usar el ORM EntityFrameworkCore.SqlServer, tienen que instalarlo desde el administrador de paquetes Nuget
// esta en Herramientas => Administrador de paquetes Nuget => Administrar paquetes Nuget.
//Son dos cosas es EntityFrameworkCore.SqlServer y EntityFrameworkCore.Tools.
//tiene que coincidir con la versión de .NET que eligieron, no instalen la 10 o superior si están usando en su propia solución la version 8.


app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("AllowAngular");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
