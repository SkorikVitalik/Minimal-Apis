using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minimal_Apis;
using Minimal_Apis.auth;
using Minimal_Apis.Data;
using Minimal_Apis.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("MysqlConnection"), new MySqlServerVersion(new Version(8, 0, 30))));
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddSingleton<IUserRepository>(new UserRepository());
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    };
                });



var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/login", [AllowAnonymous] (HttpContext context, ITokenService tokenService, IUserRepository userRepository) =>
{
    var user = new UserModel()
    {
        Password = context.Request.Query["password"],
        UserName = context.Request.Query["username"],
    };
    var userDto = userRepository.GetUser(user);
    if (userDto == null)
        return Results.Unauthorized();
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"], builder.Configuration["Jwt:Issuer"], userDto);
    return Results.Ok(token);
});

app.MapGet("/hotels",  async (IHotelRepository hotelRepository) => Results.Ok(await hotelRepository.GetHotelAsync()))
                        .Produces<List<Hotel>>(StatusCodes.Status200OK, "application/json")
                       .WithName("GetterAllHotels")
                       .WithTags("Getters");
app.MapGet("/hotels/{id}", [Authorize] async (int id, IHotelRepository hotelRepository) => await hotelRepository.GetHotelAsync(id) is Hotel hotel
    ? Results.Ok(hotel) :
    Results.NotFound())
    .WithName("GetterHotel")
    .WithTags("Getters");
app.MapGet("/hotel/search/name/{query}", [Authorize] async (string query, IHotelRepository hotelRepository) => await hotelRepository.GetHotelAsync(query) is IEnumerable<Hotel> hotels
    ? Results.Ok(hotels)
    : Results.NotFound(Array.Empty<Hotel>()))
    .Produces<List<Hotel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchHotels")
    .WithTags("Getters")
    .ExcludeFromDescription();
app.MapGet("/hotel/search/location/{coordinate}", [Authorize]  async (Coordinate coordinate, IHotelRepository hotelRepository) =>
         await hotelRepository.GetHotelAsync(coordinate) is IEnumerable<Hotel> hotels
         ? Results.Ok(hotels)
         : Results.NotFound()
    ).ExcludeFromDescription();
app.MapPost("/hotels", [Authorize] async ([FromBody] Hotel hotel, IHotelRepository hotelRepository) =>
{
    await hotelRepository.InsertHotelAsync(hotel);
    await hotelRepository.SaveAsync();
    return Results.Created($"/hotels/{hotel.Id}", hotel);
})
    .Accepts<Hotel>("application/json")
    .Produces<Hotel>(StatusCodes.Status201Created)
    .WithName("CreaterHotel")
    .WithTags("Creaters");
app.MapPut("/hotels", [Authorize] async ([FromBody] Hotel hotel, IHotelRepository hotelRepository) =>
    {
        await hotelRepository.UpdateHotelAsync(hotel);
        await hotelRepository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<Hotel>("application/json")
    .WithName("UpdaterHotel")
    .WithTags("Updaters");
app.MapDelete("/hotels/{id}", [Authorize] async (IHotelRepository hotelRepository, int id) =>
{
    await hotelRepository.DeleteHotelAsync(id);
    await hotelRepository.SaveAsync();
    return Results.NoContent();
})
    .WithName("DeleterHotel")
    .WithTags("Deleters");
app.UseHttpsRedirection();
app.Run();
