using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SobaKing
{
    public partial class Payment : UserControl
    {
        private readonly ObservableCollection<MenuItemModel> cart;
        private readonly int totalAmount;

        public Payment(ObservableCollection<MenuItemModel> cartItems)
        {
            InitializeComponent();

            cart = cartItems ?? new ObservableCollection<MenuItemModel>();

            totalAmount = cart.Sum(x => x.Price * x.Quantity);
            TotalText.Text = $"¥{totalAmount}";
        }
        private void SaveAllOrders()
        {
            foreach (MenuItemModel item in cart)
            {
                DatabaseHelper.SaveOrder(item);
            }
        }

        public Payment()
        {
        }

        private void PaidTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(PaidTextBox.Text, out int paid))
            {
                int change = paid - totalAmount;
                ChangeText.Text = $"¥{change}";
            }
            else
            {
                ChangeText.Text = "¥0";
            }
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
                PaidTextBox.Text += btn.Content?.ToString();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            PaidTextBox.Text = "";
            ChangeText.Text = "¥0";
        }

        private void Exact_Click(object sender, RoutedEventArgs e)
        {
            PaidTextBox.Text = totalAmount.ToString();
            ChangeText.Text = "¥0";
        }

        private void Cash_Click(object sender, RoutedEventArgs e)
        {
            SaveAllOrders();

            ReceiptPreviewWindow win =
          new ReceiptPreviewWindow(cart, "Cash");

            win.Show();
            Window.GetWindow(this)?.Close();
        }

        private void CreditCard_Click(object sender, RoutedEventArgs e)
        {
            SaveAllOrders();
            ReceiptPreviewWindow win =
         new ReceiptPreviewWindow(cart, "CreditCard");

            win.Show();
            Window.GetWindow(this)?.Close();
        }

        private void Emoney_Click(object sender, RoutedEventArgs e)
        {
            SaveAllOrders();
            ReceiptPreviewWindow win =
        new ReceiptPreviewWindow(cart, "Emoney");

            win.Show();
            Window.GetWindow(this)?.Close();
        }

        private void Qrcode_Click(object sender, RoutedEventArgs e)
        {
            SaveAllOrders();
            ReceiptPreviewWindow win =
         new ReceiptPreviewWindow(cart, "Qrcode");

            win.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
