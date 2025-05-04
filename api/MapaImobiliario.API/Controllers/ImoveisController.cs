using MapaImobiliario.API.Data;
using MapaImobiliario.API.Models;
using MapaImobiliario.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MapaImobiliario.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImoveisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IScraperService _scraperService;
        private readonly ILogger<ImoveisController> _logger;

        public ImoveisController(
            ApplicationDbContext context,
            IScraperService scraperService,
            ILogger<ImoveisController> logger)
        {
            _context = context;
            _scraperService = scraperService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Imovel>>> GetImoveis()
        {
            var imoveis = await _context.Imoveis.AsNoTracking().ToListAsync();
            return Ok(imoveis);
        }

        [HttpPost]
        public async Task<ActionResult<Imovel>> CreateImovel([FromBody] Imovel imovel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Imoveis.Add(imovel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImoveis), new { id = imovel.Id }, imovel);
        }

        [HttpPost("scrape")]
        public async Task<IActionResult> ScrapeImoveis()
        {
            try
            {
                await _scraperService.ImportImoveisFromPythonScriptAsync();
                return Ok("Importação concluída com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao importar imóveis.");
                return StatusCode(500, "Erro ao importar imóveis.");
            }
        }
    }
}
