using Lab3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lab3.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowCustomer()
        {
            Customer customer = new Customer
            {
                FirstName = "Jane",
                LastName = "Doe",
                CustomerNumber = "1234"
            };

            return View("Customer",customer);
        }

        public ActionResult AddCustmoer()
        {
            return View();
        }

        public ActionResult SubmitCustomerDetails(Customer customer)
        {
           /* Customer customer = new Customer
            {
                FirstName = Request.Form["FirstName"],
                LastName = Request.Form["LastName"],
                CustomerNumber = Request.Form["CustomerNumber"],
            };*/
            return View("Customer", customer);
        }
    }
}