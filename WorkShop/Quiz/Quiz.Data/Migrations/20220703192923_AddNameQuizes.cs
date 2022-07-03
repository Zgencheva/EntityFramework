using Microsoft.EntityFrameworkCore.Migrations;

namespace Quiz.Data.Migrations
{
    public partial class AddNameQuizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_MyProperty_QuizId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_MyProperty_QuizId",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyProperty",
                table: "MyProperty");

            migrationBuilder.RenameTable(
                name: "MyProperty",
                newName: "Quizzes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Quizzes_QuizId",
                table: "UserAnswers",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Quizzes_QuizId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Quizzes_QuizId",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Quizzes",
                table: "Quizzes");

            migrationBuilder.RenameTable(
                name: "Quizzes",
                newName: "MyProperty");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyProperty",
                table: "MyProperty",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_MyProperty_QuizId",
                table: "Questions",
                column: "QuizId",
                principalTable: "MyProperty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_MyProperty_QuizId",
                table: "UserAnswers",
                column: "QuizId",
                principalTable: "MyProperty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
