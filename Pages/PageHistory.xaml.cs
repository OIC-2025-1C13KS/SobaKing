using System.Windows.Controls;

namespace SobaKing.Pages
{
    public partial class PageHistory : Page
    {
        public PageHistory()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            HistoryGrid.ItemsSource =
                DatabaseHelper.LoadOrders();
        }
    }
}