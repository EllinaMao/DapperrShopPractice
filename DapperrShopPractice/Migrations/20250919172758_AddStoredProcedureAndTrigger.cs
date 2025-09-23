using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DapperrShopPractice.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedureAndTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// <summary>
            ///1.ДЗ Підвищит ціну продукту який має найменшу кількість продажів
            /// 2.  1ДЗ  в процедурі
            /// пишу процедуру в Sql с увеличением цены на продукт, в которой создаю подзапрос где выбираю айдишник продукта с самым маденький количеством продаж
            /// ASC - Ascending)
            /// </summary>  
            // Выполняем SQL через миграцию EF Core
            migrationBuilder.Sql(@"
            CREATE PROCEDURE IncreaseTheLowestSellingProductPrice
            AS
            BEGIN
                UPDATE Products
                SET Price = Price *1.1
                WHERE Id = (
                SELECT TOP 1 ProductId
                FROM OrderProducts
                GROUP BY ProductId
                ORDER BY COUNT (*) ASC);
                END"
            );


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
