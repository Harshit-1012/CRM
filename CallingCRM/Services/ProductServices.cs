using CallingCRM.Helpers;
using CallingCRM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CallingCRM.Services
{
    public class ProductServices
    {

        public hwLiveEntities _context = new hwLiveEntities();

        public CommonHelper _helper = new CommonHelper();

        public bool getProductByCode(string productCode, long? productID)
        {
            if (productID != null)
            {
                return _context.ProductsServices.Where(x => x.ProductCode == productCode && x.Id != productID).Any();
            }
            else
            {
                return _context.ProductsServices.Where(x => x.ProductCode == productCode).Any();
            }
        }

        public List<ProductsService> GetProductsListByClient(string UserId)
        {
            //var ClientID = ConfigurationManager.AppSettings["ClientId"];
            List<ProductsService> products = _context.ProductsServices.Where(x => x.AdminId == UserId).ToList();
            return products;
        }

        public ProductsService GetProductById(long? id, string UserId)
        {
            var result = new ProductsService();
            if (id != null)
            {
                result = _context.ProductsServices.Where(x => x.Id == id && x.AdminId == UserId)
                   .FirstOrDefault();

            }

            return result;
        }

        public void UpdateProduct(ProductsService product)
        {

            ProductsService products = _context.ProductsServices.Where(x => x.Id == product.Id ).FirstOrDefault();
            if (products != null)
            {
                products.ProductTitle = product.ProductTitle;
                products.ProductCode = product.ProductCode;
                products.CostPrice = product.CostPrice;
                products.SellingPrice = product.SellingPrice;
                products.Description = product.Description;
                products.ProductImage = product.ProductImage;
                products.Summary = product.Summary;
                products.EmailBody = product.EmailBody;
                products.EmailSubject = product.EmailSubject;
                products.IsActive = product.IsActive;
            }
            else
            {
                ProductsService newproduct = new ProductsService();
                newproduct.ProductTitle = product.ProductTitle;
                newproduct.ProductCode = product.ProductCode;
                newproduct.CostPrice = product.CostPrice;
                newproduct.SellingPrice = product.SellingPrice;
                newproduct.Description = product.Description;
                newproduct.Summary = product.Summary;
                newproduct.ProductImage = product.ProductImage;
                newproduct.EmailBody = product.EmailBody;
                newproduct.EmailSubject = product.EmailSubject;
                newproduct.IsActive = product.IsActive;
                newproduct.CreatedAt = _helper.GetCurrentDate();
                newproduct.AdminId = ConfigurationManager.AppSettings["ClientID"];
                _context.ProductsServices.Add(newproduct);
                
            }
            _context.SaveChanges();

        }
    }
}