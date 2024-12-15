﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _AllowedExtenstions = new List<string> { ".jpg",".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var movies = await _context.Movies
                .OrderByDescending(x => x.Rate)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto 
                {
                   Id =m.Id,
                   GenreId = m.GenreId,
                   GenreName = m.Genre.Name,
                   Poster = m.Poster,
                   Rate = m.Rate,
                   StoreLine = m.StoreLine,
                   Title = m.Title,
                   Year = m.Year
                   
                    
                })
                .ToListAsync();

            return Ok (movies);
        }

        [HttpGet(template:"{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genre)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            var dto = new MovieDetailsDto
            {
                   Id = movie.Id,
                   GenreId = movie.GenreId,
                   GenreName = movie.Genre?.Name,
                   Poster = movie.Poster,
                   Rate = movie.Rate,
                   StoreLine = movie.StoreLine,
                   Title = movie.Title,
                   Year = movie.Year
            };

            return Ok (dto);
        }

        [HttpGet("GetByGenreId")]
        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _context.Movies
                .OrderByDescending(x => x.Rate)
                .Where(m => m.GenreId == genreId)
                .Include(m => m.Genre)
                .Select(m => new MovieDetailsDto
                {
                    Id = m.Id,
                    GenreId = m.GenreId,
                    GenreName = m.Genre.Name,
                    Poster = m.Poster,
                    Rate = m.Rate,
                    StoreLine = m.StoreLine,
                    Title = m.Title,
                    Year = m.Year


                })
                .ToListAsync();

            return Ok(movies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm]MovieDto dto)
        {
            if (dto == null)
                return BadRequest(error: "Poster Is Required!");

            if (!_AllowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest(error: "Only .png  and .jpg images are Allowed!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest(error: "Max Allowed Size for Poster 1MB!");

            var isValidGenre = await _context.Genras.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenre)
                return BadRequest(error: "InValid genre ID!");

            using var dataStream = new MemoryStream();

            await dto.Poster.CopyToAsync(dataStream);

            var movie = new Movie
            {
                GenreId = dto.GenreId,
                Title = dto.Title,
                Poster = dataStream.ToArray(),
                Rate = dto.Rate,
                StoreLine = dto.StoreLine,
                Year = dto.Year

            };
            await _context.AddAsync(movie);
            _context.SaveChanges();

            return Ok(movie);
        }

        [HttpPut(template:"{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] MovieDto dto)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound(value: $"No Movie Was Found With ID {id}");

            var isValidGenre = await _context.Genras.AnyAsync(g => g.Id == dto.GenreId);

            if (!isValidGenre)
                return BadRequest(error: "InValid genre ID!");

            if (dto.Poster != null)
            {
                if (!_AllowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest(error: "Only .png  and .jpg images are Allowed!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest(error: "Max Allowed Size for Poster 1MB!");
                using var dataStream = new MemoryStream();

                await dto.Poster.CopyToAsync(dataStream);
                movie.Poster = dataStream.ToArray();
            }
           
            movie.Title = dto.Title;
            movie.GenreId = dto.GenreId;
            movie.Year = dto.Year;
            movie.StoreLine = dto.StoreLine;
            movie.Rate = dto.Rate;

            _context.SaveChanges();

            return Ok(movie);
        }

        [HttpDelete(template:"{id}")]
        public async Task<IActionResult>DeleteAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);

            if (movie == null)
                return NotFound(value:$"No Movie Was Found With ID {id}");

            _context.Remove(movie);
            _context.SaveChangesAsync();

            return Ok(movie);
        }
    }
}
