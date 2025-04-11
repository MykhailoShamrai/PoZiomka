using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class FormsMigrationWithCustomTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    FormId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameOfForm = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.FormId);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswersCollections",
                columns: table => new
                {
                    StudentAnswersId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswersCollections", x => x.StudentAnswersId);
                });

            migrationBuilder.CreateTable(
                name: "ObligatoryPreferences",
                columns: table => new
                {
                    ObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormForWhichCorrespondFormId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObligatoryPreferences", x => x.ObligatoryPreferenceId);
                    table.ForeignKey(
                        name: "FK_ObligatoryPreferences_Forms_FormForWhichCorrespondFormId",
                        column: x => x.FormForWhichCorrespondFormId,
                        principalTable: "Forms",
                        principalColumn: "FormId");
                });

            migrationBuilder.CreateTable(
                name: "Answers",
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
                    table.PrimaryKey("PK_Answers", x => x.AnswerId);
                    table.ForeignKey(
                        name: "FK_Answers_Forms_CorrespondingFormFormId",
                        column: x => x.CorrespondingFormFormId,
                        principalTable: "Forms",
                        principalColumn: "FormId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_StudentAnswersCollections_StudentAnswersId",
                        column: x => x.StudentAnswersId,
                        principalTable: "StudentAnswersCollections",
                        principalColumn: "StudentAnswersId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionsForObligatoryPreferences",
                columns: table => new
                {
                    OptionForObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PreferenceObligatoryPreferenceId = table.Column<int>(type: "int", nullable: false),
                    OptionForPreference = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsForObligatoryPreferences", x => x.OptionForObligatoryPreferenceId);
                    table.ForeignKey(
                        name: "FK_OptionsForObligatoryPreferences_ObligatoryPreferences_PreferenceObligatoryPreferenceId",
                        column: x => x.PreferenceObligatoryPreferenceId,
                        principalTable: "ObligatoryPreferences",
                        principalColumn: "ObligatoryPreferenceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChoosablePreferences",
                columns: table => new
                {
                    ChoosablePreferenceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChoosablePreferences", x => x.ChoosablePreferenceId);
                    table.ForeignKey(
                        name: "FK_ChoosablePreferences_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
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
                        name: "FK_OptionForObligatoryPreference_Answer_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "AnswerId");
                    table.ForeignKey(
                        name: "FK_OptionForObligatoryPreference_Answer_OptionsForObligatoryPreferences_OptionForObligatoryPreferenceId",
                        column: x => x.OptionForObligatoryPreferenceId,
                        principalTable: "OptionsForObligatoryPreferences",
                        principalColumn: "OptionForObligatoryPreferenceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_CorrespondingFormFormId",
                table: "Answers",
                column: "CorrespondingFormFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_StudentAnswersId",
                table: "Answers",
                column: "StudentAnswersId");

            migrationBuilder.CreateIndex(
                name: "IX_ChoosablePreferences_AnswerId",
                table: "ChoosablePreferences",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ObligatoryPreferences_FormForWhichCorrespondFormId",
                table: "ObligatoryPreferences",
                column: "FormForWhichCorrespondFormId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionForObligatoryPreference_Answer_OptionForObligatoryPreferenceId",
                table: "OptionForObligatoryPreference_Answer",
                column: "OptionForObligatoryPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionsForObligatoryPreferences_PreferenceObligatoryPreferenceId",
                table: "OptionsForObligatoryPreferences",
                column: "PreferenceObligatoryPreferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChoosablePreferences");

            migrationBuilder.DropTable(
                name: "OptionForObligatoryPreference_Answer");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "OptionsForObligatoryPreferences");

            migrationBuilder.DropTable(
                name: "StudentAnswersCollections");

            migrationBuilder.DropTable(
                name: "ObligatoryPreferences");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
