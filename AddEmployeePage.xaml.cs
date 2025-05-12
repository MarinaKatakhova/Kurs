using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfApp9
{
    public partial class AddEmployeePage : Page
    {
        public Employee CurrentEmployee;
        private List<string> _positions;

        public AddEmployeePage(Employee employee = null)
        {
            try
            {
                InitializeComponent();
                LoadPositions();

                if (employee != null)
                {
                    CurrentEmployee = employee;
                    FirstNameTextBox.Text = CurrentEmployee.FirstName;
                    LastNameTextBox.Text = CurrentEmployee.LastName;
                    PositionComboBox.Text = CurrentEmployee.Position;
                    SalaryTextBox.Text = CurrentEmployee.Salary.ToString();
                    HireDatePicker.SelectedDate = CurrentEmployee.HireDate;
                }
                else
                {
                    CurrentEmployee = new Employee();
                    HireDatePicker.SelectedDate = DateTime.Today;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPositions()
        {
            try
            {
                using (var db = new CosmeticsShopEntities6())
                {
                    _positions = db.Employees
                        .Select(e => e.Position)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList();

                    PositionComboBox.ItemsSource = _positions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки должностей: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    using (var db = new CosmeticsShopEntities6())
                    {
                        if (CurrentEmployee.EmployeeID > 0)
                        {
                            var existingEmployee = db.Employees.Find(CurrentEmployee.EmployeeID);
                            if (existingEmployee != null)
                            {
                                UpdateEmployee(existingEmployee);
                                db.SaveChanges();
                                MessageBox.Show("Данные сотрудника успешно обновлены", "Успех",
                                              MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                        else
                        {
                            UpdateEmployee(CurrentEmployee);
                            db.Employees.Add(CurrentEmployee);
                            db.SaveChanges();
                            MessageBox.Show("Новый сотрудник успешно сохранен", "Успех",
                                          MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                        NavigationService?.Navigate(new EmployeePage());
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);
                MessageBox.Show($"Ошибки валидации:\n{string.Join("\n", errorMessages)}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.InnerException?.Message ?? ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateEmployee(Employee employee)
        {
            employee.FirstName = FirstNameTextBox.Text.Trim();
            employee.LastName = LastNameTextBox.Text.Trim();
            employee.Position = PositionComboBox.Text;
            employee.Salary = decimal.Parse(SalaryTextBox.Text);
            employee.HireDate = HireDatePicker.SelectedDate.Value;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя сотрудника", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию сотрудника", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(PositionComboBox.Text))
            {
                MessageBox.Show("Выберите должность", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(SalaryTextBox.Text, out decimal salary) || salary <= 0)
            {
                MessageBox.Show("Введите корректную зарплату", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (HireDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату приема", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new EmployeePage());
        }
    }
}