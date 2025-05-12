using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp9.Pages
{
    public partial class AddProductPage : Page
    {
        private readonly Product _product;
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();
        private readonly bool _isNew;

        public AddProductPage(Product product)
        {
            InitializeComponent();

            _product = product ?? new Product();
            _isNew = product == null;

            DataContext = _product;
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_product.ProductName))
                errors.AppendLine("Укажите название товара");
            if (string.IsNullOrWhiteSpace(_product.Brand))
                errors.AppendLine("Укажите бренд товара");
            if (string.IsNullOrWhiteSpace(_product.Category))
                errors.AppendLine("Укажите категорию товара");
            if (_product.Price <= 0)
                errors.AppendLine("Цена должна быть больше 0");
            if (_product.Stock < 0)
                errors.AppendLine("Количество не может быть отрицательным");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (_isNew)
                {
                    _db.Products.Add(_product);
                }
                else
                {
                    _db.Entry(_product).State = EntityState.Modified;
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