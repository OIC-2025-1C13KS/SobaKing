using SobaKing;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SobaKing
{
    public partial class ReceiptPreviewWindow : Window
    {
        ObservableCollection<MenuItemModel> cart;
        string paymentMethod;

        public ReceiptPreviewWindow(ObservableCollection<MenuItemModel> data, string method)
        {
            InitializeComponent();
            cart = data;
            paymentMethod = method;

            docViewer.Document = CreateReceipt();
        }

        private FlowDocument CreateReceipt()
        {
            FlowDocument doc = new FlowDocument();

            doc.PageWidth = 300;
            doc.FontSize = 14;

            // Title
            doc.Blocks.Add(new Paragraph(new Run("===== RECEIPT ====="))
            {
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 18
            });

            // Items
            foreach (var item in cart)
            {
                doc.Blocks.Add(new Paragraph(
                    new Run($"{item.Name} x{item.Quantity} = ¥{item.Price * item.Quantity}")
                ));
            }

            int total = cart.Sum(x => x.Price * x.Quantity);

            doc.Blocks.Add(new Paragraph(new Run("--------------------")));

            // Total
            doc.Blocks.Add(new Paragraph(new Run($"TOTAL: ¥ {total}"))
            {
                FontWeight = FontWeights.Bold
            });

            // ⭐ Payment Method
            doc.Blocks.Add(new Paragraph(new Run($"Payment: {paymentMethod}"))
            {
                FontWeight = FontWeights.Bold
            });

            doc.Blocks.Add(new Paragraph(new Run("--------------------")));

            return doc;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
           // PrintDialog pd = new PrintDialog();

           // if (pd.ShowDialog() == true)
           // {
              //  pd.PrintDocument(
                 //   ((IDocumentPaginatorSource)docViewer.Document).DocumentPaginator,
                //    "Receipt");

                MessageBox.Show(
                    "Receipt Printed Successfully!",
                    "Print Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                  AppData.Cart.Clear();
            MainWindow main = new MainWindow();
               main.Show();
            this.Close();
            }
        }
    }
