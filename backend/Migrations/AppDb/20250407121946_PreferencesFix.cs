using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class PreferencesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisplayPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayFirstName = table.Column<bool>(type: "bit", nullable: false),
                    DisplayLastName = table.Column<bool>(type: "bit", nullable: false),
                    DisplayEmail = table.Column<bool>(type: "bit", nullable: false),
                    DisplayPhoneNumber = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayPreferences", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayPreferences");
        }
    }
}
