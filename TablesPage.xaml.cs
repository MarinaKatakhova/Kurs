using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp9;

namespace WpfApp9
{
    public partial class TablesPage : Page
    {
        public TablesPage()
        {
            InitializeComponent();
        }

        private void NavigateToCustomers(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CustomerPage());
        }

        private void NavigateToProducts(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Pages.ProductPage());
        }

        private void NavigateToOrders(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrderPage());
        }

        private void NavigateToEmployees(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeePage());
        }

        private void NavigateToSuppliers(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SupplierPage());
        }

        private void NavigateBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
