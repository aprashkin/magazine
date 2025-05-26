using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magazine_music
{
    public static class CartService
    {
        public static void AddToCart(int userId, int instrumentId)
        {
            using var db = new User9Context();

            var cartStatus = db.Statuses.FirstOrDefault(s => s.StatusName == "В корзине");      
            var cart = db.Orders.FirstOrDefault(o => o.UserId == userId && o.StatusId == cartStatus.StatusId);

            if (cart == null)
            {
                cart = new Order
                {
                    UserId = userId,
                    StatusId = cartStatus.StatusId,
                    OrderDate = DateTime.Now,
                    TotalAmount = 0,
                    Address = "test"
                };
                db.Orders.Add(cart);
                db.SaveChanges();

                Console.WriteLine("Создан заказ для пользователя: " + userId);
                MessageBoxManager.GetMessageBoxStandard("Успешно", "Товар добавлен в корзину" ).ShowAsync();
            }

            var existingItem = db.OrderItems
                .FirstOrDefault(oi => oi.OrderId == cart.OrderId && oi.InstrumentId == instrumentId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                var instrument = db.Instruments.FirstOrDefault(i => i.InstrumentId == instrumentId);
                if (instrument == null)
                {
                    Console.WriteLine("Инструмент не найден");
                    return;
                }

                db.OrderItems.Add(new OrderItem
                {
                    OrderId = cart.OrderId,
                    InstrumentId = instrumentId,
                    Quantity = 1,
                    Price = (decimal)instrument.InstrumentPrice
                });
            }

            db.SaveChanges();
        }

    }
}
