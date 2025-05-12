using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace WpfApp9
{
    public partial class AddSupplierPage : Page
    {
        private readonly Supplier _supplier;
        private readonly CosmeticsShopEntities6 _db = new CosmeticsShopEntities6();
        private readonly bool _isNew;

        public AddSupplierPage(Supplier supplier)
        {
            InitializeComponent();

            _supplier = supplier ?? new Supplier();
            _isNew = supplier == null;

            DataContext = _supplier;
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_supplier.SupplierName))
                errors.AppendLine("Укажите название поставщика");
            if (string.IsNullOrWhiteSpace(_supplier.ContactPerson))
                errors.AppendLine("Укажите контактное лицо");
            if (string.IsNullOrWhiteSpace(_supplier.PhoneNumber))
                errors.AppendLine("Укажите телефон");
            if (string.IsNullOrWhiteSpace(_supplier.Email))
                errors.AppendLine("Укажите email");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка");
                return;
            }

            try
            {
                if (_isNew)
                {
                    _db.Suppliers.Add(_supplier);
                }
                else
                {
                    _db.Entry(_supplier).State = EntityState.Modified;
                }

                _db.SaveChanges();
                MessageBox.Show("Данные сохранены успешно");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}