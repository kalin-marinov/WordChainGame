using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using WordChainGame.Auth;
using WordChainGame.Auth.Hashing;
using WordChainGame.Auth.Identity;
using WordChainGame.Data;
using WordChainGame.Data.Reports;
using WordChainGame.Data.Topics;
using WordChainGame.Data.Topics.Validation;
using WordChainGame.Data.Topics.Words;
using WordChainGame.Data.Topics.Words.Validation;

namespace WordChainGame
{
    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();


            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentity(Configuration);
            services.AddTopicStorage(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("admin", "True"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ConfigureAuth(app);

            app.UseMvc();
        }
    }

    public static class IdentityExtensions
    {

        /// <summary> Adds Identity to the project without having roles / role manager / role validator </summary>
        public static void AddIdentity(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(configuration["Storage:RedisConnection"]));

            services.AddScoped<IDatabase>(s => s.GetService<IConnectionMultiplexer>().GetDatabase());

            services.AddSingleton<IHashSerailizer<User>, UserHasher>();
            services.TryAddScoped<IUserStore<User>, UserStore<User>>();

            // Hosting doesn't add IHttpContextAccessor by default
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Identity services
            services.TryAddSingleton<IdentityMarkerService>();
            services.TryAddScoped<IUserValidator<User>, UserValidator>();
            services.TryAddScoped<IPasswordValidator<User>, PasswordValidator<User>>();
            services.TryAddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            // No interface for the error describer so we can add errors without rev'ing the interface
            services.TryAddScoped<IdentityErrorDescriber>();
            services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
            services.TryAddScoped<UserManager<User>, UserManager<User>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<User>, ClaimsFactory>();
            services.TryAddScoped<SignInManager<User>>();
            services.TryAddScoped<TokenProvider>();
            services.TryAddScoped<IIdentityResolver, IdentityResolver>();

            services.AddScoped<IOptions<TokenProviderOptions>>(_ => Options.Create(new TokenProviderOptions
            {
                Path = "/api/token",
                Audience = "ExampleAudience",
                Issuer = "ExampleIssuer",
                SigningCredentials = new SigningCredentials(Startup.signingKey, SecurityAlgorithms.HmacSha256),
            }));
        }

        public static void AddTopicStorage(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddScoped<IMongoClient>(_ => new MongoClient(configuration["Storage:MongoConnection"]));
            services.AddScoped<IMongoDatabase>(s => s.GetService<IMongoClient>().GetDatabase("wordgame"));

            services.AddScoped<IReportStore, ReportStore>();
            services.AddScoped<IReportsManager, ReportManager>();

            services.AddScoped<ITopicStore, TopicStore>();
            services.AddScoped<ITopicValidator, TopicValidator>();
            services.AddScoped<IWordValidator, WordValidator>();
            services.AddScoped<ITopicManager, TopicsManager>();
        }
    }

}
