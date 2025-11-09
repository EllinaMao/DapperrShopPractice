using Dapper;
using DapperrShopPractice.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Z.Dapper.Plus;
using static Dapper.SqlMapper;

namespace DapperrShopPractice
{
    /// <summary>
    ///
    ///Part 1
    ///
    ///1. SELECT — отримати всі товари разом із категоріями
    ///2. INSERT — додати новий товар Name = "Mouse", Price = 25.50, CategoryId = 1
    ///3. UPDATE — оновити ціну товару set Price = 27.99 where Name = "Mouse"
    ///4. DELETE — видалити товар Name = "Mouse"
    ///
    ///Part 2 
    ///
    ///1. Отримати всі товари з назвами їхніх категорій
    ///2. Отримати всі категорії та кількість товарів у кожній
    ///   SELECT з GROUP BY.
    ///3. Отримати всі замовлення з іменем покупця, датою та списком товарів у замовленні
    ///   SELECT з JOIN між Orders, Customers, OrderProducts, Products.
    ///4. Створити нове замовлення для наявного покупця
    ///   INSERT INTO Orders і INSERT INTO OrderProducts.
    ///5. Отримати всіх покупців і кількість їхніх замовлень
    ///   SELECT з JOIN і GROUP BY.
    ///6. Отримати загальну суму кожного замовлення (сума = ціна * кількість)
    ///   SELECT з JOIN і агрегатною функцією SUM.
    ///7. Знайти найдорожчий товар і покупця, який його замовив
    ///   SELECT з JOIN, MAX.
    ///   
    /// </summary>


    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=DESKTOP-BDMPPLC\\SQLEXPRESS;Database=DapperShop;Trusted_Connection=True;TrustServerCertificate=True;";
            using var connection = new SqlConnection(connectionString);

            connection.Open();

            #region part1
            //var crud = new Crud();
            ////2.INSERT — додати новий товар Name = "Mouse", Price = 25.50, CategoryId = 1
            //{
            //    var newProduct = new Product
            //    {
            //        Name = "Mouse",
            //        Price = 25.50m,
            //        CategoryId = 1
            //    };

            //    int insertedRows = crud.Insert(newProduct);
            //    Console.WriteLine(insertedRows > 0 ? "Продукт добавлен" : "Ошибка добавления");

            //    //3.UPDATE — оновити ціну товару set Price = 27.99 where Name = "Mouse"
            //    newProduct.Price = 27.99m;
            //    var updatedProduct = crud.Update(newProduct);
            //    if (updatedProduct != null)
            //    {
            //        Console.WriteLine($"Цена обновлена. Новый продукт: {updatedProduct.Name}, {updatedProduct.Price}");
            //    }
            //    else
            //    {
            //        Console.WriteLine("Ошибка обновления");
            //    }

            //    //1.SELECT — отримати всі товари разом із категоріями
            //    var products = crud.SelectWithCategoryList();
            //    Console.WriteLine("Все продукты с категориями:");
            //    foreach (var p in products)
            //    {
            //        Console.WriteLine($"{p.Id} | {p.Name} | {p.Price} | {p.Category.Name} | {p.CategoryId}");
            //    }

            //    //4.DELETE — видалити товар Name = "Mouse"
            //    bool deleted = crud.DeleteByName("Mouse");
            //    Console.WriteLine(deleted ? "Продукт удален" : "Ошибка удаления");

            //    ////1. Отримати всі товари з назвами їхніх категорій
            //    var products2 = crud.SelectWithCategory();
            //    Console.WriteLine("Все продукты с категориями:");
            //    foreach (var p in products2)
            //    {
            //        Console.WriteLine($"{p.Id} | {p.Name} | {p.Price} | {p.Category}");
            //    }
            #endregion

            #region part2
            int customerId = 1;
            int ProductId1 = 1;
            int productId2 = 2;


