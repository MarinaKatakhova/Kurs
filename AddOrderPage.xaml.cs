using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp9
{
    public partial class AddOrderPage : Page
    {
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();
        private Order _currentOrder;

        public AddOrderPage(Order order = null)
        {
            InitializeComponent();
            InitializeFields(order);
        }

        private void InitializeFields(Order order)
        {
            CustomerComboBox.ItemsSource = _db.Customers.ToList();

            _currentOrder = order ?? new Order { OrderDate = DateTime.Now };

            if (order != null)
            {
                CustomerComboBox.SelectedItem = order.Customer;
                OrderDatePicker.SelectedDate = order.OrderDate;
                TotalAmountTextBox.Text = order.TotalAmount.ToString();
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                SaveOrder();
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private bool ValidateInputs()
        {
            if (CustomerComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента");
                return false;
            }

            if (!decimal.TryParse(TotalAmountTextBox.Text, out var amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную сумму");
                return false;
            }

            return true;
        }

        private void SaveOrder()
        {
            _currentOrder.Customer = (Customer)CustomerComboBox.SelectedItem;
            _currentOrder.OrderDate = OrderDatePicker.SelectedDate ?? DateTime.Now;
            _currentOrder.TotalAmount = decimal.Parse(TotalAmountTextBox.Text);

            if (_db.Entry(_currentOrder).State == EntityState.Detached)
            {
                _db.Orders.Add(_currentOrder);
            }

            _db.SaveChanges();
        }
    }
}
