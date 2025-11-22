using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Playtesters.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoIpSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AccessValidationHistory",
                type: "TEXT COLLATE NOCASE",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AccessValidationHistory",
                type: "TEXT COLLATE NOCASE",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "IpGeoCache",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IpAddress = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpGeoCache", x => x.Id);
                });

            migrationBuilder.Sql(
                @"CREATE INDEX IF NOT EXISTS IX_AccessValidationHistory_City 
                ON AccessValidationHistory(City COLLATE NOCASE);");

            migrationBuilder.Sql(
                @"CREATE INDEX IF NOT EXISTS IX_AccessValidationHistory_Country 
                ON AccessValidationHistory(Country COLLATE NOCASE);");

            // migrationBuilder.CreateIndex(
            //    name: "IX_AccessValidationHistory_City",
            //    table: "AccessValidationHistory",
            //    column: "City");

            // migrationBuilder.CreateIndex(
            //    name: "IX_AccessValidationHistory_Country",
            //    table: "AccessValidationHistory",
            //    column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_IpGeoCache_IpAddress",
                table: "IpGeoCache",
                column: "IpAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpGeoCache");

            migrationBuilder.DropIndex(
                name: "IX_AccessValidationHistory_City",
                table: "AccessValidationHistory");

            migrationBuilder.DropIndex(
                name: "IX_AccessValidationHistory_Country",
                table: "AccessValidationHistory");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AccessValidationHistory");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AccessValidationHistory");
        }
    }
}
