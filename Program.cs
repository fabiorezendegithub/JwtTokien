using System.Security.Claims;
using JwtAspnet.Models;
using JwtAspnet.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

app.MapGet("/", (TokenService service)
    =>
{
    var user = new User(
        1,
        "Fabio",
        "fabio@email.com",
        "img/fabio.png",
        "123456",
        new[] { "admin", "stackholder" });

    return service.Create(user);
});

app.Run();