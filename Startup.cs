using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using AutoMapper;
using CoinTreeViewer.Database;
using CoinTreeViewer.Services;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.EntityFrameworkCore;

namespace CoinTreeViewer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => AutoMapperConfig.Configure(cfg));
            services.AddMassTransit(cfg => 
            {
                cfg.AddConsumer<DbPriceConsumer>();
            });
            
            services.AddSignalR();
            services.AddScoped<PriceWatcherHub>();
            services.AddSingleton<IPriceWatcher, CoinTreeWatcher>();
            services.AddScoped<DbPriceConsumer>();
            services.AddSingleton<IBus>(provider =>
            {
                return Bus.Factory.CreateUsingInMemory(configure =>
                {
                    configure.ReceiveEndpoint("cointree_prices", endpoint => {
                        endpoint.LoadFrom(provider);
                    });
                });
            });
            services.AddEntityFrameworkSqlite();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=prices.db")); //only for development

            services.AddMemoryCache();
            services.AddMvc();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app,
                            IHostingEnvironment env,
                            IServiceProvider serviceProvider)
        {
            var messageBus = serviceProvider.GetService<IBus>();
            var priceWatcher = serviceProvider.GetService<IPriceWatcher>();
            var dbPriceConsumer = serviceProvider.GetService<DbPriceConsumer>();
            var dbContext = serviceProvider.GetService<AppDbContext>();

            // Migrate db to latest state
            dbContext.Database.Migrate();
            
            // Start message bus
            (messageBus as IBusControl).Start();
            // Start price watcher for CoinTree API.
            priceWatcher.Start(dbPriceConsumer.GetLatestPrice());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<PriceWatcherHub>("prices");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
