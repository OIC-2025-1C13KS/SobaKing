using System.Collections.ObjectModel;

namespace SobaKing
{
    public static class AppData
    {
        public static ObservableCollection<MenuItemModel> MenuItems
            = new ObservableCollection<MenuItemModel>();

        public static ObservableCollection<MenuItemModel> Cart
            = new ObservableCollection<MenuItemModel>();

        public static void LoadMenu()
        {
            MenuItems.Clear();

            var items = DatabaseHelper.LoadMenuItems();

            foreach (var item in items)
            {
                // 🔥 Category Normalize (VERY IMPORTANT)
                // "Soft Drink" → "SoftDrink"
                // "soft drink" → "SoftDrink"
                // "softdrink" → "SoftDrink"
                item.Category = item.Category
                    .Replace(" ", "")                // remove spaces
                    .Trim();                         // remove extra whitespace

                // Capitalize to match FilterByCategory("SoftDrink")
                item.Category = item.Category switch
                {
                    "drink" => "Drink",
                    "softdrink" => "SoftDrink",
                    "food" => "Food",
                    _ => item.Category
                };

                MenuItems.Add(item);
            }
        }

        public static void SaveMenu()
        {
            DatabaseHelper.DeleteAllMenuItems();

            foreach (var item in MenuItems)
            {
                DatabaseHelper.InsertMenuItem(item);
            }
        }
    }
}
