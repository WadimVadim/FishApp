using fishapp.Models;
using fishapp.Services;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace fishapp
{
    public partial class ProductEditWindow : Window
    {
        private Product? product;

        public ProductEditWindow(Product? product)
        {
            InitializeComponent();
            LoadLogo();
            this.product = product;
            LoadCategories();
            LoadManufacturers();
            LoadSuppliers();
            LoadProductData();
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

        private void LoadProductData()
        {
            if (product != null)
            {
                NameTextBox.Text = product.Name ?? "";
                ArticleTextBox.Text = product.Article ?? "";
                CategoryComboBox.SelectedValue = product.IdCategory;
                ManufacturerComboBox.SelectedValue = product.IdProisvoditel;
                SupplierComboBox.SelectedValue = product.IdPostavshik;
                PriceTextBox.Text = (product.Price ?? 0).ToString();
                UnitTextBox.Text = product.Unit ?? "";
                CountTextBox.Text = (product.Count ?? 0).ToString();
                SaleTextBox.Text = (product.Sale ?? 0).ToString();
                MaxSaleTextBox.Text = (product.MaxSale ?? 0).ToString();
                DescriptionTextBox.Text = product.Description ?? "";
                ImagePathTextBox.Text = product.Imgpath ?? "";
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
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Введите наименование товара.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using (FishContext db = new FishContext())
                {
                    Product productToSave;

                    if (product != null && product.Id > 0)
                    {
                        productToSave = db.Products.FirstOrDefault(p => p.Id == product.Id);
                        if (productToSave == null)
                        {
                            MessageBox.Show("Товар не найден в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        productToSave = new Product();
                        db.Products.Add(productToSave);
                    }

                    productToSave.Name = NameTextBox.Text.Trim();
                    productToSave.Article = ArticleTextBox.Text.Trim();
                    
                    if (CategoryComboBox.SelectedValue != null && int.TryParse(CategoryComboBox.SelectedValue.ToString(), out int categoryId))
                        productToSave.IdCategory = categoryId;
                    else
                        productToSave.IdCategory = null;

                    if (ManufacturerComboBox.SelectedValue != null && int.TryParse(ManufacturerComboBox.SelectedValue.ToString(), out int manufacturerId))
                        productToSave.IdProisvoditel = manufacturerId;
                    else
                        productToSave.IdProisvoditel = null;

                    if (SupplierComboBox.SelectedValue != null && int.TryParse(SupplierComboBox.SelectedValue.ToString(), out int supplierId))
                        productToSave.IdPostavshik = supplierId;
                    else
                        productToSave.IdPostavshik = null;

                    if (int.TryParse(PriceTextBox.Text, out int price))
                        productToSave.Price = price;
                    else
                        productToSave.Price = null;

                    productToSave.Unit = UnitTextBox.Text.Trim();

                    if (int.TryParse(CountTextBox.Text, out int count))
                        productToSave.Count = count;
                    else
                        productToSave.Count = null;

                    if (int.TryParse(SaleTextBox.Text, out int sale))
                        productToSave.Sale = sale;
                    else
                        productToSave.Sale = null;

                    if (int.TryParse(MaxSaleTextBox.Text, out int maxSale))
                        productToSave.MaxSale = maxSale;
                    else
                        productToSave.MaxSale = null;

                    productToSave.Description = DescriptionTextBox.Text.Trim();
                    productToSave.Imgpath = ImagePathTextBox.Text.Trim();

                    db.SaveChanges();
                    MessageBox.Show("Товар успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}\n\nДетали: {ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

