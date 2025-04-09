using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TqPerformanceEvaluationHr.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToEvaluationCycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "EvaluationCycles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "EvaluationCycles");
        }
    }
}
