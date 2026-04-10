using MongoDB.Driver;
using ShortenUrlWeb.Interfaces;
using ShortenUrlWeb.Repositories;
using ShortenUrlWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Đọc connection string: ưu tiên Environment Variable (Render), fallback về appsettings.json (local)
var mongoUri = Environment.GetEnvironmentVariable("MONGODB_URI")
               ?? builder.Configuration["MongoDB:ConnectionString"]
               ?? throw new InvalidOperationException("Không tìm thấy MongoDB connection string.");

var dbName = builder.Configuration["MongoDB:DatabaseName"] ?? "ShortenUrlDb";

// 2. Đăng ký MongoClient là Singleton (1 instance duy nhất cho toàn app — best practice của MongoDB)
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoUri));

// 3. Đăng ký IMongoDatabase để inject thẳng vào Repository
builder.Services.AddSingleton<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));

// 4. Đăng ký Repository và Service
builder.Services.AddScoped<IShortenUrlRepository, MongoShortenUrlRepository>();
builder.Services.AddScoped<IShortenUrlService, ShortenUrlService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ❌ Bỏ UseHttpsRedirection trên Render vì Render tự xử lý HTTPS ở load balancer
// app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();