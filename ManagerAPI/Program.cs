using ManagerAPI.Data;
using ManagerAPI.Hubs;
using ManagerAPI.Middleware;
using ManagerAPI.Services;
using ManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// implementacion JWT 
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwt["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddSingleton<JwtService>();

builder.Services.AddSignalR();

builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// CORS
app.UseCors(x =>
    x.AllowAnyOrigin()
     .AllowAnyMethod()
     .AllowAnyHeader());

// Middlewares globales
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlerMiddleware>();




// Seed db 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await SeedData.EnsureSeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

app.Urls.Add($"http://0.0.0.0:{port}");

// HTTPS solo local 
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapHub<CommentsHub>("/hubs/comments");
app.Run();
