using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SobaKing.Pages
{
    public partial class Page1 : Page
    {
        private string currentCategory = "All";

        public Page1()
        {
            InitializeComponent();

            AppData.LoadMenu();

            ItemsControlMenu.ItemsSource = AppData.MenuItems;
            CartListBox.ItemsSource = AppData.Cart;

            UpdateTotal();

            SearchPlaceholder.Visibility = Visibility.Visible;
        }

        // CATEGORY FILTER
        public void FilterByCategory(string category)
        {
            currentCategory = category;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            string keyword = SearchBox.Text.Trim().ToLower();

            var filtered = AppData.MenuItems.AsEnumerable();

            // CATEGORY (case-insensitive + space remove)
            if (currentCategory != "All")
                filtered = filtered.Where(x =>
                    x.Category.Replace(" ", "")
                              .Equals(currentCategory, StringComparison.OrdinalIgnoreCase));

            // SEARCH
            if (!string.IsNullOrWhiteSpace(keyword))
                filtered = filtered.Where(x =>
                    x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            ItemsControlMenu.ItemsSource = filtered.ToList();
        }


        // SEARCH BOX
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            ApplyFilter();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                SearchPlaceholder.Visibility = Visibility.Visible;
        }

        // ADD TO CART
        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Image)?.DataContext is MenuItemModel item)
            {
                var exist = AppData.Cart.FirstOrDefault(x => x.Name == item.Name);

                if (exist != null)
                {
                    exist.Quantity++;
                }
                else
                {
                    AppData.Cart.Add(new MenuItemModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Price = item.Price,
                        ImageUrl = item.ImageUrl,
                        Quantity = 1,
                        Category = item.Category
                    });
                }

                CartListBox.Items.Refresh();
                UpdateTotal();
            }
        }

        // +
        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is MenuItemModel item)
            {
                item.Quantity++;
                CartListBox.Items.Refresh();
                UpdateTotal();
            }
        }

        // -
        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.DataContext is MenuItemModel item)
            {
                if (item.Quantity > 1)
                    item.Quantity--;
                else
                    AppData.Cart.Remove(item);

                CartListBox.Items.Refresh();
                UpdateTotal();
            }
        }

        // TOTAL
        private void UpdateTotal()
        {
            int total = AppData.Cart.Sum(x => x.Price * x.Quantity);
            TotalText.Text = $"Total : ¥ {total}";
        }

        // PAYMENT
        private void Payment_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Window
            {
                Title = "Payment",
                Width = 1200,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = new Payment(AppData.Cart)
            };

            win.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
