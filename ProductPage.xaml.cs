using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System;

namespace WpfApp9.Pages
{
    public partial class ProductPage : Page
    {
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();

        public ProductPage()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            productDataGrid.ItemsSource = _db.Products.ToList();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddProductPage(null));
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button)?.DataContext as Product;
            if (product != null)
            {
                NavigationService?.Navigate(new AddProductPage(product));
            }
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var productsForRemoving = productDataGrid.SelectedItems.Cast<Product>().ToList();
            if (productsForRemoving.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить {productsForRemoving.Count} элемент?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Products.RemoveRange(productsForRemoving);
                    _db.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ProductPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                productDataGrid.ItemsSource = _db.Products.ToList();
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SearchProductName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void SortProductCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void CleanFilter_OnClick(object sender, RoutedEventArgs e)
        {
            SearchProductName.Text = string.Empty;
            SortProductCategory.SelectedIndex = -1;
            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var currentProducts = _db.Products.ToList();
            currentProducts = currentProducts.Where(x =>
                x.ProductName.ToLower().Contains(SearchProductName.Text.ToLower())).ToList();
            productDataGrid.ItemsSource = currentProducts;
        }
    }
}
