using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DapperrShopPractice.Migrations
{
    /// <inheritdoc />
    public partial class IHateThisMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE TRIGGER trg_Products_Delete
            ON Products
            AFTER DELETE
            AS
            BEGIN
                INSERT INTO DeletedProducts (Id, Name, Price, CategoryId, DeletedAt)
                SELECT Id, Name, Price, CategoryId, GETDATE()
                FROM deleted;
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
