using CallingCRM.Models;
using CallingCRM.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CallingCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {

        public ProductServices _productServices = new ProductServices();
        public OrderServices _orderServices = new OrderServices();


        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        [HttpGet]
        [Route("/Product/AddProducts/{id?}")]
        public ActionResult AddProducts(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            ProductsService ThisProduct = new ProductsService();

            if (id != null)
            {
                ThisProduct = _productServices.GetProductById(id, UserId);
            }
           
            return View(ThisProduct);
        }

        [HttpGet]
        public ActionResult ProductList()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var product = _productServices.GetProductsListByClient(UserId);
            ViewBag.Products = product;

            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddProducts(HttpPostedFileBase ProductImage, ProductsService product)
        {
            var _existingproduct = _productServices.getProductByCode(product.ProductCode, product.Id);

            if (_existingproduct)
            {
                Session["eMessage"] = "Product with this Product Code already exists.";
            }

            try
            {
                if (ProductImage != null && ProductImage.FileName != null)
                {
                    var filename = Path.GetFileName(ProductImage.FileName);
                    var filePath = Server.MapPath("~/Uploads/Products/Clients/" + ConfigurationManager.AppSettings["clientID"]);
                    var virtualPath = "/Uploads/Products/Clients/" + ConfigurationManager.AppSettings["clientID"] + "/" + filename;
                    if (!System.IO.Directory.Exists(filePath))
                    {
                        System.IO.Directory.CreateDirectory(filePath);
                    }
                    var path = Path.Combine(filePath, filename);
                    ProductImage.SaveAs(path);

                    product.ProductImage = virtualPath;
                }

                _productServices.UpdateProduct(product);
                Session["sMessage"] = "Details has been saved successfully.";

                return RedirectToAction("/ProductList");
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;

            }
            return View(product);
        }

        [HttpGet]
        [Route("/Product/RemoveProductImage/{id}")]
        public ActionResult RemoveProductImage(long id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            ProductsService ThisProduct = _productServices.GetProductById(id, UserId);
            var path = ThisProduct.ProductImage;
            var filePath = Server.MapPath(path);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                file.Delete();
            }
            ThisProduct.ProductImage = null;

            _productServices.UpdateProduct(ThisProduct);

            return RedirectToAction("AddProducts/"+ id);
        }


        [HttpGet]
        [Route("{id?}")]
        public ActionResult CreateOrders(long? id)
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            Order ThisOrder = new Order();

            //ThisOrder = _orderServices.GetInvoiceNumber();

            List<SelectListItem> OrderStatus = new List<SelectListItem>();
            OrderStatus.Add(new SelectListItem() { Text = "---Select---",Value="" });
            OrderStatus.Add(new SelectListItem() { Text = "Confirm", Value = "Confirm" });
            OrderStatus.Add(new SelectListItem() { Text = "Packing", Value = "Packing" });
            OrderStatus.Add(new SelectListItem() { Text = "Dispatched", Value = "Dispatched" });
            OrderStatus.Add(new SelectListItem() { Text = "Delivered", Value = "Delivered" });

            ViewBag.OrderStatus = OrderStatus;

            List<SelectListItem> PaymentStatus = new List<SelectListItem>();
            PaymentStatus.Add(new SelectListItem() { Text = "---Select---", Value = "" });
            PaymentStatus.Add(new SelectListItem() { Text = "Pending", Value = "Pending" });
            PaymentStatus.Add(new SelectListItem() { Text = "Successful", Value = "Successful" });
            PaymentStatus.Add(new SelectListItem() { Text = "Cancelled", Value = "Cancelled" });

            ViewBag.PaymentStatus = PaymentStatus;

            if (id != null)
            {
                ThisOrder = _orderServices.GetOrderById(id, UserId);
            }

            ViewBag.ThisOrder = ThisOrder;

            return View(ThisOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrders(Order order)
        {
            var _existingOrder = _orderServices.GetOrderListByInvoiceNumber(order.InvoiceNumber, order.Id);

            if (_existingOrder)
            {
                Session["eMessage"] = "Order with this Invoice Number already exists.";
            }

            try
            {
                
                _orderServices.UpdateOrders(order);
                Session["sMessage"] = "Details has been saved successfully.";

                return RedirectToAction("/OrderList");
            }
            catch (Exception ex)
            {
                Session["eMessage"] = ex.Message;

            }
            return View(order);
        }


        [HttpGet]
        public ActionResult OrderList()
        {
            var UserId = AuthenticationManager.User.Identity.GetUserId();
            var orders = _orderServices.GetOrdersListByClient(UserId);
            ViewBag.order = orders;

            return View();
        }

    }
}