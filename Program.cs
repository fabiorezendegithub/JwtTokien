using JwtAspnet;
using JwtAspnet.Models;
using JwtAspnet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<TokenService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.PrivateKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
builder.Services.AddAuthorization(x => { x.AddPolicy("Admin", p => p.RequireRole("admin")); });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", (TokenService service)
    =>
{
    var user = new User(
        1,
        "Fabio",
        "fabio@email.com",
        "img/fabio.png",
        "123456",
        ["stackholder", "singleuser"]);

    return service.Create(user);
});

app.MapGet("/restrito", (ClaimsPrincipal user) => new
{
    id = user.FindFirst("id")?.Value,
    name = user.FindFirst(ClaimTypes.Name)?.Value,
    email = user.FindFirst(ClaimTypes.Email)?.Value,
    givenName = user.FindFirst(ClaimTypes.GivenName)?.Value,
    image = user.FindFirst("image")?.Value
})
    .RequireAuthorization();

app.MapGet("/admin", () => "Você tem acesso.").RequireAuthorization("admin");

app.Run();