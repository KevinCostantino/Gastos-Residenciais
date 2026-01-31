using Microsoft.EntityFrameworkCore;
using GastosResiduenciais.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração do Entity Framework com SQLite
builder.Services.AddDbContext<GastosResiduenciaisContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=gastos_residenciais.db"));

// Configuração dos controllers
builder.Services.AddControllers();

// Configuração do CORS para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // React na porta 3000
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Middleware para criação automática do banco de dados em desenvolvimento
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GastosResiduenciaisContext>();
    context.Database.EnsureCreated(); // Cria o banco se não existir
}

app.UseHttpsRedirection();

// Ativar CORS
app.UseCors("AllowReactApp");

// Configurar roteamento dos controllers
app.MapControllers();

app.Run();
