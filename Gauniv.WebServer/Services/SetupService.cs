using Gauniv.WebServer.Data;
using Gauniv.WebServer.Websocket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Gauniv.WebServer.Services
{
    public class SetupService : IHostedService
    {
        private ApplicationDbContext? applicationDbContext;
        private readonly IServiceProvider serviceProvider;
        private Task? task;

        public SetupService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = serviceProvider.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                applicationDbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();

                if(applicationDbContext is null)
                {
                    throw new Exception("ApplicationDbContext is null");
                }

                if (applicationDbContext.Database.GetPendingMigrations().Any())
                {
                    applicationDbContext.Database.Migrate();
                }

                // Ajouter ici les données que vous insérer dans votre DB au démarrage
                // If the game is already in the DB, do not add it
                // applicationDbContext.Games.Add(new Game { Name = "Counter Strike", Description = "War game 5v5 with guns and russians", price = 19.99F });
                //applicationDbContext.Games.Add(new Game { Name = "League of Legends", Description = "5v5 face to face game with champions to destroy nexus", price = 4.99F });
                //applicationDbContext.Games.Add(new Game { Name = "Rocket League", Description = "Car and football fusion to make this game", price = 9.99F });
                //applicationDbContext.SaveChanges();

                return Task.CompletedTask;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
