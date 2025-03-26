using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Store_API.Data;
using Store_API.Models;
using System.Text;
using Store_API.Hubs;
using Store_API.Extensions;
using Store_API.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

#region Connection Database

builder.Services.AddDbContext<StoreContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Config Swagger JWT Bearer

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BMT_APIs", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put Bearer + your token in the box below",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

#endregion 

#region CORS Policy

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    );
});

#endregion

#region Authentication + Authorization

builder.Services
    .AddIdentity<User, Store_API.Models.Role>(opt =>
    {
        opt.Password.RequiredLength = 7;
        opt.Password.RequireDigit = false;
        opt.Password.RequireUppercase = false;
        opt.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<StoreContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => {
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration["JWTSettings:TokenKey"]))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:ClientID"];
        options.ClientSecret = builder.Configuration["OAuth:ClientSecret"];
    });

builder.Services.AddAuthorization();

#endregion 

#region Services

builder.Services.AddApplicationServices();

#endregion 

#region Connect Redis 

//var sshConfig = new SshConfig
//{
//    SshHost = "ec2-47-129-101-54.ap-southeast-1.compute.amazonaws.com",
//    SshPort = 22,
//    SshUsername = "ec2-user",
//    SshKeyFile = @"A:\Personal\EConmmercial\jump-ssh-keypair.pem",
//    LocalPort = 6379,
//    RemoteHost = "master.redisvalkeynoncluster.ohpidg.apse1.cache.amazonaws.com",
//    RemotePort = 6379
//};

//builder.Services.AddSingleton(sshConfig);
//builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
//{
//    var sshManager = new SshTunnelManager(sp.GetRequiredService<SshConfig>());
//    sshManager.StartTunnel();

//    var configuration = new ConfigurationOptions
//    {
//        EndPoints = { "localhost:6379" },
//        AbortOnConnectFail = false,
//    };

//    Console.WriteLine("🔄 Connecting Redis...");
//    var connection = ConnectionMultiplexer.Connect(configuration);
//    Console.WriteLine("✅ Connected Redis Successfully !!");
//    return connection;
//});

var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

#endregion 

#region Signal IR

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

#endregion

#region RabbitMQ

builder.Services.AddHostedService<RabbitMQConsumerService>();

#endregion 

var app = builder.Build();

#region Middlewares

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

#endregion 

app.MapControllers();

#region Signal hubs

app.MapHub<OrderHub>("/orderHub");

#endregion


#region Seed Data

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    context.Database.Migrate();
    DatabaseSeeding.SeedData(context);
}

#endregion

app.Run();