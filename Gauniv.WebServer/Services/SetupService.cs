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
                var userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                var signinManager = scope.ServiceProvider.GetService<SignInManager<User>>();




                if (applicationDbContext is null)
                {
                    throw new Exception("ApplicationDbContext is null");
                }

                if (applicationDbContext.Database.GetPendingMigrations().Any())
                {
                    applicationDbContext.Database.Migrate();
                }

                // Ajouter ici les données que vous insérer dans votre DB au démarrage
                if (!applicationDbContext.Games.Any())
                {
                    var games = new List<Game>
                    {
                        new Game { Name = "Game 1", Description = "Description for Game 1", Payload = new byte[0], Price = 19.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Action" } } },
                        new Game { Name = "Game 2", Description = "Description for Game 2", Payload = new byte[0], Price = 29.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Adventure" } } },
                        new Game { Name = "Game 3", Description = "Description for Game 3", Payload = new byte[0], Price = 14.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "RPG" } } },
                        new Game { Name = "Game 4", Description = "Description for Game 4", Payload = new byte[0], Price = 9.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Strategy" } } },
                        new Game { Name = "Game 5", Description = "Description for Game 5", Payload = new byte[0], Price = 39.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Shooter" } } },
                        new Game { Name = "Game 6", Description = "Description for Game 6", Payload = new byte[0], Price = 24.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Horror" } } },
                        new Game { Name = "Game 7", Description = "Description for Game 7", Payload = new byte[0], Price = 34.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Survival" } } },
                        new Game { Name = "Game 8", Description = "Description for Game 8", Payload = new byte[0], Price = 44.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Simulation" } } },
                        new Game { Name = "Game 9", Description = "Description for Game 9", Payload = new byte[0], Price = 12.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "Puzzle" } } },
                        new Game { Name = "Game 10", Description = "Description for Game 10", Payload = new byte[0], Price = 49.99m, Categories = new List<Game.Category>{ new Game.Category { Name = "MMO" } } },
                    };

                    applicationDbContext.Games.AddRange(games);
                    applicationDbContext.SaveChanges();
                }

                if (!applicationDbContext.Users.Any())
                {

                    var users = new List<User>
                    {
                        new User
                            {
                                UserName = "alice@example.com",
                                FirstName = "Alice",
                                LastName = "Smith",
                                Email = "alice@example.com",
                                PurchasedGames = new List<Game> { applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 1")! }
                            },
                        new User
                            {
                                UserName = "bob@example.com",
                                FirstName = "Bob",
                                LastName = "Johnson",
                                Email = "bob@example.com",
                                PurchasedGames = new List<Game> { applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 2")!, applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 3")! }
                            },
                        new User
                            {   
                                UserName = "charlie@example.com",
                                FirstName = "Charlie",
                                LastName = "Brown",
                                Email = "charlie@example.com",
                                PurchasedGames = new List<Game>()
                            },
                        new User
                            {
                                UserName = "diana@example.com",
                                FirstName = "Diana",
                                LastName = "Miller",
                                Email = "diana@example.com",
                                PurchasedGames = new List<Game> { applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 4")!, applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 5")!, applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 6")! }
                            },
                        new User
                            {
                                UserName = "edward@example.com",
                                FirstName = "Edward",
                                LastName = "Davis",
                                Email = "edward@example.com",
                                PurchasedGames = new List<Game> { applicationDbContext.Games.FirstOrDefault(g => g.Name == "Game 7")! }
                            }
                    };

                    if (!applicationDbContext.Roles.Any(r => r.Name == "Admin"))
                    {
                        roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
                    }
                    if (!applicationDbContext.Roles.Any(r => r.Name == "User"))
                    {
                        roleManager.CreateAsync(new IdentityRole("User")).Wait();
                    }

                    foreach (var user in users)
                    {
                        userManager.CreateAsync(user,"password").Wait();
                        userManager.AddToRoleAsync(user, "User").Wait();
                    }
                    
                    userManager.AddToRoleAsync(users[0], "Admin").Wait();

                    applicationDbContext.Users.AddRange(users);
                    applicationDbContext.SaveChanges();
                }
                //var t0 = signinManager.PasswordSignInAsync("edward@example.com", "password", false, lockoutOnFailure: false);
                //t0.Wait();

                //var t1 = signinManager.CanSignInAsync(applicationDbContext.Users.First(x => x.Email == "edward@example.com"));
                //t1.Wait();
                //var t2 = signinManager.CheckPasswordSignInAsync(applicationDbContext.Users.First(x => x.Email == "edward@example.com"), "password",false);
                //t2.Wait();
                return Task.CompletedTask;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
