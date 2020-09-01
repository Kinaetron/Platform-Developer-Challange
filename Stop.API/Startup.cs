using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Stop.API.Mappers;
using TinyCsvParser.Mapping;
using System.IO.Abstractions;
using Stop.Model;
using Stop.Repository;
using Stop.Repository.Importers;

namespace Stop.API
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
            services.AddHttpClient();
            services.AddControllers();
            services.AddScoped<IFileSystem, FileSystem>();
            services.AddScoped<ICsvMapping<CSVStopViewModel>, CSVStopMapping>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<ICSVStopRepository, CSVStopRepository>();
            services.AddScoped<ICSVImporter<CSVStopViewModel>, CSVImporter<CSVStopViewModel>>();

            services.AddAutoMapper(typeof(MappingProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
