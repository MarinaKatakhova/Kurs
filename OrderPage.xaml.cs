using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System;

namespace WpfApp9
{
    public partial class OrderPage : Page
    {
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();

        public OrderPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            orderDataGrid.ItemsSource = _db.Orders.ToList();
        }

        private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddOrderPage(null));
        }

        private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
        {
            var order = (sender as Button)?.DataContext as Order;
            if (order != null)
            {
                NavigationService?.Navigate(new AddOrderPage(order));
            }
        }

        private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var ordersForRemoving = orderDataGrid.SelectedItems.Cast<Order>().ToList();
            if (ordersForRemoving.Count == 0) return;

            if (MessageBox.Show($"Вы точно хотите удалить {ordersForRemoving.Count} заказов?",
                "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Orders.RemoveRange(ordersForRemoving);
                    _db.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    LoadOrders();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void OrderPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                orderDataGrid.ItemsSource = _db.Orders.ToList();
            }
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void SearchOrderId_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateOrders();
        }

        private void SortOrderCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOrders();
        }

        private void CleanFilter_OnClick(object sender, RoutedEventArgs e)
        {
            SearchOrderId.Text = string.Empty;
            SortOrderCategory.SelectedIndex = -1;
            UpdateOrders();
        }

        private void UpdateOrders()
        {
            var currentOrders = _db.Orders.ToList();
            currentOrders = currentOrders.Where(x =>
                x.OrderID1.ToString().Contains(SearchOrderId.Text)).ToList();
            orderDataGrid.ItemsSource = currentOrders;
        }
    }
}
