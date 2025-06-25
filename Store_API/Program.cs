using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Store_API.Data;
using System.Text;
using Store_API.Extensions;
using Store_API.Models.Users;
using Store_API.Services;
using Store_API.SignalR;
using MassTransit;
using Store_API.Consumers;

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
    .AddIdentity<User, Store_API.Models.Users.Role>(opt =>
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
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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

        // SignalR read access token from query string
        opt.Events = new JwtBearerEvents
        {
           OnMessageReceived = context =>
           {
           var accessToken = context.Request.Query["access_token"];
           var path = context.HttpContext.Request.Path;

           if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/ordersHub"))
           {
               context.Token = accessToken;
           }

           return Task.CompletedTask;
           }
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:ClientID"];
        options.ClientSecret = builder.Configuration["OAuth:ClientSecret"];
    });

builder.Services.AddAuthorizationServices();

#endregion 

#region MassTransit

builder.Services.AddMassTransit(x =>
{
    // register DI
    x.AddConsumersFromNamespaceContaining<StockHoldCreatedConsumer>();

    // ensure to add the outbox after savechange success
    x.AddEntityFrameworkOutbox<StoreContext>(o =>
    {
        // if process rabbitMQ server is downed, retry every 10 seconds
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UseSqlServer();

        // when savechange success, no add to message queue immediately, add outbox first
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("stock-hold-created", e =>
        {
            e.ConfigureConsumer<StockHoldCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("stock-hold-expired", e =>
        {
            e.ConfigureConsumeTopology = false;

            e.Bind("my-delayed-exchange", configure =>
            {
                configure.ExchangeType = "x-delayed-message";
                configure.SetExchangeArgument("x-delayed-type", "direct");
                configure.RoutingKey = "";
            });

            e.ConfigureConsumer<StockHoldExpiredConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});

#endregion

#region Services

builder.Services.AddApplicationServices();
builder.Services.AddHostedService<BasketBackgroundService>();

#endregion 

#region Redis 

var redisConnectionString = builder.Configuration.GetValue<string>("Redis:ConnectionString");
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

#endregion 

#region SignalR

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

#endregion

var app = builder.Build();

#region Middlewares

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

// Because the request body can only be read once, we need to enable buffering
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

app.MapControllers();

#endregion 

app.MapHub<OrdersHub>("/ordersHub");

app.Run();