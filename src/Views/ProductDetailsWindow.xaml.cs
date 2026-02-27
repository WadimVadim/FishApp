using fishapp.Models;
using fishapp.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace fishapp
{
    public partial class ProductDetailsWindow : Window
    {
        private int? productId;
        private Product? currentProduct;
        private bool isEditMode = false;

        public ProductDetailsWindow(int? productId)
        {
            InitializeComponent();
            LoadLogo();
            this.productId = productId;
            LoadProduct();
            UpdateEditMode();
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

        private void LoadProduct()
        {
            if (productId.HasValue)
            {
                using (FishContext db = new FishContext())
                {
                    currentProduct = db.Products
                        .Include(p => p.IdCategoryNavigation)
                        .Include(p => p.IdProisvoditelNavigation)
                        .Include(p => p.IdPostavshikNavigation)
                        .FirstOrDefault(p => p.Id == productId.Value);

                    if (currentProduct != null)
                    {
                        DisplayProduct();
                    }
                    else
                    {
                        MessageBox.Show("Товар не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                    }
                }
            }
            else
            {
                var editWindow = new ProductEditWindow(null);
                if (editWindow.ShowDialog() == true)
                {
                    this.DialogResult = true;
                }
                this.Close();
                return;
            }
        }

        private void DisplayProduct()
        {
            if (currentProduct == null) return;

            string categoryName = currentProduct.IdCategoryNavigation?.Name ?? "Не указано";
            NameCategoryText.Text = $"{currentProduct.Name ?? "Не указано"} | {categoryName}";

            ManufacturerText.Text = currentProduct.IdProisvoditelNavigation?.Name ?? "Не указано";

            SupplierText.Text = currentProduct.IdPostavshikNavigation?.Name ?? "Не указано";

            string unit = currentProduct.Unit ?? "шт";
            CountText.Text = $"{(currentProduct.Count ?? 0)} {unit}";

            DescriptionText.Text = currentProduct.Description ?? "Описание отсутствует";

            int price = currentProduct.Price ?? 0;
            int sale = currentProduct.Sale ?? 0;
            
            SaleText.Text = Convert.ToString(sale) + "%";

            PriceText.Text = $"Цена: {price} ₽ / {unit}";

            LoadProductImage();
        }

        private void LoadProductImage()
        {
            bool imageLoaded = false;
            BitmapImage bitmap = new BitmapImage();
            try
            {
                if (!string.IsNullOrEmpty(currentProduct?.Imgpath) && File.Exists(currentProduct.Imgpath))
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(currentProduct.Imgpath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ProductImage.Source = bitmap;
                    imageLoaded = true;
                }
                else
                {
                    string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "picture.png");
                    if (File.Exists(placeholderPath))
                    {
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(placeholderPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        ProductImage.Source = bitmap;
                        imageLoaded = true;
                    }
                    else
                    {
                        ProductImage.Source = null;
                        imageLoaded = false;
                    }
                }
            }
            catch
            {
                try
                {
                    string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "picture.png");
                    if (File.Exists(placeholderPath))
                    {
                        bitmap = new BitmapImage(new Uri(placeholderPath, UriKind.Absolute));
                        ProductImage.Source = bitmap;
                        imageLoaded = true;
                    }
                }
                catch
                {
                    ProductImage.Source = null;
                    imageLoaded = false;
                }
            }
            
            if (PhotoPlaceholderText != null)
            {
                PhotoPlaceholderText.Visibility = imageLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void UpdateEditMode()
        {
            var user = RoleState._CurrentUser;
            bool isAdmin = user != null && user.IdRole >= 2;

            if (isAdmin && productId.HasValue)
            {
                EditButton.Visibility = isEditMode ? Visibility.Collapsed : Visibility.Visible;
                SaveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                EditButton.Visibility = Visibility.Collapsed;
                SaveButton.Visibility = Visibility.Collapsed;
                CancelButton.Visibility = Visibility.Collapsed;
            }
            
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            isEditMode = true;
            UpdateEditMode();
            ShowEditControls();
        }

        private void ShowEditControls()
        {
            var editWindow = new ProductEditWindow(currentProduct);
            if (editWindow.ShowDialog() == true)
            {
                LoadProduct();
                isEditMode = false;
                UpdateEditMode();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var editWindow = new ProductEditWindow(currentProduct);
            if (editWindow.ShowDialog() == true)
            {
                LoadProduct();
                isEditMode = false;
                UpdateEditMode();
                this.DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            isEditMode = false;
            LoadProduct();
            UpdateEditMode();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

