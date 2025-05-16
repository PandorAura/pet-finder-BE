using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnimalAdoption.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdopted = table.Column<bool>(type: "bit", nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Species = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdopterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Users_AdopterId",
                        column: x => x.AdopterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "AdopterId", "Age", "ArrivalDate", "Breed", "Description", "IsAdopted", "Location", "Name", "Photo", "Species" },
                values: new object[] { 2, null, 2, new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tabby", "Playful tabby cat", false, "Chicago", "Mittens", "mittens.jpg", "Cat" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "FirstName", "LastName", "PhoneNumber", "RegistrationDate" },
                values: new object[,]
                {
                    { 1, "123 Main St", "john@example.com", "John", "Doe", "1234567890", new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "456 Oak Ave", "jane@example.com", "Jane", "Smith", "0987654321", new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "AdopterId", "Age", "ArrivalDate", "Breed", "Description", "IsAdopted", "Location", "Name", "Photo", "Species" },
                values: new object[,]
                {
                    { 1, 1, 3, new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Golden Retriever", "Friendly golden retriever", true, "New York", "Buddy", "buddy.jpg", "Dog" },
                    { 3, 2, 5, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Labrador", "Calm and gentle", true, "Los Angeles", "Rocky", "rocky.jpg", "Dog" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_AdopterId",
                table: "Animals",
                column: "AdopterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animals");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
