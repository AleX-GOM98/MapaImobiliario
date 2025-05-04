namespace MapaImobiliario.API.Models;

public class Imovel
{
    public int Id { get; set; }
    public string Titulo { get; set; }
    public string? Endereco { get; set; }
    public decimal? Preco { get; set; }
    public decimal? Condominio { get; set; }
    public decimal? Iptu { get; set; }
    public string? Link { get; set; }
}
