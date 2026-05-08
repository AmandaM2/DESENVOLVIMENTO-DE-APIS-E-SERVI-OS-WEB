using Microsoft.EntityFrameworkCore;
using Api.Data;         // Ajustado de Livros para Api
using Api.Interfaces;   // Onde estão suas IContaRepository e IContaService
using Api.Repositories;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do Banco de Dados MySQL (Pomelo)
// Ajustado para usar "DefaultConnection" que configuramos no appsettings.json

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL($"{builder.Configuration.GetConnectionString("ConexaoPadrao")}"));

// --- 2. INJEÇÃO DE DEPENDÊNCIA (SISTEMA BANCÁRIO) ---

// Registrando Repositórios
builder.Services.AddScoped<IContaService, ContaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();

// --- 3. CONFIGURAÇÃO DO JWT (SEGURANÇA) ---

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "SenhaSuperSecretaDePeloMenos32Caracteres");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero // Remove o atraso na expiração do token
    };
});

// --- 4. SWAGGER COM BOTÃO DE CADEADO ---

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema Bancário Api", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer {seu token}"
    });

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", doc)] = []
    });
});

var app = builder.Build();

// --- 5. PIPELINE DE EXECUÇÃO (MIDDLEWARES) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ORDEM IMPORTANTE: Autenticação primeiro, depois Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
