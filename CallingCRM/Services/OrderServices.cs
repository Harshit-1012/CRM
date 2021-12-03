using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class OrderServices
    {
        public hwLiveEntities _context = new hwLiveEntities();

        public CommonHelper _helper = new CommonHelper();


        public string GetInvoiceNumber()
        {
            var result = _helper.GetNextInvoiceNumber();
            return result;
        }

        public Order GetOrderById(long? id,string UserId)
        {
            var result = new Order();
            if (id != null)
            {
                result = _context.Orders.Where(x => x.Id == id && x.AdminId == UserId)
                                         
                                .FirstOrDefault();
            }
            return result;
        }

        public List<Order> GetOrdersListByClient(string UserId)
        {
            
            List<Order> orders = _context.Orders.Where(x => x.AdminId == UserId).ToList();
            return orders;
        }

        public bool GetOrderListByInvoiceNumber(string InvoiceNumber, long? OrderID)
        {
            var ClientID = ConfigurationManager.AppSettings["ClientId"];
            if (OrderID != null)
            {
                return _context.Orders.Where(x => x.InvoiceNumber == InvoiceNumber && x.Id != OrderID && x.AdminId == ClientID).Any();
            }
            else
            {
                return _context.ProductsServices.Where(x => x.Id == OrderID).Any();
            }
        }

        public void UpdateOrders(Order order)
        {
            ProductsService products = _context.ProductsServices.Where(x => x.Id == order.ProductId).FirstOrDefault();
            Order orders = _context.Orders.Where(x => x.Id == order.Id).FirstOrDefault();
            if (orders != null)
            {
                orders.ProductId = products.Id;
                orders.ProductPrice = order.ProductPrice;
                orders.DiscountPrice = order.DiscountPrice;
                orders.ProductSalePrice = order.ProductSalePrice;
                orders.Quantity = order.Quantity;
                orders.TAX = order.TAX;
                orders.PurchaseDate = order.PurchaseDate;
                orders.TransactionDetails = order.TransactionDetails;
                orders.OrderStatus = order.OrderStatus;
                orders.PaymentStatus = order.PaymentStatus;
                orders.IsActive = order.IsActive;
                orders.Comment = order.Comment;
                
            }
            else
            {
                Order neworder = new Order();
                neworder.ProductId = products.Id;
                neworder.InvoiceNumber = _helper.GetNextInvoiceNumber();
                neworder.ProductPrice = order.ProductPrice;
                neworder.DiscountPrice = order.DiscountPrice;
                neworder.ProductSalePrice = order.ProductSalePrice;
                neworder.Quantity = order.Quantity;
                neworder.TAX = order.TAX;
                neworder.PurchaseDate = order.PurchaseDate;
                neworder.TransactionDetails = order.TransactionDetails;
                neworder.OrderStatus = order.OrderStatus;
                neworder.PaymentStatus = order.PaymentStatus;
                neworder.IsActive = order.IsActive;
                neworder.Comment = order.Comment;
                neworder.CreatedAt = _helper.GetCurrentDate();
                neworder.AdminId = ConfigurationManager.AppSettings["ClientID"];
                _context.Orders.Add(neworder);
            }
            _context.SaveChanges();

        }
    }
}