using System.Windows;
using System.Windows.Input;

namespace SobaKing
{
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            MainWindow main = new MainWindow();

            main.Show();

            this.Close();
        }
    }
}