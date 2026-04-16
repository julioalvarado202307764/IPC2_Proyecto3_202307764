var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// El DataStore DEBE ser Singleton para que los datos no se borren en cada petición
builder.Services.AddSingleton<Proyecto3_API.Services.DataStoreService>();

// El procesador puede ser transitorio/scoped porque solo hace operaciones
builder.Services.AddScoped<Proyecto3_API.Services.XmlProcessorService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
