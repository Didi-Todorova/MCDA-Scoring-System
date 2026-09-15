using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCDA_Scoring_System.Migrations
{
    /// <inheritdoc />
    public partial class RanlInInterval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "NumericRanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxValue",
                table: "NumericalCriterionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinValue",
                table: "NumericalCriterionRules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "NumericRanges");

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "NumericalCriterionRules");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "NumericalCriterionRules");
        }
    }
}
