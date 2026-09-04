using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCDA_Scoring_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipCriterion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NumericalCriterionRules_CriterionId",
                table: "NumericalCriterionRules");

            migrationBuilder.CreateIndex(
                name: "IX_NumericalCriterionRules_CriterionId",
                table: "NumericalCriterionRules",
                column: "CriterionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NumericalCriterionRules_CriterionId",
                table: "NumericalCriterionRules");

            migrationBuilder.CreateIndex(
                name: "IX_NumericalCriterionRules_CriterionId",
                table: "NumericalCriterionRules",
                column: "CriterionId");
        }
    }
}
