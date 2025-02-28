using Microsoft.AspNetCore.Mvc;

namespace Gauniv.WebServer.Dtos
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Liste des jeux possédés
        public List<GameDto> PurchasedGames { get; set; }
    }
}

