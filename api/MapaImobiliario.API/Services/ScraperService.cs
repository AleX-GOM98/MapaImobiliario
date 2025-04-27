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
        // Caminho para o script Python, relativo � pasta de trabalho da aplica��o
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(),"scraper", "viva_real_scraper.py");
        Console.WriteLine(scriptPath);

        // Configura��o do processo para rodar o Python
        var startInfo = new ProcessStartInfo
        {
            FileName = "python", // Certifique-se de que 'python' est� no PATH do sistema
            Arguments = scriptPath,  // Passa o caminho do script Python
            RedirectStandardOutput = true,  // Captura a sa�da do Python
            UseShellExecute = false,  // N�o usa a shell, para capturar a sa�da
            CreateNoWindow = true  // N�o abre uma janela do console
        };

        using (var process = Process.Start(startInfo))
        using (var reader = process.StandardOutput)
        {
            // L� a sa�da do script Python
            var output = await reader.ReadToEndAsync();
            var imoveis = ParsePythonOutput(output);  // Fun��o que converte a sa�da em lista de objetos

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
        // Parse a sa�da do Python e converta para objetos Imovel
        var imoveis = new List<Imovel>();
        // O formato de sa�da precisa ser adequado, por exemplo, JSON ou CSV
        // Exemplo de convers�o fict�cia:
        // imoveis = JsonConvert.DeserializeObject<List<Imovel>>(output);

        return imoveis;
    }
}
