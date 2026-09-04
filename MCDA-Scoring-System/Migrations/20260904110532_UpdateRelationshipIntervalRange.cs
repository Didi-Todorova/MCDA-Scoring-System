using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCDA_Scoring_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationshipIntervalRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NumericRanges_NumericalCriterionRules_NumericalCriterionRuleId",
                table: "NumericRanges");

            migrationBuilder.RenameColumn(
                name: "NumericalCriterionRuleId",
                table: "NumericRanges",
                newName: "CriterionNumericalRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_NumericRanges_NumericalCriterionRuleId",
                table: "NumericRanges",
                newName: "IX_NumericRanges_CriterionNumericalRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_NumericRanges_NumericalCriterionRules_CriterionNumericalRuleId",
                table: "NumericRanges",
                column: "CriterionNumericalRuleId",
                principalTable: "NumericalCriterionRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NumericRanges_NumericalCriterionRules_CriterionNumericalRuleId",
                table: "NumericRanges");

            migrationBuilder.RenameColumn(
                name: "CriterionNumericalRuleId",
                table: "NumericRanges",
                newName: "NumericalCriterionRuleId");

            migrationBuilder.RenameIndex(
                name: "IX_NumericRanges_CriterionNumericalRuleId",
                table: "NumericRanges",
                newName: "IX_NumericRanges_NumericalCriterionRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_NumericRanges_NumericalCriterionRules_NumericalCriterionRuleId",
                table: "NumericRanges",
                column: "NumericalCriterionRuleId",
                principalTable: "NumericalCriterionRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
