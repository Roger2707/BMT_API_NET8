using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Store_API;
using Store_API.Data;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;
using Store_API.Services;
using Store_API.Validations;
using Stripe;
using System.Text;
using Renci.SshNet;
using Store_API.RedisConfig;
using Store_API.RabbitMQConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Vô hiệu hóa kiểm tra ModelState tự động bằng cách cấu hình ApiBehaviorOptions
    options.SuppressModelStateInvalidFilter = true;

    // Custom Errors Validations
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorResponse = new
        {
            StatusCode = 400,
            Errors = context.ModelState
                            .Where(m => m.Value.Errors.Count > 0)
                            .ToDictionary
                            (
                                    d => char.ToLower(d.Key[0]) + d.Key.Substring(1),
                                    d => d.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                            )
        };
        return new BadRequestObjectResult(errorResponse);
    };
});

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

#region Identity

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
    // Reset token valid from 2 hours
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
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
    //.AddCookie(options =>
    //{
    //    options.Cookie.HttpOnly = true;
    //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //    options.Cookie.SameSite = SameSiteMode.Lax;
    //    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    //    options.SlidingExpiration = true;
    //})
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:ClientID"];
        options.ClientSecret = builder.Configuration["OAuth:ClientSecret"];
        options.CallbackPath = "/signin-google";
    })
;

//Cookie Policy needed for External Auth
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

builder.Services.AddAuthorization();

#endregion 


#region Services

builder.Services.AddHttpClient();

builder.Services.AddTransient<IDapperService, DapperService>();
builder.Services.AddTransient<IImageRepository, ImageService>();
builder.Services.AddTransient<ICSVRepository, CSVService>();
builder.Services.AddTransient<EmailSenderService>();
builder.Services.AddTransient<ITokenRepository, TokenIdentityService>();
builder.Services.AddTransient<IPaymentRepository, PaymentService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
#endregion 

#region Connect Redis

var sshConfig = new SshConfig
{
    SshHost = "ec2-47-129-57-48.ap-southeast-1.compute.amazonaws.com",
    SshPort = 22,
    SshUsername = "ec2-user",
    SshKeyFile = @"A:\Personal\EConmmercial\jump_server_keypair.pem",
    LocalPort = 6379,
    RemoteHost = "rediscache.vejvgg.ng.0001.apse1.cache.amazonaws.com",
    RemotePort = 6379
};

builder.Services.AddSingleton(sshConfig);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var sshManager = new SshTunnelManager(sp.GetRequiredService<SshConfig>());
    sshManager.StartTunnel();

    var configuration = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" },
        AbortOnConnectFail = false,
    };

    return ConnectionMultiplexer.Connect(configuration);
});

#endregion 

#region RabbitMQ

builder.Services.AddSingleton<MessageQueue>();
builder.Services.AddSingleton<RabbitMQService>();

#endregion 

var app = builder.Build();

#region Middlewares


#endregion 

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

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();

    context.Database.Migrate();
    DatabaseSeeding.SeedData(context);
    DatabaseSeeding.SeedData(builder.Configuration.GetConnectionString("DefaultConnection"));
}

app.Run();
