using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.Entity;

namespace WpfApp9
{
    public partial class EmployeePage : Page
    {
        public EmployeePage()
        {
            InitializeComponent();
            LoadEmployeeData();
        }

        private void LoadEmployeeData()
        {
            try
            {
                using (var context = new CosmeticsShopEntities6())
                {
                    var employees = context.Employees.ToList();
                    employeeDataGrid.ItemsSource = employees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployeePage());
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Employee employee)
            {
                NavigationService.Navigate(new AddEmployeePage(employee));
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (employeeDataGrid?.SelectedItems?.Count == 0)
            {
                MessageBox.Show("Выберите сотрудников для удаления",
                              "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var employeesToDelete = employeeDataGrid.SelectedItems.Cast<Employee>().ToList();
            var result = MessageBox.Show($"Вы точно хотите удалить {employeesToDelete.Count} сотрудников?",
                                       "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new CosmeticsShopEntities6())
                    {
                        var idsToDelete = employeesToDelete.Select(emp => emp.EmployeeID).ToList();
                        var employeesInDb = context.Employees
                                            .Where(emp => idsToDelete.Contains(emp.EmployeeID))
                                            .ToList();

                        context.Employees.RemoveRange(employeesInDb);
                        int changes = context.SaveChanges();

                        if (changes > 0)
                        {
                            MessageBox.Show($"Успешно удалено {changes} сотрудников",
                                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadEmployeeData();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}\n\n{ex.InnerException?.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SearchEmployee_TextChanged(object sender, TextChangedEventArgs args)
        {
            FilterAndSortEmployees();
        }

        private void SortEmployee_SelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            FilterAndSortEmployees();
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs args)
        {
            SearchEmployeeName.Text = string.Empty;
            SortComboBox.SelectedIndex = 0;
            FilterAndSortEmployees();
        }

        private void FilterAndSortEmployees()
        {
            try
            {
                using (var context = new CosmeticsShopEntities6())
                {
                    IQueryable<Employee> query = context.Employees;

                    if (!string.IsNullOrWhiteSpace(SearchEmployeeName.Text))
                    {
                        string searchText = SearchEmployeeName.Text.ToLower();
                        query = query.Where(emp =>
                            emp.FirstName.ToLower().Contains(searchText) ||
                            emp.LastName.ToLower().Contains(searchText));
                    }

                    if (SortComboBox.SelectedItem is ComboBoxItem selectedItem)
                    {
                        switch (selectedItem.Content.ToString())
                        {
                            case "По имени":
                                query = query.OrderBy(emp => emp.FirstName);
                                break;
                            case "По фамилии":
                                query = query.OrderBy(emp => emp.LastName);
                                break;
                            case "По дате найма":
                                query = query.OrderBy(emp => emp.HireDate);
                                break;
                            case "По зарплате":
                                query = query.OrderBy(emp => emp.Salary);
                                break;
                        }
                    }
                   if (query == null)
                    LoadEmployeeData();
                    else if (employeeDataGrid == null)
                       LoadEmployeeData();
                    else
                       employeeDataGrid.ItemsSource = query.ToList();
                }
            }
            catch (Exception ex)
            {
               MessageBox.Show($"Ошибка при фильтрации данных: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeePage_VisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (Visibility == Visibility.Visible)
            {
                LoadEmployeeData();
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs args)
        {
            if (NavigationService?.CanGoBack == true)
            {
                NavigationService.GoBack();
            }
        }
    }
}