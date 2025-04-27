using MapaImobiliario.API.Data;
using MapaImobiliario.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ImoveisController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ScraperService _scraperService;

    public ImoveisController(ApplicationDbContext context, ScraperService scraperService)
    {
        _context = context;
        _scraperService = scraperService;
    }

    [HttpGet]
    public async Task<IActionResult> GetImoveis()
    {
        var imoveis = await _context.Imoveis.ToListAsync();
        return Ok(imoveis);
    }

    [HttpPost]
    public async Task<IActionResult> CreateImovel(Imovel imovel)
    {
        _context.Imoveis.Add(imovel);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetImoveis), new { id = imovel.Id }, imovel);
    }
    [HttpGet("scrape")]
    public async Task<IActionResult> ScrapeImoveis()
    {
        await _scraperService.ScrapeAndSaveDataAsync();
        return Ok("Imóveis salvos com sucesso.");
    }

    [HttpGet("scraperTeste")]
    public async Task<IActionResult> ScrapeImoveisTeste()
    {
        return Ok("Imóveis salvos com sucesso.");
    }
}
