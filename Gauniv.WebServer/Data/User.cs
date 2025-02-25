using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Data
{
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public List<Game> PurchasedGames { get; set; } = new List<Game>();
    }
}
