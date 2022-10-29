using Microsoft.EntityFrameworkCore.Migrations;

namespace AppClubes.Data.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JugadorClub",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clubid = table.Column<int>(nullable: false),
                    Jugadorid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JugadorClub", x => x.id);
                    table.ForeignKey(
                        name: "FK_JugadorClub_club_Clubid",
                        column: x => x.Clubid,
                        principalTable: "club",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JugadorClub_jugador_Jugadorid",
                        column: x => x.Jugadorid,
                        principalTable: "jugador",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JugadorClub_Clubid",
                table: "JugadorClub",
                column: "Clubid");

            migrationBuilder.CreateIndex(
                name: "IX_JugadorClub_Jugadorid",
                table: "JugadorClub",
                column: "Jugadorid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JugadorClub");
        }
    }
}
