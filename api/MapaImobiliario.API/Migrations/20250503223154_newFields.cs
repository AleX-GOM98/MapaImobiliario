using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MapaImobiliario.API.Migrations
{
    /// <inheritdoc />
    public partial class newFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Imoveis",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<decimal>(
                name: "Condominio",
                table: "Imoveis",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Iptu",
                table: "Imoveis",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Imoveis",
                type: "text",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Condominio",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Iptu",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Imoveis");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Imoveis",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
