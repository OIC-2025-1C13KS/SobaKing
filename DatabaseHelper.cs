using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace SobaKing
{
    public static class DatabaseHelper
    {
        private static string dbPath = "SobaKing.db";
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        // ============================
        // INITIALIZE DATABASE
        // ============================
        public static void InitializeDatabase()
        {
            if (!File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string createMenuTable = @"
                CREATE TABLE IF NOT EXISTS MenuItems
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price INTEGER NOT NULL,
                    ImageUrl TEXT,
                    Category TEXT
                );";

                string createOrderTable = @"
                CREATE TABLE IF NOT EXISTS Orders
                (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Price INTEGER NOT NULL,
                    Quantity INTEGER NOT NULL,
                    OrderDate TEXT NOT NULL,
                    ImageUrl TEXT
                );";

                new SQLiteCommand(createMenuTable, conn).ExecuteNonQuery();
                new SQLiteCommand(createOrderTable, conn).ExecuteNonQuery();

                AddCategoryColumn(conn);
                FixOldImagePaths(conn);   // ★ auto-fix old absolute paths
            }
        }

        // ============================
        // ADD CATEGORY COLUMN
        // ============================
        private static void AddCategoryColumn(SQLiteConnection conn)
        {
            try
            {
                string sql = "SELECT Category FROM MenuItems LIMIT 1";
                new SQLiteCommand(sql, conn).ExecuteScalar();
            }
            catch
            {
                string alter = "ALTER TABLE MenuItems ADD COLUMN Category TEXT";
                new SQLiteCommand(alter, conn).ExecuteNonQuery();
            }
        }

        // ============================
        // AUTO FIX OLD ABSOLUTE PATHS
        // ============================
        private static void FixOldImagePaths(SQLiteConnection conn)
        {
            string q = "SELECT Id, ImageUrl FROM MenuItems";

            using (var cmd = new SQLiteCommand(q, conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    int id = Convert.ToInt32(r["Id"]);
                    string oldPath = r["ImageUrl"].ToString();

                    string fileName = Path.GetFileName(oldPath);
                    string newRelative = $"Images/{fileName}";

                    string update = "UPDATE MenuItems SET ImageUrl=@i WHERE Id=@id";

                    using (var cmd2 = new SQLiteCommand(update, conn))
                    {
                        cmd2.Parameters.AddWithValue("@i", newRelative);
                        cmd2.Parameters.AddWithValue("@id", id);
                        cmd2.ExecuteNonQuery();
                    }
                }
            }
        }

        // ============================
        // LOAD MENU ITEMS
        // ============================
        public static List<MenuItemModel> LoadMenuItems()
        {
            InitializeDatabase();

            List<MenuItemModel> list = new List<MenuItemModel>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string q = "SELECT * FROM MenuItems";

                using (var cmd = new SQLiteCommand(q, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string relative = r["ImageUrl"]?.ToString() ?? "";

                        // project root (not bin\Debug)
                        string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string fullPath = Path.Combine(projectRoot, relative);

                       

                        list.Add(new MenuItemModel
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Name = r["Name"].ToString(),
                            Price = Convert.ToInt32(r["Price"]),
                            ImageUrl = fullPath,
                            Category = r["Category"]?.ToString() ?? "",
                            Quantity = 1
                        });
                    }
                }
            }

            return list;
        }

        // ============================
        // ADD MENU ITEM
        // ============================
        public static void AddMenuItem(string name, decimal price, string imagePath, string category)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string relativePath = $"Images/{Path.GetFileName(imagePath)}";

                string q = @"INSERT INTO MenuItems (Name, Price, ImageUrl, Category)
                             VALUES(@n, @p, @i, @c)";

                using (var cmd = new SQLiteCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@n", name);
                    cmd.Parameters.AddWithValue("@p", Convert.ToInt32(price));
                    cmd.Parameters.AddWithValue("@i", relativePath);
                    cmd.Parameters.AddWithValue("@c", category);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================
        // INSERT MENU ITEM
        // ============================
        public static void InsertMenuItem(MenuItemModel item)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string relativePath = $"Images/{Path.GetFileName(item.ImageUrl)}";

                string q = @"INSERT INTO MenuItems (Name, Price, ImageUrl, Category)
                             VALUES(@n, @p, @i, @c)";

                using (var cmd = new SQLiteCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@n", item.Name);
                    cmd.Parameters.AddWithValue("@p", item.Price);
                    cmd.Parameters.AddWithValue("@i", relativePath);
                    cmd.Parameters.AddWithValue("@c", item.Category);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================
        // DELETE MENU ITEM
        // ============================
        public static void DeleteMenuItem(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string q = "DELETE FROM MenuItems WHERE Id=@id";

                using (var cmd = new SQLiteCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================
        // DELETE ALL MENU ITEMS
        // ============================
        public static void DeleteAllMenuItems()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string q = "DELETE FROM MenuItems";
                new SQLiteCommand(q, conn).ExecuteNonQuery();
            }
        }

        // ============================
        // SAVE ORDER
        // ============================
        public static void SaveOrder(MenuItemModel item)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string relativePath = $"Images/{Path.GetFileName(item.ImageUrl)}";

                string q = @"INSERT INTO Orders (Name, Price, Quantity, OrderDate, ImageUrl)
                             VALUES(@n, @p, @q, @d, @i)";

                using (var cmd = new SQLiteCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@n", item.Name);
                    cmd.Parameters.AddWithValue("@p", item.Price);
                    cmd.Parameters.AddWithValue("@q", item.Quantity);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@i", relativePath);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ============================
        // LOAD ORDERS
        // ============================
        public static List<OrderHistoryModel> LoadOrders()
        {
            List<OrderHistoryModel> list = new List<OrderHistoryModel>();

            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                string q = "SELECT * FROM Orders ORDER BY Id ASC";

                using (var cmd = new SQLiteCommand(q, conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        string relative = r["ImageUrl"]?.ToString() ?? "";
                        string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                        string fullPath = Path.Combine(projectRoot, relative);

                        list.Add(new OrderHistoryModel
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            Name = r["Name"].ToString(),
                            Price = Convert.ToInt32(r["Price"]),
                            Quantity = Convert.ToInt32(r["Quantity"]),
                            OrderDate = r["OrderDate"].ToString(),
                            ImageUrl = fullPath
                        });
                    }
                }
            }

            return list;
        }
    }
}
