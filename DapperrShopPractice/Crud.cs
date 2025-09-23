using DapperrShopPractice.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Collections.Generic;

namespace DapperrShopPractice
{
    public class Crud : Icrud<Product>
    {
        private readonly string _connectionString =
            "Server=DESKTOP-BDMPPLC\\SQLEXPRESS;Database=DapperShop;Trusted_Connection=True;TrustServerCertificate=True;";

        public bool Delete(Product entity)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "DELETE FROM Products WHERE Id = @Id";
            var rows = connection.Execute(sql, new { entity.Id });
            return rows > 0;

        }

        public int Insert(Product entity)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = @"INSERT INTO Products (Name, Price, CategoryId) 
                VALUES (@Name, @Price, @CategoryId);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
            var id = connection.QuerySingle<int>(sql, entity);
            entity.Id = id; 
            return id;
        }


            public List<Product> SelectWithCategoryList()
            {
                using var connection = new SqlConnection(_connectionString);
                var sql = @"
                    SELECT p.Id, p.Name, p.Price, p.CategoryId, c.Id, c.Name
                    FROM Products p
                    JOIN Categories c ON p.CategoryId = c.Id";
                List<Product> productsList = connection.Query<Product, Category, Product>(
                    sql,
                    (product, category) =>
                    {
                        product.Category = category;
                        return product;
                    },
                    splitOn: "Id"
                ).ToList();
                return productsList;
            }
        public IEnumerable<dynamic> SelectWithCategory()
        {
            using var connection = new SqlConnection(_connectionString);
            var products = connection.Query(@"
                SELECT p.Id, p.Name, p.Price, c.Name AS CategoryName, c.Id as CategoryId
                FROM Products p
                JOIN Categories c ON p.CategoryId = c.Id");
            return products.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Category = p.CategoryName,
                CategoryId = p.CategoryId,
            });
        }

        public Product? Update(Product entity)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "UPDATE Products SET Name = @Name, Price = @Price, CategoryId = @CategoryId WHERE Id = @Id";
            var rows = connection.Execute(sql, entity);
            if (rows > 0)
            {
                var selectSql = "SELECT * FROM Products WHERE Id = @Id";
                return connection.QueryFirstOrDefault<Product>(selectSql, new { entity.Id });
            }
            return null;
        }
        public bool DeleteByName(string name)
        {
            using var connection = new SqlConnection(_connectionString);
            var sql = "DELETE FROM Products WHERE Name = @Name";
            var rows = connection.Execute(sql, new { Name = name });
            return rows > 0;
        }
    }
}
