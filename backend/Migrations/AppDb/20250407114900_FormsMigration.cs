using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class FormsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.FormId);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    StudentAnswersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.StudentAnswersId);
                });

            migrationBuilder.CreateTable(
                name: "ObligatoryPreference",
                columns: table => new
                {
                    ObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormForWhichCorrespondFormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligatoryPreference", x => x.ObligatoryPreferenceId);
                    table.ForeignKey(
                        name: "FK_ObligatoryPreference_Form_FormForWhichCorrespondFormId",
                        column: x => x.FormForWhichCorrespondFormId,
                        principalTable: "Form",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrespondingFormFormId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StudentAnswersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answer_Form_CorrespondingFormFormId",
                        column: x => x.CorrespondingFormFormId,
                        principalTable: "Form",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answer_StudentAnswers_StudentAnswersId",
                        column: x => x.StudentAnswersId,
                        principalTable: "StudentAnswers",
                        principalColumn: "StudentAnswersId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionForObligatoryPreference",
                columns: table => new
                {
                    OptionForObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreferenceObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false),
                    OptionForPreference = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionForObligatoryPreference", x => x.OptionForObligatoryPreferenceId);
                    table.ForeignKey(
                        name: "FK_OptionForObligatoryPreference_ObligatoryPreference_PreferenceObligatoryPreferenceId",
                        column: x => x.PreferenceObligatoryPreferenceId,
                        principalTable: "ObligatoryPreference",
                        principalColumn: "ObligatoryPreferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChoosablePreference",
                columns: table => new
                {
                    ChoosablePreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoosablePreference", x => x.ChoosablePreferenceId);
                    table.ForeignKey(
                        name: "FK_ChoosablePreference_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "AnswerId");
                });

            migrationBuilder.CreateTable(
                name: "OptionForObligatoryPreference_Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    OptionForObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionForObligatoryPreference_Answer", x => new { x.AnswerId, x.OptionForObligatoryPreferenceId });
                    table.ForeignKey(
                        name: "FK_OptionForObligatoryPreference_Answer_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "AnswerId");
                    table.ForeignKey(
                        name: "FK_OptionForObligatoryPreference_Answer_OptionForObligatoryPreference_OptionForObligatoryPreferenceId",
                        column: x => x.OptionForObligatoryPreferenceId,
                        principalTable: "OptionForObligatoryPreference",
                        principalColumn: "OptionForObligatoryPreferenceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answer_CorrespondingFormFormId",
                table: "Answer",
                column: "CorrespondingFormFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_StudentAnswersId",
                table: "Answer",
                column: "StudentAnswersId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoosablePreference_AnswerId",
                table: "ChoosablePreference",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligatoryPreference_FormForWhichCorrespondFormId",
                table: "ObligatoryPreference",
                column: "FormForWhichCorrespondFormId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionForObligatoryPreference_PreferenceObligatoryPreferenceId",
                table: "OptionForObligatoryPreference",
                column: "PreferenceObligatoryPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionForObligatoryPreference_Answer_OptionForObligatoryPreferenceId",
                table: "OptionForObligatoryPreference_Answer",
                column: "OptionForObligatoryPreferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoosablePreference");

            migrationBuilder.DropTable(
                name: "OptionForObligatoryPreference_Answer");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "OptionForObligatoryPreference");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "ObligatoryPreference");

            migrationBuilder.DropTable(
                name: "Form");
        }
    }
}
