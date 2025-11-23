using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DecimalFieldsHandled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grade",
                type: "decimal(8,4)",
                precision: 8,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "WeightPercentage",
                table: "Assignment",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MarksObtained",
                table: "Grade",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,4)",
                oldPrecision: 8,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "WeightPercentage",
                table: "Assignment",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2);
        }
    }
}
