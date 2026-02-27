using fishapp.Models;
using fishapp.Services;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace fishapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadLogo();
        }

        private void LoadLogo()
        {
            if (LogoImage == null) return;
            
            try
            {
                string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "logo.png");
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
                if (LogoImage != null)
                    LogoImage.Visibility = Visibility.Collapsed;
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void GuestClick(object sender, RoutedEventArgs e)
        {
            RoleState._CurrentUser = null;
            OpenProductsWindow();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (Login.Text != string.Empty && Pass.Password != string.Empty)
            {
                using (FishContext db = new FishContext())
                {
                    User user = db.Users.FirstOrDefault(u => u.Login == Login.Text && u.Pass == Pass.Password);

                    if (user != null)
                    {
                        RoleState._CurrentUser = user;
                        string roleName = user.IdRole switch
                        {
                            0 => "клиента",
                            1 => "менеджера",
                            _ => "администратора"
                        };
                        MessageBox.Show($"Вход {roleName}");
                        OpenProductsWindow();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось войти.");
                    }
                }
            }
        }

        private void OpenProductsWindow()
        {
            var productsWindow = new ProductsWindow();
            productsWindow.Show();
            this.Close();
        }
    }
}