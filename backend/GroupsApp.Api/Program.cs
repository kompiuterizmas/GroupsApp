using Microsoft.EntityFrameworkCore;
using GroupsApp.Api.Data;
using GroupsApp.Api.Services;
using GroupsApp.Api.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow React app on localhost:3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EF Core In-Memory database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("GroupsDb"));

// Register AutoMapper with your MappingProfile
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register application services
builder.Services.AddScoped<IGroupsService, GroupsService>();

// Register MVC controllers
builder.Services.AddControllers();

var app = builder.Build();

// Enable CORS globally
app.UseCors("AllowReactApp");

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map controller routes
app.MapControllers();

app.Run();
