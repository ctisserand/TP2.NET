using Gauniv.WebServer.Data;
using Gauniv.WebServer.Websocket;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using static Gauniv.WebServer.Data.Game;

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

                var categories = new List<string>
                    {
                        "Action", "Adventure", "Shooter", "RPG", "Strategy", "Puzzle", "Horror", "Survival", "Simulation", "MMO"
                    };


                // Insérer les catégories si elles n'existent pas déjà
                foreach (var categoryName in categories)
                {
                    if (!applicationDbContext.Categories.Any(c => c.Name == categoryName))
                    {
                        applicationDbContext.Categories.Add(new Category { Name = categoryName });
                    }
                }

                // Sauvegarder les catégories dans la base de données
                applicationDbContext.SaveChanges();



                // Chemin où se trouvent les fichiers des jeux
                string baseDirectory = @"C:\Users\brieuc.mandin\Desktop\3A\MBDS\ServAppetEnvideDev";
                // Ajouter ici les données que vous insérer dans votre DB au démarrage
                if (!applicationDbContext.Games.Any())
                {
                    var games = new List<Game>
                    {
                        new Game
                        {
                            Name = "Game 1",
                            Description = "Description for Game 1",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_1.exe")),  // Remplace "Game_1.exe" par ton fichier réel
                            Price = 19.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Action") }
                        },
                        new Game
                        {
                            Name = "Game 2",
                            Description = "Description for Game 2",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_2.exe")),  // Remplace "Game_2.exe" par ton fichier réel
                            Price = 29.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Adventure") }
                        },
                        new Game
                        {
                            Name = "Game 3",
                            Description = "Description for Game 3",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_3.exe")),  // Remplace "Game_3.exe" par ton fichier réel
                            Price = 14.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "RPG") }
                        },
                        new Game
                        {
                            Name = "Game 4",
                            Description = "Description for Game 4",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_4.exe")),  // Remplace "Game_4.exe" par ton fichier réel
                            Price = 9.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "RPG") }
                        },
                        new Game
                        {
                            Name = "Game 5",
                            Description = "Description for Game 5",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_5.exe")),  // Remplace "Game_5.exe" par ton fichier réel
                            Price = 39.99m,
                            Categories = new List<Category> {
                                applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Shooter"),
                                applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Action")
                            }
                        },
                        new Game
                        {
                            Name = "Game 6",
                            Description = "Description for Game 6",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_6.exe")),  // Remplace "Game_6.exe" par ton fichier réel
                            Price = 24.99m,
                            Categories = new List<Category> {
                                applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Horror"),
                                applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Survival")
                            }
                        },
                        new Game
                        {
                            Name = "Game 7",
                            Description = "Description for Game 7",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_7.exe")),  // Remplace "Game_7.exe" par ton fichier réel
                            Price = 34.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Survival") }
                        },
                        new Game
                        {
                            Name = "Game 8",
                            Description = "Description for Game 8",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_8.exe")),  // Remplace "Game_8.exe" par ton fichier réel
                            Price = 44.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Simulation") }
                        },
                        new Game
                        {
                            Name = "Game 9",
                            Description = "Description for Game 9",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_9.exe")),  // Remplace "Game_9.exe" par ton fichier réel
                            Price = 12.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "Puzzle") }
                        },
                        new Game
                        {
                            Name = "Game 10",
                            Description = "Description for Game 10",
                            Payload = File.ReadAllBytes(Path.Combine(baseDirectory, "Game_10.exe")),  // Remplace "Game_10.exe" par ton fichier réel
                            Price = 49.99m,
                            Categories = new List<Category> { applicationDbContext.Categories.FirstOrDefault(c => c.Name == "MMO") }
                        }
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
                        Task.Run(async () =>
                        {
                            try
                            {
                            await userManager.CreateAsync(user, "password");
                            await userManager.AddToRoleAsync(user, "User");

                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }).Wait();
                    }
                    
                    userManager.AddToRoleAsync(users[0], "Admin").Wait();

                    //applicationDbContext.Users.AddRange(users);
                    //applicationDbContext.SaveChanges();
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
