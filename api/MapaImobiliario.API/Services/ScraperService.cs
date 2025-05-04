using MapaImobiliario.API.Data;
using MapaImobiliario.API.Models;
using MapaImobiliario.API.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

public class ScraperService: IScraperService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ScraperService> _logger;
    private readonly IConfiguration _configuration;

    public ScraperService(ApplicationDbContext context, ILogger<ScraperService> logger, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ImportImoveisFromPythonScriptAsync()
    {
        var relativeScriptPath = _configuration["ScraperSettings:ScriptPath"];
        var solutionDir = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName)!.FullName;
        var scriptPath = Path.Combine(solutionDir, relativeScriptPath);

        if (!File.Exists(scriptPath))
        {
            _logger.LogError("Script Python não encontrado em: {Path}", scriptPath);
            throw new FileNotFoundException("Script Python não encontrado.", scriptPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "python", 
            Arguments = scriptPath,  
            RedirectStandardOutput = true,  
            UseShellExecute = false,  
            CreateNoWindow = true,  
            StandardOutputEncoding = Encoding.UTF8
        };

        _logger.LogInformation("Executando script Python: {ScriptPath}", scriptPath);
        try
        {
            using var process = Process.Start(startInfo);

            if (process == null)
            {
                _logger.LogError("Falha ao iniciar o processo do script Python.");
                return;
            }

            string output = await process.StandardOutput.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(output))
            {
                _logger.LogWarning("Script Python executado, mas não retornou dados.");
                return;
            }

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Erro ao executar script Python");
            }
            var imoveis = DeserializeImoveisFromJson(output);

            if (imoveis == null || !imoveis.Any())
            {
                _logger.LogWarning("Nenhum imóvel válido foi retornado do script.");
                return;
            }

            await SaveImoveisToDatabase(imoveis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante importação de imóveis.");
            throw;
        }        
    }
    private async Task SaveImoveisToDatabase(List<Imovel> imoveis)
    {
        var linksExistentes = _context.Imoveis
            .Select(i => i.Link)
            .ToHashSet();

        var novosImoveis = imoveis
            .Where(i => !linksExistentes.Contains(i.Link))
            .ToList();

        if (novosImoveis.Any())
        {
            await _context.Imoveis.AddRangeAsync(novosImoveis);
            await _context.SaveChangesAsync();
            _logger.LogInformation("{Count} novos imóveis salvos no banco de dados.", novosImoveis.Count);
        }

    }
    private List<Imovel> DeserializeImoveisFromJson(string output)
    {
        try
        {
            var imoveis = JsonConvert.DeserializeObject<List<Imovel>>(output);
            return imoveis?.Where(i => !string.IsNullOrWhiteSpace(i.Link)).ToList() ?? new List<Imovel>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao desserializar a saída do script Python.");
            return new List<Imovel>();
        }
    }
}
