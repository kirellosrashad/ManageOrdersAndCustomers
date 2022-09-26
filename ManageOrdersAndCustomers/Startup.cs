using ManageOrdersAndCustomersBL.Models.ViewModels;
using ManageOrdersAndCustomersBL.UnitOfWork;
using ManageOrdersAndCustomersEF;
using ManageOrdersAndCustomersEF.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ManageOrdersAndCustomersAPI
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
            services.AddDbContext<AppDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("OrderManager"),  // define the connectionstring
                    b => b.MigrationsAssembly(typeof(AppDBContext).Assembly.FullName)));  // tells what is the DBContext class

            //services.AddTransient(typeof(IRepository<>), typeof(Repository<>));  //==> to inject the generic repository to any controller
            services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));  //==> to inject the generic UnitOfWork to any controller
            services.AddScoped<CustomersVM>();
            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
