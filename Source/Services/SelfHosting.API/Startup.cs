﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SelfHosting.API.AppSettings;
using SelfHosting.API.WorkerService;
using SelfHosting.Repository;
using SelfHosting.Services;
using Serilog;
using Serilog.Events;

namespace SelfHosting.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {


            ///Basit bir SeriLog entegrasyonu gerçekleştiriyoruz.
            Configuration = configuration;

            //TODO: Slf'yi baz al
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                 .MinimumLevel.Override("System", LogEventLevel.Error)
                .WriteTo.RollingFile(@"C:\TayfunSelfHostSerilog\log-{Date}.txt", fileSizeLimitBytes: null, retainedFileCountLimit: null) //.txt yazdırmak için RollingFile Sink'ini kuruyoruz.
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                  options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );

            //TODO:Configden al
            services.AddDbContext<JobContext>(option => option.UseSqlServer(@"Data Source=192.168.5.43\LOGITEST,1434;Initial Catalog=LOGI3PLJMS;Persist Security Info=True;User ID=sa;Password=3plLogi+;MultipleActiveResultSets=True;Encrypt=False;Application Name=LOGIJOB;Connection Lifetime=3;Max Pool Size=3"));

            //AppSettings içerinde tanımlamış olduğum parametreleri sınıfıma set ediyorum.
            services.Configure<ConfigParameter>(Configuration.GetSection("ConfigParameter"));

            //Worker Servimiz ayağa kalktığında Üreteceği SchedulerFactory Context'ine API üzerinden erişebilmek için ctor'u bir kez ayağa kaldırıyoruz.
            services.AddSingleton(typeof(SchedulerWorkerService));
            
            //Worker Servisin Tüketeceği Scheduler servisi Singleton yapıyoruz. Api tarafından Monitoring işlemlerinde Quartz Context'ini kullanabilmek için..
            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddScoped<ICustomerJobRepository, CustomerJobRepository>();
            services.AddScoped<ICustomerJobService, CustomerJobService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IJobRepository, JobRepository>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            //{
            //    var context = serviceScope.ServiceProvider.GetRequiredService<JobContext>();
            //    context.Database.EnsureCreated();
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
