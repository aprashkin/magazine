using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using magazine_music.Context;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace magazine_music
{
    public partial class ProfileWindow : Window
    {
        public string UserFirstName { get; set; }
        public string UserEmail { get; set; }
        public string UserBirthday { get; set; }
        public string UserLastname { get; set; }

        public ObservableCollection<CartItemViewModel> CartItems { get; set; } = new ObservableCollection<CartItemViewModel>();

        public ProfileWindow()
        {
            InitializeComponent();

            if (Session.CurrentUser != null)
            {
                UserFirstName = $"Имя: {Session.CurrentUser.UserFirstname}";
                UserEmail = $"Email: {Session.CurrentUser.UserEmail}";
                UserLastname = $"Фамилия: {Session.CurrentUser.UserLastname}";
                UserBirthday = $"Дата рождения: {Session.CurrentUser.UserBirthday}";

                DataContext = this;

                LoadCart(Session.CurrentUser.UserId);
            }
        }

        private void LoadCart(int? userId)
        {
            if (userId == null) return;

            using var db = new User9Context();

            var cartStatus = db.Statuses.FirstOrDefault(s => s.StatusName == "Cart");
            if (cartStatus == null) return;

            var cart = db.Orders
                .Where(o => o.UserId == userId && o.StatusId == cartStatus.StatusId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Instrument)
                .FirstOrDefault();
            var cartProd = db.OrderItems
                .Where(oi => oi.Order.UserId == userId && oi.Order.StatusId == cartStatus.StatusId)
                .Include(oi => oi.Instrument)
                .ToList();
            CartListBox.ItemsSource = null;
            CartListBox.ItemsSource = cartProd;
            if (cart != null && cart.OrderItems != null && cart.OrderItems.Any())
            {
                foreach (var item in cart.OrderItems)
                {
                    if (item.Instrument == null) continue; // Если вдруг нет инструмента, пропускаем

                    CartItems.Add(new CartItemViewModel
                    {
                        InstrumentId = item.InstrumentId,
                        InstrumentName = item.Instrument.InstrumentName,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }
            }
        }

        private void DeleteItem_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int instrumentId)
            {
                using var db = new User9Context();

                var cartStatus = db.Statuses.FirstOrDefault(s => s.StatusName == "Cart");
                if (cartStatus == null) return;

                var order = db.Orders
                    .FirstOrDefault(o => o.UserId == Session.CurrentUser.UserId && o.StatusId == cartStatus.StatusId);

                if (order == null) return;

                var orderItem = db.OrderItems
                    .FirstOrDefault(oi => oi.OrderId == order.OrderId && oi.InstrumentId == instrumentId);

                if (orderItem != null)
                {
                    db.OrderItems.Remove(orderItem);
                    db.SaveChanges();
                }

                LoadCart(Session.CurrentUser.UserId);
            }
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Button_Click(object? sender, RoutedEventArgs e)
        {
            var newWindow = new MainAppWindow();
            newWindow.Show();
            this.Close();
        }
    }

    public class CartItemViewModel
    {
        public int InstrumentId { get; set; }
        public string InstrumentName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
