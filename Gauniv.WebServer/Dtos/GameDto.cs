using AutoMapper;
using Elfie.Serialization;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;

namespace Gauniv.WebServer.Dtos
{
    public class GameDto
    {
        public int Id { get; set; }
        public String? Name { get; set; }
        public String? Description { get; set; }
        public BinaryData? payload { get; set; }
        public float price { get; set; }
        public String[]? categories { get; set; }
    }
}