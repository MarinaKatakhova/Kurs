using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Linq;

namespace WpfApp9
{
    public partial class AddCustomerPage : Page
    {
        private readonly Customer _customer;
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();
        private readonly bool _isNew;

        public AddCustomerPage(Customer customer)
        {
            InitializeComponent();

            _customer = customer ?? new Customer();
            _isNew = customer == null;

            if (!_isNew)
            {
                _customer = _db.Customers.Find(customer.CustomerID) ?? customer;
                passwordBox.Password = "********";
            }

            DataContext = _customer;
        }

        private static string GetHash(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                return string.Concat(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_customer.FirstName))
                errors.AppendLine("Укажите имя");
            if (string.IsNullOrWhiteSpace(_customer.LastName))
                errors.AppendLine("Укажите фамилию");
            if (string.IsNullOrWhiteSpace(_customer.Email))
                errors.AppendLine("Укажите email");
            if (string.IsNullOrWhiteSpace(_customer.PhoneNumber))
                errors.AppendLine("Укажите телефон");
            if (string.IsNullOrWhiteSpace(_customer.login))
                errors.AppendLine("Укажите логин");
            if (_isNew && string.IsNullOrWhiteSpace(passwordBox.Password))
                errors.AppendLine("Укажите пароль");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (_isNew || passwordBox.Password != "********")
                {
                    _customer.password = GetHash(passwordBox.Password);
                }

                if (_isNew)
                {
                    _db.Customers.Add(_customer);
                }
                else
                {
                    _db.Entry(_customer).State = EntityState.Modified;
                }

                _db.SaveChanges();
                MessageBox.Show("Данные сохранены успешно");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}