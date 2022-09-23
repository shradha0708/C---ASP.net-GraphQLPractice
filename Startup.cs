using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPPractice.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ASPPractice.Repository;
using ASPPractice.GraphQL;
using GraphQL;
using GraphQL.Server;

namespace ASPPractice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<StudentContext> (options => options.UseSqlServer(Configuration["ConnectionStrings:ASPPractice"]));
            services.AddScoped<StudentRepository>();
            services.AddScoped<StudentSchema>();
            services.AddScoped<IServiceProvider>(s => new FuncServiceProvider(s.GetRequiredService)); // Iservice provider -> to create object of name class dynamically
            services.AddGraphQL().AddSystemTextJson().AddGraphTypes(ServiceLifetime.Scoped);
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, StudentContext sc) //do something in between frontend and backend
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            sc.Seed();
            app.UseGraphQL<StudentSchema>();
            app.UseGraphQLPlayground();
        }
    }
}
