using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class RefactoredMigrationForForms : Migration
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
                name: "Answers",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrespondingFormFormId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormForWhichCorrespondFormId = table.Column<int>(type: "int", nullable: true),
                    IsObligatory = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Forms_FormForWhichCorrespondFormId",
                        column: x => x.FormForWhichCorrespondFormId,
                        principalTable: "Forms",
                        principalColumn: "FormId");
                });

            migrationBuilder.CreateTable(
                name: "OptionsForQuestions",
                columns: table => new
                {
                    OptionForQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsForQuestions", x => x.OptionForQuestionId);
                    table.ForeignKey(
                        name: "FK_OptionsForQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptionForQuestion_Answer",
                columns: table => new
                {
                    AnswerId = table.Column<int>(type: "int", nullable: false),
                    OptionForQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionForQuestion_Answer", x => new { x.AnswerId, x.OptionForQuestionId });
                    table.ForeignKey(
                        name: "FK_OptionForQuestion_Answer_Answers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answers",
                        principalColumn: "AnswerId");
                    table.ForeignKey(
                        name: "FK_OptionForQuestion_Answer_OptionsForQuestions_OptionForQuestionId",
                        column: x => x.OptionForQuestionId,
                        principalTable: "OptionsForQuestions",
                        principalColumn: "OptionForQuestionId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_CorrespondingFormFormId",
                table: "Answers",
                column: "CorrespondingFormFormId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionForQuestion_Answer_OptionForQuestionId",
                table: "OptionForQuestion_Answer",
                column: "OptionForQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OptionsForQuestions_QuestionId",
                table: "OptionsForQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_FormForWhichCorrespondFormId",
                table: "Questions",
                column: "FormForWhichCorrespondFormId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptionForQuestion_Answer");

            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "OptionsForQuestions");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Forms");
        }
    }
}
