    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    namespace SobaKing
    {
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (clickedButton != null)
            {
                string itemName = clickedButton.Content.ToString();
                string itemPrice = clickedButton.Tag.ToString();

                OrderList.Items.Add(itemName);

                int currentTotal = int.Parse(TotalText.Text.TrimStart('¥'));
                int newTotal = currentTotal + int.Parse(itemPrice);
                TotalText.Text = $"¥{newTotal}";
            }
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thank you for your payment!",
                            "Payment Successful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            OrderList.Items.Clear();
            TotalText.Text = "¥0";
        }
    }
}
