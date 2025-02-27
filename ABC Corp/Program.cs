using Microsoft.EntityFrameworkCore;
using DomainLayer.Interfaces;
using InfraLayer.DB_Layer;
using InfraLayer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;
using InfraLayer.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using InfraLayer.Middleware;


var builder = WebApplication.CreateBuilder(args);

//Authorization


/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));*/
//Setting up RBAC using policies
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
//    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
//});

                                                //Connecting Application Insights


//var instrumentationKey = builder.Configuration.GetSection("ApplicationInsights").GetValue<string>("InstrumentationKey");
//builder.Services.AddApplicationInsightsTelemetry(connstring => { connstring.ConnectionString = instrumentationKey; });
//builder.Logging.AddApplicationInsights();

                                                    ////Connecting Redis cache


//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
//    options.InstanceName = builder.Configuration.GetConnectionString("RedisInstanceName");
//});

                                                    //Blob storage connection

/*var blobStorageConnection = builder.Configuration.GetSection("BlobStorageSettings");
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(blobStorageConnection.GetValue<string>("ConnectionString"));
});

builder.Services.AddSingleton<BlobService>(provider =>
{
    var blobServiceClient = provider.GetRequiredService<BlobServiceClient>();
    var containerName = blobStorageConnection.GetValue<string>("ContainerName");
    return new BlobService(blobServiceClient, containerName);
});
*/
    

                                        //Entity Framework setup     // Sqlite is used as Database

//var kvUrl = builder.Configuration.GetSection("KeyVault").GetValue<string>("Uri");
//var client = new SecretClient(new Uri(kvUrl), new DefaultAzureCredential(includeInteractiveCredentials: true));
//var awsSecretValue = (client.GetSecret("DBConnectionString")).Value.Value;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Bearer"; 
    options.DefaultChallengeScheme = "Bearer"; 
})
.AddJwtBearer("Bearer", options =>
{
    options.Authority = ""; 
    options.Audience = ""; 
    options.RequireHttpsMetadata = false; 
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//DI registration
builder.Services.AddScoped<ITaskRepo, TaskRepository>();
builder.Services.AddScoped<IUserRepo, UserRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseHsts();
app.UseMiddleware<AuthorizationMiddleware>();
app.UseAuthorization();

app.MapControllers();


app.Run();