            using (IDbConnection dbConnection = new SqlConnection(connectionString))
            {
                var repository = new DapperPart2(dbConnection);
                //1
                Console.WriteLine("Товары и категории");
                try
                {
                    var products = repository.GetProductsWithCategories();
                    foreach (var p in products.Take(5))
                    {

                        Console.WriteLine($"Товар: {p.ProductName}, Категория: {p.CategoryName}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                //2
                Console.WriteLine("Категориии и количество товаров");
                try
                {
                    var category = repository.GetCategoryProductCounts();
                    foreach (var c in category)
                    {

                        Console.WriteLine($"Категория: {c.CategoryName}, Категория: {c.ProductCount}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
                //3.
                Console.WriteLine("Заказы с именем покупателя, датой и списком товаров");
                try
                {
                    var orders = repository.GetOrderDetails();
                    foreach (var o in orders)
                    {
                        Console.WriteLine($"  Заказ: {o.OrderId} - {o.CustomerName} - Товар: {o.ProductName} - {o.Quantity}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }


                //4
                Console.WriteLine("Новый заказ");

                //анонимный
                var itemsToOrder = new[]
                                {
                    new { ProductId = ProductId1, Quantity = 2 },
                    new { ProductId = productId2, Quantity = 1 }
                };

                try
                {
                    int newOrderId = repository.CreateNewOrder(customerId, itemsToOrder);
                    Console.WriteLine($"Успешно создан новый заказ с ID: {newOrderId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }

                // 5.
                Console.WriteLine("Количество заказов покупателей");
                try
                {
                    var customerCounts = repository.GetCustomerOrderCounts();
                    foreach (var c in customerCounts)
                    {
                        Console.WriteLine($"Покупатель: {c.CustomerName}, Заказы: {c.OrderCount}");
                    }
                }

                catch (Exception ex) { Console.WriteLine($"{ex.Message}"); }
                // 6
                Console.WriteLine("Общая сумма заказов");
                try
                {
                    var totals = repository.GetOrderTotals();
                    foreach (var t in totals.Take(5))
                    {
                        Console.WriteLine($"Заказ ID: {t.OrderId}, Сумма: {t.TotalAmount}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }

            #endregion

            #region Закоментировано




            //  Console.WriteLine(" Товари ");

            //  // 1. SELECT — отримати всі товари разом із категоріями


            //var sql1 = @"SELECT p.Id, p.Name, p.Price, c.Name AS CategoryName FROM Products p JOIN Categories c ON p.CategoryId = c.Id";

            //var products = connection.Query(sql1);///<dynamic> з полями Id, Name, Price, CategoryName 

            //foreach (var p in products)
            //{

            //    Console.WriteLine($"ProductId:{p.Id} Product: {p.Name} ProductPrice: {p.Price} CategoryName: ({p.CategoryName})");

            //}



            /////////////////////////////////////////////////
            ///Error!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            //var sql1 = @"SELECT p.Id, p.Name, p.Price, c.Name AS Category FROM Products p JOIN Categories c ON p.CategoryId = c.Id";///<dynamic> з полями Id, Name, Price, CategoryName 

            //List<Product> products = connection.Query<Product>(sql1).ToList();

            //foreach (var p in products)
            //{
            //    Console.WriteLine($"{p.Id}: {p.Name} - {p.Price} ({p.Category.Name})");
            //}


            ////////////////////////////////////////////////
            ///

            //var sql1 = @"SELECT  p.Id, p.Name, p.CategoryId, c.Id,  FROM Products p JOIN Categories c ON p.CategoryId = c.Id";

            //var products = connection.Query<Product, Category, Product>(
            //    sql1,
            //    (product, category) =>
            //    {
            //        product.Category = category;
            //        return product;
            //    },
            //    splitOn: "id"
            //).ToList();

            //foreach (var pr in products)
            //{
            //    Console.WriteLine($"Product: {pr.Name,-20}, Category: {pr.Category.Name,-20} Price: {pr.Price,-10} CategoryId: {pr.CategoryId,-10} CategoryId2:{pr.Category.Id,-10}  ");
            //}



            //  Console.WriteLine("\nДодавання товару ");

            //  // 2. INSERT — додати новий товар


            //var insertSql = "INSERT INTO Products (Name, Price, CategoryId) VALUES (@Name, @Price, @CategoryId)";
            //connection.Execute(insertSql, new { @Name = "Mouse", @Price = 25.50, @CategoryId = 1 });





            //  // 3. UPDATE — оновити ціну товару
            //var updateSql = "UPDATE Products SET Price = @Price WHERE Name = @Name";
            //connection.Execute(updateSql, new { @Price = 27.99, @Name = "Mouse" });

            //  Console.WriteLine("Ціну оновлено");

            //  Console.WriteLine("\n Видалення товару ");

            //  // 4. DELETE — видалити товар
            //var deleteSql = "DELETE FROM Products WHERE Name = @Name";
            //connection.Execute(deleteSql, new { @Name = "Mouse" });

            //////dynamic type

            //  Console.WriteLine("\nЗамовлення з товарами ");

            //  // 5. JOIN: отримати всі замовлення з покупцями та товарами
            //var sqlOrders = @"SELECT o.Id AS OrderId, o.OrderDate, c.FullName, pr.Name AS Product, op.Quantity FROM Orders o JOIN Customers c ON o.CustomerId = c.Id JOIN OrderProducts op ON o.Id = op.OrderId JOIN Products pr ON op.ProductId = pr.Id ORDER BY o.Id";

            //var orders = connection.Query(sqlOrders);

            //foreach (var o in orders)
            //{
            //    Console.WriteLine($"Замовлення {o.OrderId} від {o.FullName} ({o.OrderDate}): {o.Product} x{o.Quantity}");
            //}

            ///6. 
            //var sql = "SELECT COUNT(*) FROM Products";
            //var count = connection.ExecuteScalar(sql);
            //Console.WriteLine($"Total products: {count}");

            //connection.Close();

            //7.
            // sql = "SELECT * FROM Products WHERE Id = @productID";
            //var product = connection.QuerySingle(sql, new { @productID = 1 });
            //Console.WriteLine($"ProductID: {product.Id}; Name: {product.Name}");

            //8.
            //var sql = "SELECT * FROM Products WHERE Id = @productID";
            //Product product_ = connection.QuerySingle<Product>(sql, new { @productID = 1 });
            //Console.WriteLine($"ProductID: {product_.Id}; Name: {product_.Name}");


            ///9.

            //var products = new List<Product>{
            //new Product { Name = "Tablet", Price = 299.99M, CategoryId = 1 },
            //new Product { Name = "E-Reader", Price = 129.99M, CategoryId = 2 },
            //new Product { Name = "Sneakers", Price = 89.99M, CategoryId = 3 }
            // };


            //object value = connection.BulkInsert(products);

            //10

            //    var productsToDelete = connection.Query<Product>(
            //"SELECT * FROM Products WHERE Price < 20").ToList();

            //    connection.BulkDelete(productsToDelete);

            //11  

            //var produc = connection.Query<Product>("SELECT * FROM Products").ToList();
            //produc.ForEach(p => p.Price *= 1.1M); // підвищення ціни на 10%
            //connection.BulkUpdate(produc);
            #endregion



            #region lastHomework
            //        var crud1 = new Crud();
            //        var prod = crud1.SelectWithCategory();
            //        foreach (var item in prod)
            //        {
            //            Console.WriteLine(item);
            //        }
            //        ////1.ДЗ Підвищит ціну продукту який має найменшу кількість продажів
            //        var sql = @"
            //        UPDATE Products
            //        SET Price = Price * 1.1
            //        WHERE Id = (
            //            SELECT TOP 1 ProductId
            //            FROM OrderProducts
            //            GROUP BY ProductId
            //            ORDER BY COUNT(*) ASC
            //            );
            //        ";

            //        // Выполняем запрос
            //        int affectedRows = connection.Execute(sql);
            //        Console.WriteLine($"Обновлено {affectedRows} продукт(ов)");

            //        Console.WriteLine("Цена смартфорна увеличилась");
            //        var product = connection.QuerySingleOrDefault(@"
            //        SELECT p.Id, p.Name, p.Price, c.Id AS CategoryId, c.Name AS CategoryName
            //        FROM Products p
            //        JOIN Categories c ON p.CategoryId = c.Id
            //        WHERE p.Id = (
            //        SELECT TOP 1 ProductId
            //        FROM OrderProducts
            //        GROUP BY ProductId
            //        ORDER BY COUNT(*) ASC
            //    );
            //");

            //        if (product != null)//я пока писала запрос замучалась с ошибками
            //        {
            //            Console.WriteLine($"Продукт с минимальными продажами после повышения цены:");
            //            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, Category: {product.CategoryName}");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Продукт не найден.");
            //        }
            //        //// 2.  1ДЗ  в процедурі
            //        connection.Execute("EXEC IncreaseTheLowestSellingProductPrice");
            //        Console.WriteLine("Цена смартфорна увеличилась");
            //        product = connection.QuerySingleOrDefault(@"
            //        SELECT p.Id, p.Name, p.Price, c.Id AS CategoryId, c.Name AS CategoryName
            //        FROM Products p
            //        JOIN Categories c ON p.CategoryId = c.Id
            //        WHERE p.Id = (
            //        SELECT TOP 1 ProductId
            //        FROM OrderProducts
            //        GROUP BY ProductId
            //        ORDER BY COUNT(*) ASC
            //    );
            //");

            //        if (product != null)
            //        {
            //            Console.WriteLine($"Продукт с минимальными продажами после повышения цены:");
            //            Console.WriteLine($"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}, Category: {product.CategoryName}");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Продукт не найден.");
            //        }
            //        //// 3. Тригери на видалення об'єктів таблиць: видалені об'єкти переносяться в таблицю видалених об'єктів
            //        connection.Execute("DELETE FROM Products WHERE Id = @Id", new { Id = 1 });
            //        var deletedProducts = connection.Query("SELECT * FROM DeletedProducts");
            //        foreach (var p in deletedProducts)
            //        {
            //            Console.WriteLine($"{p.Id} | {p.Name} | {p.Price} | {p.DeletedAt}");
            //        }

            //        connection.Close();
            #endregion


        }
    }
}

