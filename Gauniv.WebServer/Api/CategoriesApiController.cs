using AutoMapper;
using Gauniv.WebServer.Data; // Assurez-vous d'importer le bon contexte de la base de données
using Gauniv.WebServer.Dtos;  // Importer les DTOs pour les catégories
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gauniv.WebServer.Api
{
    [Route("api/1.0.0/[controller]")]
    [ApiController]
    public class CategoriesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly IMapper _mapper;

        public CategoriesApiController(ApplicationDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        // Lister toutes les catégories
        [HttpGet]
        public async Task<IActionResult> ListCategories()
        {
            var categories = await _appDbContext.Categories.ToListAsync();

            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found.");
            }

            // Mapper les catégories récupérées en DTO
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

            return Ok(categoryDtos);
        }

        // Action pour obtenir une catégorie par Id
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
        {
            var category = await _appDbContext.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound(); // Retourne 404 si la catégorie n'est pas trouvée
            }

            return Ok(_mapper.Map<CategoryDto>(category));
        }
    }
}
