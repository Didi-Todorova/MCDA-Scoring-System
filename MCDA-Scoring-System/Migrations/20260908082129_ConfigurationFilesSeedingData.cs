using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MCDA_Scoring_System.Migrations
{
    /// <inheritdoc />
    public partial class ConfigurationFilesSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Decisions",
                columns: new[] { "Id", "CreatedAt", "Name", "UserId", "WeightingMethod" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Choose a Laptop", null, 1 },
                    { 2, new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Choose a University", null, 0 }
                });

            migrationBuilder.InsertData(
                table: "Alternatives",
                columns: new[] { "Id", "DecisionId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Laptop A" },
                    { 2, 1, "Laptop B" }
                });

            migrationBuilder.InsertData(
                table: "Criteria",
                columns: new[] { "Id", "CriterionType", "DecisionId", "Name", "Weight" },
                values: new object[,]
                {
                    { 1, 0, 1, "Price", 3.4m },
                    { 2, 0, 1, "RAM", 2.1m },
                    { 3, 0, 1, "Storage", 2.5m },
                    { 4, 1, 1, "Brand", 1.0m },
                    { 5, 1, 1, "Performance", 1.5m }
                });

            migrationBuilder.InsertData(
                table: "AlternativeValues",
                columns: new[] { "Id", "AlternativeId", "CriterionId", "CriterionOptionId", "NumericValue" },
                values: new object[,]
                {
                    { 1, 1, 1, null, 56m },
                    { 4, 1, 4, null, 1100m },
                    { 5, 1, 5, null, 880m },
                    { 6, 2, 1, null, 44m },
                    { 9, 2, 4, null, 1050m },
                    { 10, 2, 5, null, 15m }
                });

            migrationBuilder.InsertData(
                table: "CriterionOptions",
                columns: new[] { "Id", "CriterionId", "Rank", "Value" },
                values: new object[,]
                {
                    { 1, 2, 1, "Lenovo" },
                    { 2, 2, 2, "HP" },
                    { 3, 2, 3, "Mac" },
                    { 4, 3, 1, "Excellent" },
                    { 5, 3, 2, "Good" },
                    { 6, 3, 3, "Poor" }
                });

            migrationBuilder.InsertData(
                table: "NumericalCriterionRules",
                columns: new[] { "Id", "CriterionId", "Direction", "MaxValue", "MinValue", "NumericType", "TargetValue" },
                values: new object[,]
                {
                    { 1, 1, null, 128m, 16m, 2, 64m },
                    { 2, 4, 0, 1500m, 750m, 0, null },
                    { 3, 5, null, 1024m, 32m, 1, null }
                });

            migrationBuilder.InsertData(
                table: "AlternativeValues",
                columns: new[] { "Id", "AlternativeId", "CriterionId", "CriterionOptionId", "NumericValue" },
                values: new object[,]
                {
                    { 2, 1, 2, 1, null },
                    { 3, 1, 3, 5, null },
                    { 7, 2, 2, 3, null },
                    { 8, 2, 3, 5, null }
                });

            migrationBuilder.InsertData(
                table: "NumericRanges",
                columns: new[] { "Id", "CriterionNumericalRuleId", "MaxValue", "MinValue" },
                values: new object[,]
                {
                    { 1, 3, 1000.0m, 570m },
                    { 2, 3, 560m, 240m },
                    { 3, 3, 239m, 124m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AlternativeValues",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Decisions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NumericRanges",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NumericRanges",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NumericRanges",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NumericalCriterionRules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NumericalCriterionRules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Alternatives",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Alternatives",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Criteria",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Criteria",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CriterionOptions",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NumericalCriterionRules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Criteria",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Criteria",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Criteria",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Decisions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "MaxValue",
                table: "NumericalCriterionRules");

            migrationBuilder.DropColumn(
                name: "MinValue",
                table: "NumericalCriterionRules");
        }
    }
}
