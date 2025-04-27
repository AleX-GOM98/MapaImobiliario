using MapaImobiliario.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MapaImobiliario.API.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Imovel> Imoveis { get; set; }
}
