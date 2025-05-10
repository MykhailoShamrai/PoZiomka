using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class InitProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptionForQuestion_Answer_Answers_AnswerId",
                table: "OptionForQuestion_Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionForQuestion_Answer_OptionsForQuestions_OptionForQuestionId",
                table: "OptionForQuestion_Answer");

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoommatesIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Statuses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminStatus = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proposals_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_RoomId",
                table: "Proposals",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_OptionForQuestion_Answer_Answers_AnswerId",
                table: "OptionForQuestion_Answer",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "AnswerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OptionForQuestion_Answer_OptionsForQuestions_OptionForQuestionId",
                table: "OptionForQuestion_Answer",
                column: "OptionForQuestionId",
                principalTable: "OptionsForQuestions",
                principalColumn: "OptionForQuestionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OptionForQuestion_Answer_Answers_AnswerId",
                table: "OptionForQuestion_Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_OptionForQuestion_Answer_OptionsForQuestions_OptionForQuestionId",
                table: "OptionForQuestion_Answer");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.AddForeignKey(
                name: "FK_OptionForQuestion_Answer_Answers_AnswerId",
                table: "OptionForQuestion_Answer",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_OptionForQuestion_Answer_OptionsForQuestions_OptionForQuestionId",
                table: "OptionForQuestion_Answer",
                column: "OptionForQuestionId",
                principalTable: "OptionsForQuestions",
                principalColumn: "OptionForQuestionId");
        }
    }
}
