using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var  genres = await _context.Genras.OrderBy(g => g.Name).ToListAsync();

            return Ok(genres);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(CreateGenreDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            await _context.Genras.AddAsync(genre);
            _context.SaveChanges();

            return Ok(dto);
        }

        [HttpPut(template: "{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateGenreDto dto)
        {
            var genre = await _context.Genras.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound(value:$"No Genre Was Found With ID: {id}");
            genre.Name = dto.Name;

            _context.SaveChanges();

            return Ok(genre);
        }

        [HttpDelete(template: "{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var genre = await _context.Genras.SingleOrDefaultAsync(g => g.Id == id);
            if (genre == null)
                return NotFound(value: $"No Genre Was Found With ID: {id}");

            _context.Remove(genre);
            _context.SaveChanges();
            return Ok(genre);
        }
    }
}
