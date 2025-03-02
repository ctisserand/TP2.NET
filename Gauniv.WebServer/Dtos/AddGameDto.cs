using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gauniv.WebServer.Dtos
{
    public class AddGameDTO
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        // Payload optionnel pour l'ajout : si aucun fichier n'est fourni, on pourra stocker un tableau vide

        public IFormFile? Payload { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Liste des noms de catégories à associer au jeu
        public List<string> Categories { get; set; } = new List<string>();
    }
}
