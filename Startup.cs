using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Cosmos;
using api_app.Services;

namespace api_app
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationSection configurationSection = Configuration.GetSection("CosmosDb");
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;

            CosmosClient dbClient = new CosmosClient(account, key);
            Container container = dbClient.GetContainer(databaseName, containerName);

            services.AddControllersWithViews();
            services.AddSingleton<IItemService>(InitializeCosmosClientInstanceAsync(container).GetAwaiter().GetResult());
            services.AddSingleton<IQueueService>(InitializeQueueServiceAsync(Configuration.GetSection("Queues")).GetAwaiter().GetResult());
            services.AddSingleton<MessageReceiver>(InitializeMessageReceiverAsync(container, Configuration.GetSection("Queues")).GetAwaiter().GetResult());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task<ItemService> InitializeCosmosClientInstanceAsync(Container container)
        {
            return new ItemService(container);
        }

        private static async Task<QueueService> InitializeQueueServiceAsync(IConfigurationSection configurationSection)
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            string queueName = configurationSection.GetSection("TaskQueueName").Value;
            return new QueueService(connectionString, queueName);
        }

        private static async Task<MessageReceiver> InitializeMessageReceiverAsync(Container container, IConfigurationSection configurationSection) 
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            string queueName = configurationSection.GetSection("ResultQueueName").Value;
            return new MessageReceiver(container, connectionString, queueName);
        }
    }
}
