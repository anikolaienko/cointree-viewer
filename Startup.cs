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
            // services.AddMassTransit(cfg =>
            // {
            //     cfg.AddConsumer<DbPriceConsumer>();
            //     cfg.AddConsumer<CoinTreeWatcher>();
            // });

            var messageBus = Bus.Factory.CreateUsingInMemory(configure =>
            {
                configure.ReceiveEndpoint("cointree_prices", endpoint => {
                    endpoint.Consumer<DbPriceConsumer1>();
                    // endpoint.Consumer<CoinTreeWatcher1>();
                });
            });
            

            services.AddSignalR();
            services.AddScoped<PriceWatcherHub>();
            services.AddScoped<IPriceWatcher, CoinTreeWatcher>();
            services.AddScoped<DbPriceConsumer>();
            services.AddSingleton<IBus>(messageBus);

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
            

            var messageBus = serviceProvider.GetService<IBus>();
            var priceWatcher = serviceProvider.GetService<IPriceWatcher>();

            messageBus.ConnectInstance(serviceProvider.GetService<DbPriceConsumer>());
            messageBus.ConnectInstance(priceWatcher);

            (messageBus as IBusControl).Start();
            messageBus.Publish(new CoinTreeViewer.Models.CurrencyPrice() {Name = "HJdkf"});

            priceWatcher.Start();
        }
    }
}
