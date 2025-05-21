using magazine_music.Context;
using Microsoft.EntityFrameworkCore;
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

            var cartStatus = db.Statuses.FirstOrDefault(s => s.StatusName == "Cart");
            if (cartStatus == null) return;
            // Найдём или создадим заказ-корзину
            var cart = db.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.UserId == userId && o.StatusId == cartStatus.StatusId);
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
                if (instrument == null) return;

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
