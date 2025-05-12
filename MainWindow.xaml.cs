using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp9
{
    public partial class MainWindow : Window
    {
        private CosmeticsShopEntities6 db = new CosmeticsShopEntities6();

        public MainWindow()
        {
            InitializeComponent();
            NavigateToDatabaseButton.Visibility = Visibility.Collapsed;
        }

        public static string GetHash(string password)
        {
            using (var sha1 = SHA1.Create())
            {
                return string.Concat(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTextBox.Text) ||
                string.IsNullOrEmpty(passwordText.Password) ||
                string.IsNullOrEmpty(emailText.Text) ||
                string.IsNullOrEmpty(phoneText.Text))
            {
                MessageBox.Show("Please fill in all fields!");
                return;
            }

            var user = db.Customers.AsNoTracking().FirstOrDefault(u => u.login == loginTextBox.Text);
            if (user != null)
            {
                MessageBox.Show("A user with this login already exists!");
                return;
            }

            var passwordValidation = ValidatePassword(passwordText.Password);
            var emailValid = ValidateEmail(emailText.Text);
            var phoneValid = ValidatePhone(phoneText.Text);

            if (!passwordValidation.isValid || !emailValid || !phoneValid)
            {
                StringBuilder errors = new StringBuilder();

                if (!passwordValidation.isValid)
                {
                    errors.AppendLine("Password requirements:");
                    errors.AppendLine("- At least 8 characters");
                    errors.AppendLine("- At least one digit");
                    errors.AppendLine("- At least one uppercase letter");
                    errors.AppendLine("- At least one lowercase letter");
                }

                if (!emailValid)
                    errors.AppendLine("Please enter a valid email in the format x@x.x");

                if (!phoneValid)
                    errors.AppendLine("Please enter the phone number in the format +7XXXXXXXXXX (11 digits)");

                MessageBox.Show(errors.ToString());
                return;
            }

            string hashedPassword = GetHash(passwordText.Password);

            Customer customer = new Customer
            {
                FirstName = "",
                LastName = "",
                login = loginTextBox.Text,
                password = hashedPassword,
                Email = emailText.Text,
                PhoneNumber = phoneText.Text
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            MessageBox.Show("You have successfully registered!", "Success", MessageBoxButton.OK);

            AuthPanel.Visibility = Visibility.Collapsed;
            NavigateToDatabaseButton.Visibility = Visibility.Visible;
        }

        private (bool isValid, bool hasNumber, bool hasUpper, bool hasLower) ValidatePassword(string password)
        {
            bool hasNumber = false;
            bool hasUpper = false;
            bool hasLower = false;

            foreach (char c in password)
            {
                if (char.IsDigit(c)) hasNumber = true;
                if (char.IsUpper(c)) hasUpper = true;
                if (char.IsLower(c)) hasLower = true;
            }

            bool isValid = password.Length >= 8 && hasNumber && hasUpper && hasLower;
            return (isValid, hasNumber, hasUpper, hasLower);
        }

        private bool ValidateEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        private bool ValidatePhone(string phone)
        {
            var phoneRegex = new Regex(@"^\+?[1-9]\d{10}$");
            return phoneRegex.IsMatch(phone);
        }

        private void signInButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(loginTextBox.Text) || string.IsNullOrEmpty(passwordText.Password))
            {
                MessageBox.Show("Please enter your login and password!");
                return;
            }

            string hashedPassword = GetHash(passwordText.Password);

            var user = db.Customers.AsNoTracking().FirstOrDefault(u => u.login == loginTextBox.Text && u.password == hashedPassword);
            if (user == null)
            {
                MessageBox.Show("User with these credentials not found!");
                return;
            }

            MessageBox.Show("Login successful!", "Success", MessageBoxButton.OK);

            MainFrame.Navigate(new TablesPage());
        }

        private void NavigateToTablesPage(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TablesPage());
        }
    }
}