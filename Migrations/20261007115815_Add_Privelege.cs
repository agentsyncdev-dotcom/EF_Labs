using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_Lab.Migrations
{
    /// <inheritdoc />
    public partial class Add_Privelege : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Privelege",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privelege",
                table: "Customers");
        }
    }
}
