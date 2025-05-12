using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System;

namespace WpfApp9
{
    public partial class CustomerPage : Page
    {
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();

        public CustomerPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            _db.Customers.Load();
            customerDataGrid.ItemsSource = _db.Customers.Local.ToList();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddCustomerPage(null));
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var customer = (sender as Button)?.DataContext as Customer;
            if (customer != null)
            {
                NavigationService?.Navigate(new AddCustomerPage(customer));
            }
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var customersForRemoving = customerDataGrid.SelectedItems.Cast<Customer>().ToList();
            if (customersForRemoving.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить {customersForRemoving.Count} клиентов?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Customers.RemoveRange(customersForRemoving);
                    _db.SaveChanges();
                    MessageBox.Show("Клиенты успешно удалены!");
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Ошибка при удалении");
                }
            }
        }

        private void CustomerPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                customerDataGrid.ItemsSource = _db.Customers.ToList();
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SearchCustomerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCustomers();
        }

        private void SortCustomerCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCustomers();
        }

        private void CleanFilter_OnClick(object sender, RoutedEventArgs e)
        {
            SearchCustomerName.Text = string.Empty;
            SortCustomerCategory.SelectedIndex = -1;
            UpdateCustomers();
        }

        private void UpdateCustomers()
        {
            var currentCustomers = _db.Customers.ToList();
            currentCustomers = currentCustomers.Where(x =>
                x.FirstName.ToLower().Contains(SearchCustomerName.Text.ToLower()) ||
                x.LastName.ToLower().Contains(SearchCustomerName.Text.ToLower())).ToList();
            customerDataGrid.ItemsSource = currentCustomers;
        }
    }
}
