using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperrShopPractice
{
    internal class DapperPart2
    {
        private readonly IDbConnection _connection;

        public DapperPart2(IDbConnection connection)
        {
            _connection = connection;
        }
        /*1.Отримати всі товари з назвами їхніх категорій
•	SELECT з JOIN між Products та Categories.
*/
        public IEnumerable<dynamic> GetProductsWithCategories()
        {
            string sql = @"
                SELECT
                    p.Name AS ProductName,
                    c.Name AS CategoryName
                FROM
                    Products AS p
                JOIN
                    Categories AS c ON p.CategoryId = c.Id;";

            return _connection.Query(sql);
        }

        /*2.	Отримати всі категорії та кількість товарів у кожній
•	SELECT з GROUP BY.
         */
        public IEnumerable<dynamic> GetCategoryProductCounts()
        {
            string sql = @"
                SELECT
                    c.Name AS CategoryName,
                    COUNT(p.Id) AS ProductCount
                FROM
                    Categories AS c
                LEFT JOIN
                    Products AS p ON c.Id = p.CategoryId
                GROUP BY
                    c.Name;";

            return _connection.Query(sql);
        }

        /*Отримати всі замовлення з іменем покупця, датою та списком товарів у замовленні
        •	SELECT з JOIN між Orders, Customers, OrderProducts, Products.
        */
        public IEnumerable<dynamic> GetOrderDetails()
        {
            string sql = @"
                SELECT
                    o.Id AS OrderId,
                    c.FullName AS CustomerName,
                    o.OrderDate,
                    p.Name AS ProductName,
                    op.Quantity
                FROM
                    Orders AS o
                JOIN
                    Customers AS c ON o.CustomerId = c.Id
                JOIN
                    OrderProducts AS op ON o.Id = op.OrderId
                JOIN
                    Products AS p ON op.ProductId = p.Id
                ORDER BY
                    o.Id, p.Name;";

            return _connection.Query(sql);
        }

        /*.	Створити нове замовлення для наявного покупця
•	INSERT INTO Orders і INSERT INTO OrderProducts*/
        public int CreateNewOrder(int customerId, IEnumerable<dynamic> items)
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }

            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    string insertOrderSql = @"
                        INSERT INTO Orders (CustomerId)
                        OUTPUT INSERTED.Id
                        VALUES (@CustomerId);";

                    int newOrderId = _connection.ExecuteScalar<int>(
                        insertOrderSql,
                        new { CustomerId = customerId },
                        transaction
                    );

                    string insertOrderProductsSql = @"
                        INSERT INTO OrderProducts (OrderId, ProductId, Quantity)
                        VALUES (@OrderId, @ProductId, @Quantity);";

                    foreach (var item in items)
                    {
                        _connection.Execute(
                            insertOrderProductsSql,
                            new
                            {
                                OrderId = newOrderId,
                                item.ProductId,
                                item.Quantity
                            },
                            transaction
                        );
                    }

                    transaction.Commit();
                    return newOrderId;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /*	Отримати всіх покупців і кількість їхніх замовлень
        •	SELECT з JOIN і GROUP BY.*/
        public IEnumerable<dynamic> GetCustomerOrderCounts()
        {
            string sql = @"
                SELECT
                    c.FullName AS CustomerName,
                    COUNT(o.Id) AS OrderCount
                FROM
                    Customers AS c
                LEFT JOIN
                    Orders AS o ON c.Id = o.CustomerId
                GROUP BY
                    c.Id, c.FullName
                ORDER BY
                    CustomerName;";

            return _connection.Query(sql);
        }

        /*.	Отримати загальну суму кожного замовлення (сума = ціна * кількість)
        •	SELECT з JOIN і агрегатною функцією SUM.
        */
        public IEnumerable<dynamic> GetOrderTotals()
        {
            string sql = @"
                SELECT
                    o.Id AS OrderId,
                    ISNULL(SUM(p.Price * op.Quantity), 0) AS TotalAmount
                FROM
                    Orders AS o
                LEFT JOIN
                    OrderProducts AS op ON o.Id = op.OrderId
                LEFT JOIN
                    Products AS p ON op.ProductId = p.Id
                GROUP BY
                    o.Id
                ORDER BY
                    o.Id;";

            return _connection.Query(sql);
        }

        /*Знайти найдорожчий товар і покупця, який його замовив
SELECT з JOIN, MAX.
*/
        public IEnumerable<dynamic> GetCustomersOfMostExpensiveProduct()
        {
            string sql = @"
                SELECT DISTINCT
    c.FullName AS CustomerName,
    p.Name AS ProductName,
    p.Price
FROM
    Customers AS c
JOIN
    Orders AS o ON c.Id = o.CustomerId
JOIN
    OrderProducts AS op ON o.Id = op.OrderId
JOIN
    Products AS p ON op.ProductId = p.Id
WHERE
    p.Price = (SELECT MAX(Price) FROM Products);";

            return _connection.Query(sql);
        }
    }
}

