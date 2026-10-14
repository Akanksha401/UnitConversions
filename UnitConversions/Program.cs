using UnitConversions.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register our converter layer via standard Dependency Injection
builder.Services.AddSingleton<IUnitConverterService, UnitConverterService>();

// Configure OpenAPI/Swagger for clean exploratory interaction
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();