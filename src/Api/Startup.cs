using Api.Utils;
using Logic.Decorators;
using Logic.Dtos;
using Logic.Students;
using Logic.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();


            var config = new Config(3); //get from appsettings in real scenario
            var commandsConnectionString = new CommandsConnectionString(Configuration["ConnectionString"]);
            var queriesConnectionString = new QueriesConnectionString(Configuration["QueriesConnectionString"]);
            
            services.AddSingleton(config);
            services.AddSingleton(commandsConnectionString);
            services.AddSingleton(queriesConnectionString);
            services.AddSingleton<SessionFactory>();
            services.AddSingleton<Messages>();
            services.AddHandlers();
            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}
