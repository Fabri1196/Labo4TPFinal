using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppClubes.Data.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoria", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jugador",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    apellido = table.Column<string>(nullable: true),
                    nombre = table.Column<string>(nullable: true),
                    biografía = table.Column<string>(nullable: true),
                    foto = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jugador", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "club",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(nullable: true),
                    resumen = table.Column<string>(nullable: true),
                    fechaFund = table.Column<DateTime>(nullable: false),
                    imagenEscudo = table.Column<string>(nullable: true),
                    pais = table.Column<string>(nullable: true),
                    Categoriaid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club", x => x.id);
                    table.ForeignKey(
                        name: "FK_club_categoria_Categoriaid",
                        column: x => x.Categoriaid,
                        principalTable: "categoria",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_club_Categoriaid",
                table: "club",
                column: "Categoriaid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "club");

            migrationBuilder.DropTable(
                name: "jugador");

            migrationBuilder.DropTable(
                name: "categoria");
        }
    }
}
