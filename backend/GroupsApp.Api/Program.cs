using Microsoft.EntityFrameworkCore;
using GroupsApp.Api.Data;
using GroupsApp.Api.Services;
using GroupsApp.Api.Mappings;   // for your AutoMapper profiles

var builder = WebApplication.CreateBuilder(args);

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core In-Memory database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("GroupsDb"));

// Register AutoMapper with your MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register your application service
builder.Services.AddScoped<IGroupsService, GroupsService>();

// Register MVC controllers
builder.Services.AddControllers();

var app = builder.Build();

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controller routes
app.MapControllers();

app.Run();
