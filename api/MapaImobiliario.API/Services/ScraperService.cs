using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MapaImobiliario.API.Data;
using MapaImobiliario.API.Models;

public class ScraperService
{
    private readonly ApplicationDbContext _context;

    public ScraperService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ScrapeAndSaveDataAsync()
    {
        // Caminho para o script Python, relativo à pasta de trabalho da aplicação
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(),"scraper", "viva_real_scraper.py");
        Console.WriteLine(scriptPath);

        // Configuração do processo para rodar o Python
        var startInfo = new ProcessStartInfo
        {
            FileName = "python", // Certifique-se de que 'python' está no PATH do sistema
            Arguments = scriptPath,  // Passa o caminho do script Python
            RedirectStandardOutput = true,  // Captura a saída do Python
            UseShellExecute = false,  // Não usa a shell, para capturar a saída
            CreateNoWindow = true  // Não abre uma janela do console
        };

        using (var process = Process.Start(startInfo))
        using (var reader = process.StandardOutput)
        {
            // Lê a saída do script Python
            var output = await reader.ReadToEndAsync();
            var imoveis = ParsePythonOutput(output);  // Função que converte a saída em lista de objetos

            // Salva os dados no banco de dados
            await SaveImoveisToDatabase(imoveis);
        }
    }

    private async Task SaveImoveisToDatabase(List<Imovel> imoveis)
    {
        await _context.Imoveis.AddRangeAsync(imoveis);
        await _context.SaveChangesAsync();
    }

    private List<Imovel> ParsePythonOutput(string output)
    {
        // Parse a saída do Python e converta para objetos Imovel
        var imoveis = new List<Imovel>();
        // O formato de saída precisa ser adequado, por exemplo, JSON ou CSV
        // Exemplo de conversão fictícia:
        // imoveis = JsonConvert.DeserializeObject<List<Imovel>>(output);

        return imoveis;
    }
}
