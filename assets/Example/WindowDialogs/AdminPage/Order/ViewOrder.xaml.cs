using BeautySalonWpf.Pages.Admin.Tabs;
using HandyControl.Controls;
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
using System.Windows.Shapes;

namespace BeautySalonWpf.WindowDialogs.AdminPage.Order
{
    /// <summary>
    /// Логика взаимодействия для ViewOrder.xaml
    /// </summary>
    public partial class ViewOrder : System.Windows.Window
    {
        private Orders _order;
        private OrdersTab _owner;
        public ViewOrder(Orders order, OrdersTab owner)
        {
            InitializeComponent();

            _owner = owner;
            _order = order;
            var orderItems = ConnectionDb.db.OrderProducts.Where(x => x.orderId == _order.id).ToList();

            SelectedProducts.ItemsSource = orderItems;
            ClientText.Text = _order.Clients.Lname + " " + _order.Clients.FName;
            DateText.Text = getDate(_order.date.Value);
            CountProductsText.Text = _order.OrderProducts.Count.ToString() + " шт";
            totalPriceText.Text = _order.sum.ToString() + " ₽";
            DiscountText.Text = _order.discount.ToString() + " %";
            DiscountPriceText.Text = _order.discountSum.ToString() + " ₽";
            OrderPay.IsEnabled=_order.statusId == 2 ?  false : true;
            OrderPay.Content = _order.statusId == 1 ? "Отметить оплаченным" : "Уже оплачен";





        }
        public string getDate(DateTime date)
        {
            int day = date.Day;
            int month =date.Month;
            int year = date.Year;

            string[] months = {
    "января", "февраля", "марта", "апреля", "мая", "июня",
    "июля", "августа", "сентября", "октября", "ноября", "декабря"
};
            
            return $"{day} {months[month - 1]} {year} г.";
        }
        private async void CreateAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            _order.statusId = 2;
            ConnectionDb.db.SaveChanges();
            Growl.Success("заказ оплачен");
            await Task.Delay(2000);
            Growl.Clear();
            _owner.UpdateOrdersList();
            this.Close();

        }

        private void CloseAddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
