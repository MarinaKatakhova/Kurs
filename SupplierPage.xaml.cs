using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System;
using System.Collections.Generic;

namespace WpfApp9
{
    public partial class SupplierPage : Page
    {
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();

        public SupplierPage()
        {
            InitializeComponent();
            LoadSupplierData();
        }

        private void LoadSupplierData()
        {
            try
            {
                supplierListView.ItemsSource = _db.Suppliers.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки поставщиков: {ex.Message}");
            }
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddSupplierPage(null));
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Supplier supplier)
            {
                NavigationService?.Navigate(new AddSupplierPage(supplier));
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            var selectedSupplier = (sender as Button)?.DataContext as Supplier;
            if (selectedSupplier == null) return;

            if (MessageBox.Show($"Удалить поставщика {selectedSupplier.SupplierName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Suppliers.Remove(selectedSupplier);
                    _db.SaveChanges();
                    LoadSupplierData();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        private void SupplierPage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                LoadSupplierData();
            }
        }

        private void GoBackSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }

        private void SearchSupplier_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshSupplierList();
        }

        private void SortSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshSupplierList();
        }

        private void ClearSupplierFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchSupplierName.Text = string.Empty;
            SortSupplierCategory.SelectedIndex = -1;
            RefreshSupplierList();
        }

        private void RefreshSupplierList()
        {
            try
            {
                var currentSuppliers = _db.Suppliers.ToList();
                if (!string.IsNullOrWhiteSpace(SearchSupplierName?.Text))
                {
                    currentSuppliers = currentSuppliers.Where(x =>
                        x.SupplierName?.ToLower().Contains(SearchSupplierName.Text.ToLower()) == true).ToList();
                }
                supplierListView.ItemsSource = currentSuppliers;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления: {ex.Message}");
            }
        }
    }
}