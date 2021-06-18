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

            services.AddSingleton(config);
            services.AddSingleton(new SessionFactory(Configuration["ConnectionString"]));
            services.AddTransient<UnitOfWork>();
            services.AddSingleton<Messages>();

            services.AddTransient<ICommandHandler<EditPersonalInfoCommand>>(provider =>
                                    new AuditLoggingDecorator<EditPersonalInfoCommand>(
                                    new DatabaseRetryDecorator<EditPersonalInfoCommand>(new EditPersonalInfoCommandHandler(provider.GetService<SessionFactory>()),provider.GetService<Config>())));
            services.AddTransient<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
            services.AddTransient<ICommandHandler<UnRegisterCommand>, UnRegisterCommandHandler>();
            services.AddTransient<ICommandHandler<EnrollCommand>, EnrollCommandHandler>();
            services.AddTransient<ICommandHandler<TransferCommand>, TransferCommandHandler>();
            services.AddTransient<ICommandHandler<DisenrollCommand>, DisenrollCommandHandler>();
            services.AddTransient <IQueryHandler<GetListQuery, List<StudentDto>>>();
 
            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandler>();
            app.UseMvc();
        }
    }
}
