using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Gauniv.WebServer.Dtos
{
    public class EditGameDTO
    {
        public int? Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Liste des noms de catégories à associer au jeu
        public List<string> Categories { get; set; } = new List<string>();

        // Payload optionnel : si null, l'ancien payload sera conservé
        public IFormFile? Payload { get; set; }
    }
}
