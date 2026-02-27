using fishapp.Models;
using fishapp.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace fishapp
{
    public partial class ProductsWindow : Window
    {
        private List<ProductViewModel> allProducts = new List<ProductViewModel>();
        private List<ProductViewModel> filteredProducts = new List<ProductViewModel>();

        public ProductsWindow()
        {
            InitializeComponent();
            LoadLogo();
            LoadProducts();
            LoadFilters();
            UpdateUI();
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

        private void LoadProducts()
        {
            try
            {
                using (FishContext db = new FishContext())
                {
                    allProducts = db.Products
                        .Include(p => p.IdCategoryNavigation)
                        .Include(p => p.IdProisvoditelNavigation)
                        .Include(p => p.IdPostavshikNavigation)
                        .Select(p => new ProductViewModel
                        {
                            Id = p.Id,
                            Name = p.Name ?? "Не указано",
                            CategoryName = p.IdCategoryNavigation != null ? p.IdCategoryNavigation.Name : "Не указано",
                            ManufacturerName = p.IdProisvoditelNavigation != null ? p.IdProisvoditelNavigation.Name : "Не указано",
                            SupplierName = p.IdPostavshikNavigation != null ? p.IdPostavshikNavigation.Name : "Не указано",
                            Price = p.Price ?? 0,
                            Unit = p.Unit ?? "шт",
                            Count = p.Count ?? 0,
                            Sale = Convert.ToString(p.Sale) + "%" ?? "-",
                            Description = p.Description ?? "",
                            ImagePath = p.Imgpath,
                            CategoryId = p.IdCategory,
                            ManufacturerId = p.IdProisvoditel,
                            SupplierId = p.IdPostavshik
                        })
                        .ToList();

                    if (allProducts != null)
                    {
                        foreach (var product in allProducts)
                        {
                            product.ImageSource = LoadProductImage(product.ImagePath);
                        }
                    }
                }

                if (allProducts != null)
                {
                    filteredProducts = new List<ProductViewModel>(allProducts);
                    if (ProductsListBox != null)
                    {
                        ProductsListBox.ItemsSource = filteredProducts;
                    }
                }
                else
                {
                    filteredProducts = new List<ProductViewModel>();
                    if (ProductsListBox != null)
                    {
                        ProductsListBox.ItemsSource = filteredProducts;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                allProducts = new List<ProductViewModel>();
                filteredProducts = new List<ProductViewModel>();
                if (ProductsListBox != null)
                {
                    ProductsListBox.ItemsSource = filteredProducts;
                }
            }
        }

        private BitmapImage LoadProductImage(string? imagePath)
        {
            BitmapImage bitmap = new BitmapImage();
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
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
                    }
                    else
                    {
                        bitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/picture.png"));
                    }
                }
            }
            catch
            {

                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "picture.png");
                if (File.Exists(placeholderPath))
                {
                    bitmap = new BitmapImage(new Uri(placeholderPath, UriKind.Absolute));
                }
            }
            return bitmap;
        }

        private void LoadFilters()
        {
            try
            {
                using (FishContext db = new FishContext())
                {
                    var categories = db.Categories?.ToList() ?? new List<Category>();
                    if (CategoryFilterComboBox != null)
                    {
                        CategoryFilterComboBox.Items.Clear();
                        CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все категории", IsSelected = true });
                        foreach (var category in categories)
                        {
                            CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = category.Name ?? "Без названия", Tag = category.Id });
                        }
                    }

                    var manufacturers = db.Proisvoditels?.ToList() ?? new List<Proisvoditel>();
                    if (ManufacturerFilterComboBox != null)
                    {
                        ManufacturerFilterComboBox.Items.Clear();
                        ManufacturerFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все производители", IsSelected = true });
                        foreach (var manufacturer in manufacturers)
                        {
                            ManufacturerFilterComboBox.Items.Add(new ComboBoxItem { Content = manufacturer.Name ?? "Без названия", Tag = manufacturer.Id });
                        }
                    }

                    var suppliers = db.Postavshiks?.ToList() ?? new List<Postavshik>();
                    if (SupplierFilterComboBox != null)
                    {
                        SupplierFilterComboBox.Items.Clear();
                        SupplierFilterComboBox.Items.Add(new ComboBoxItem { Content = "Все поставщики", IsSelected = true });
                        foreach (var supplier in suppliers)
                        {
                            SupplierFilterComboBox.Items.Add(new ComboBoxItem { Content = supplier.Name ?? "Без названия", Tag = supplier.Id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке фильтров: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilters()
        {
            if (allProducts == null || SearchTextBox == null)
                return;

            filteredProducts = allProducts.Where(p =>
            {
                if (!string.IsNullOrEmpty(SearchTextBox?.Text))
                {
                    string searchText = SearchTextBox.Text.ToLower();
                    if (!p.Name.ToLower().Contains(searchText) &&
                        !p.CategoryName.ToLower().Contains(searchText) &&
                        !p.ManufacturerName.ToLower().Contains(searchText) &&
                        !p.SupplierName.ToLower().Contains(searchText) &&
                        !p.Description.ToLower().Contains(searchText))
                    {
                        return false;
                    }
                }

                if (CategoryFilterComboBox != null && CategoryFilterComboBox.SelectedItem is ComboBoxItem categoryItem && categoryItem.Tag != null)
                {
                    if (p.CategoryId != (int?)categoryItem.Tag)
                        return false;
                }

                if (ManufacturerFilterComboBox != null && ManufacturerFilterComboBox.SelectedItem is ComboBoxItem manufacturerItem && manufacturerItem.Tag != null)
                {
                    if (p.ManufacturerId != (int?)manufacturerItem.Tag)
                        return false;
                }

                if (SupplierFilterComboBox != null && SupplierFilterComboBox.SelectedItem is ComboBoxItem supplierItem && supplierItem.Tag != null)
                {
                    if (p.SupplierId != (int?)supplierItem.Tag)
                        return false;
                }

                return true;
            }).ToList();

            ApplySorting();
            if (ProductsListBox != null)
            {
                ProductsListBox.ItemsSource = filteredProducts;
            }
        }

        private void ApplySorting()
        {
            if (filteredProducts == null || SortComboBox == null)
                return;

            if (SortComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Content != null)
            {
                switch (selectedItem.Content.ToString())
                {
                    case "По наименованию (А-Я)":
                        filteredProducts = filteredProducts.OrderBy(p => p.Name).ToList();
                        break;
                    case "По наименованию (Я-А)":
                        filteredProducts = filteredProducts.OrderByDescending(p => p.Name).ToList();
                        break;
                    case "По цене (возрастание)":
                        filteredProducts = filteredProducts.OrderBy(p => p.Price).ToList();
                        break;
                    case "По цене (убывание)":
                        filteredProducts = filteredProducts.OrderByDescending(p => p.Price).ToList();
                        break;
                    case "По категории":
                        filteredProducts = filteredProducts.OrderBy(p => p.CategoryName).ToList();
                        break;
                }
            }
        }

        private void UpdateUI()
        {
            var user = RoleState._CurrentUser;
            if (UserInfo == null || AddProductButton == null || EditProductButton == null || DeleteProductButton == null)
                return;

            if (user == null)
            {
                UserInfo.Text = "Гость";
                AddProductButton.Visibility = Visibility.Collapsed;
                EditProductButton.Visibility = Visibility.Collapsed;
                DeleteProductButton.Visibility = Visibility.Collapsed;
            }
            else if (user.IdRole == 0)
            {
                UserInfo.Text = $"Клиент: {user.Fio ?? user.Login}";
                AddProductButton.Visibility = Visibility.Collapsed;
                EditProductButton.Visibility = Visibility.Collapsed;
                DeleteProductButton.Visibility = Visibility.Collapsed;
            }
            else if (user.IdRole == 1)
            {
                UserInfo.Text = $"Менеджер: {user.Fio ?? user.Login}";
                AddProductButton.Visibility = Visibility.Visible;
                EditProductButton.Visibility = Visibility.Visible;
                DeleteProductButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                UserInfo.Text = $"Администратор: {user.Fio ?? user.Login}";
                AddProductButton.Visibility = Visibility.Visible;
                EditProductButton.Visibility = Visibility.Visible;
                DeleteProductButton.Visibility = Visibility.Visible;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ManufacturerFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SupplierFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null)
                SearchTextBox.Text = "";
            if (SortComboBox != null && SortComboBox.Items.Count > 0)
                SortComboBox.SelectedIndex = 0;
            if (CategoryFilterComboBox != null && CategoryFilterComboBox.Items.Count > 0)
                CategoryFilterComboBox.SelectedIndex = 0;
            if (ManufacturerFilterComboBox != null && ManufacturerFilterComboBox.Items.Count > 0)
                ManufacturerFilterComboBox.SelectedIndex = 0;
            if (SupplierFilterComboBox != null && SupplierFilterComboBox.Items.Count > 0)
                SupplierFilterComboBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ProductsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsListBox != null)
            {
                ViewDetailsButton.IsEnabled = ProductsListBox.SelectedItem != null;
                var user = RoleState._CurrentUser;
                bool canEdit = ProductsListBox.SelectedItem != null && user != null && user.IdRole >= 1;
                EditProductButton.IsEnabled = canEdit;
                DeleteProductButton.IsEnabled = ProductsListBox.SelectedItem != null && user != null && user.IdRole >= 2;
            }
        }

        private void ProductItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ProductViewModel product)
            {
                ProductsListBox.SelectedItem = product;
                if (e.ClickCount == 2)
                {
                    ViewDetailsButton_Click(null, null);
                }
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox != null && ProductsListBox.SelectedItem is ProductViewModel selectedProduct)
            {
                var detailsWindow = new ProductDetailsWindow(selectedProduct.Id);
                detailsWindow.ShowDialog();
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new ProductAddWindow();
            if (addWindow.ShowDialog() == true)
            {
                LoadProducts();
                LoadFilters();
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox != null && ProductsListBox.SelectedItem is ProductViewModel selectedProduct)
            {
                using (FishContext db = new FishContext())
                {
                    var product = db.Products.Find(selectedProduct.Id);
                    if (product != null)
                    {
                        var editWindow = new ProductEditWindow(product);
                        if (editWindow.ShowDialog() == true)
                        {
                            LoadProducts();
                            LoadFilters();
                        }
                    }
                }
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsListBox != null && ProductsListBox.SelectedItem is ProductViewModel selectedProduct)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedProduct.Name}'?", 
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    using (FishContext db = new FishContext())
                    {
                        var product = db.Products.Find(selectedProduct.Id);
                        if (product != null)
                        {
                            db.Products.Remove(product);
                            db.SaveChanges();
                            LoadProducts();
                            MessageBox.Show("Товар успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string ManufacturerName { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public int Price { get; set; }
        public string Unit { get; set; } = "";
        public int Count { get; set; }
        public string Sale { get; set; } = "";
        public string Description { get; set; } = "";
        public string? ImagePath { get; set; }
        public BitmapImage? ImageSource { get; set; }
        public int? CategoryId { get; set; }
        public int? ManufacturerId { get; set; }
        public int? SupplierId { get; set; }
    }
}

