using fishapp.Models;
using fishapp.Services;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace fishapp
{
    public partial class ProductAddWindow : Window
    {
        public ProductAddWindow()
        {
            InitializeComponent();
            LoadLogo();
            LoadCategories();
            LoadManufacturers();
            LoadSuppliers();
        }

        private void LoadLogo()
        {
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.png");
                if (File.Exists(logoPath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(logoPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    LogoImage.Source = bitmap;
                    LogoImage.Visibility = Visibility.Visible;
                }
            }
            catch
            {
                LogoImage.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadCategories()
        {
            using (FishContext db = new FishContext())
            {
                var categories = db.Categories.ToList();
                CategoryComboBox.ItemsSource = categories;
                CategoryComboBox.DisplayMemberPath = "Name";
                CategoryComboBox.SelectedValuePath = "Id";
            }
        }

        private void LoadManufacturers()
        {
            using (FishContext db = new FishContext())
            {
                var manufacturers = db.Proisvoditels.ToList();
                ManufacturerComboBox.ItemsSource = manufacturers;
                ManufacturerComboBox.DisplayMemberPath = "Name";
                ManufacturerComboBox.SelectedValuePath = "Id";
            }
        }

        private void LoadSuppliers()
        {
            using (FishContext db = new FishContext())
            {
                var suppliers = db.Postavshiks.ToList();
                SupplierComboBox.ItemsSource = suppliers;
                SupplierComboBox.DisplayMemberPath = "Name";
                SupplierComboBox.SelectedValuePath = "Id";
            }
        }

        private void BrowseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = dialog.FileName;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FishContext db = new FishContext())
                {
                    Product product = new Product();

                    product.Name = NameTextBox.Text.Trim();
                    product.Article = ArticleTextBox.Text.Trim();
                    
                    if (CategoryComboBox.SelectedValue != null && int.TryParse(CategoryComboBox.SelectedValue.ToString(), out int categoryId))
                        product.IdCategory = categoryId;
                    else
                        product.IdCategory = null;

                    if (ManufacturerComboBox.SelectedValue != null && int.TryParse(ManufacturerComboBox.SelectedValue.ToString(), out int manufacturerId))
                        product.IdProisvoditel = manufacturerId;
                    else
                        product.IdProisvoditel = null;

                    if (SupplierComboBox.SelectedValue != null && int.TryParse(SupplierComboBox.SelectedValue.ToString(), out int supplierId))
                        product.IdPostavshik = supplierId;
                    else
                        product.IdPostavshik = null;

                    if (int.TryParse(PriceTextBox.Text, out int price))
                        product.Price = price;
                    else
                        product.Price = null;

                    product.Unit = UnitTextBox.Text.Trim();

                    if (int.TryParse(CountTextBox.Text, out int count))
                        product.Count = count;
                    else
                        product.Count = null;

                    if (int.TryParse(SaleTextBox.Text, out int sale))
                        product.Sale = sale;
                    else
                        product.Sale = null;

                    if (int.TryParse(MaxSaleTextBox.Text, out int maxSale))
                        product.MaxSale = maxSale;
                    else
                        product.MaxSale = null;

                    product.Description = DescriptionTextBox.Text.Trim();
                    product.Imgpath = ImagePathTextBox.Text.Trim();

                    db.Products.Add(product);
                    db.SaveChanges();
                    MessageBox.Show("Товар успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
