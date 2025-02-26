using Gauniv.WebServer.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace Gauniv.WebServer.Models
{
    public class CreateViewModel()
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public IFormFile Content { get; set; }
        public int[] Categories { get; set; }
    }
    public class EditViewModel()
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public float? Price { get; set; }
        public IFormFile? Content { get; set; }
        public int[]? Categories { get; set; }
    }

    /**
     * This class is used to display the list of games
     */
    public class IndexViewModel()
    {
        public required List<Game> Games { get; set; }

    }

    /**
     * This class is used to create a game
     */
    public class CreateGameViewModel()
    {
        public String? Name { get; set; }
        public String? Description { get; set; }
        public float Price { get; set; }
        public String[]? Categories { get; set; }
        public byte[]? payload { get; set; }
    }
}
