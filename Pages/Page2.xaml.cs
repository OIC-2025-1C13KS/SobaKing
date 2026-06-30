using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SobaKing.Pages
{
    public partial class Page2 : UserControl
    {

        private string selectedImagePath = "";


        public Page2()
        {
            InitializeComponent();
        }

        // Browse Image
        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog dialog = new OpenFileDialog();


            dialog.Filter =
                "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (dialog.ShowDialog() == true)
            {

                selectedImagePath = dialog.FileName;


                txtImage.Text =
                    Path.GetFileName(dialog.FileName);

            }

        }

        // Add Menu
        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Enter menu name");
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Invalid price");
                    return;
                }

                // Category Check
                if (cmbCategory.SelectedItem == null)
                {
                    MessageBox.Show("Select Category");
                    return;
                }

                string category =
                    (cmbCategory.SelectedItem as ComboBoxItem)
                    .Content.ToString();
                string imagePath = "";

                if (!string.IsNullOrEmpty(selectedImagePath))
                {

                    string imagesFolder =
                        Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            "Images");


                    if (!Directory.Exists(imagesFolder))
                    {
                        Directory.CreateDirectory(imagesFolder);
                    }

                    string fileName =
                        Path.GetFileName(selectedImagePath);

                    string destination =
                        Path.Combine(
                            imagesFolder,
                            fileName);

                    File.Copy(
                        selectedImagePath,
                        destination,
                        true);
                    imagePath = Path.Combine("Images",fileName);

                }
                // Save Database
                DatabaseHelper.AddMenuItem(
                    txtName.Text,
                    price,
                    imagePath,
                    category
                );

                MessageBox.Show(
                    "Menu Added Successfully");
                // Clear
                txtName.Clear();

                txtPrice.Clear();

                txtImage.Clear();

                cmbCategory.SelectedIndex = -1;


                selectedImagePath = "";

            }
            catch (Exception ex)
            {

                MessageBox.Show(
                    ex.Message);

            }

        }

    }
}