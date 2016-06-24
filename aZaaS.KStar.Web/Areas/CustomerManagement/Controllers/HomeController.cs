
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Web.Areas.CustomerManagement.Models;
using aZaaS.KStar.Web.Areas.CustomerManagement.Fakes;
using aZaaS.KStar.Web.Areas.CustomerManagement.Exceptions;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly FakeDataSource _ds;

        public HomeController()
        {
            _ds = new FakeDataSource();
        }


        //
        // GET: /Customer_Management/Home/
        public ActionResult Index()
        {
            return PartialView("Index");
        }

        [HttpPost]
        public ActionResult Get(string customerID)
        {
            if (string.IsNullOrEmpty(customerID))
                throw new ArgumentNullException("customerID");

            var customer = _ds.GetCustomer(customerID);
            if (customer == null)
                throw new CustomerNotFoundException("The customer was not found");

            return Json(customer);
        }

        [HttpPost]
        public ActionResult List(CustomerSearchCriteria filter)
        {
            var customers = _ds.GetCustomers();

            var totalCount = customers.Count;
            customers = filter.Apply(customers, () =>
            {
                //Apply custom serching
                IEnumerable<CustomerModel> items = customers;

                if (!string.IsNullOrEmpty(filter.searchPhrase))
                    items = items.Where(c => c.CustomerID.Contains(filter.searchPhrase)
                                            || c.CompanyName.Contains(filter.searchPhrase));

                if (!string.IsNullOrEmpty(filter.City))
                    items = items.Where(c => c.City == filter.City);

                if (!string.IsNullOrEmpty(filter.Country))
                    items = items.Where(c => c.Country == filter.Country);

                return items;
            }, (items, field, dir) =>
            {
                //Apply custom ordering
                return items.OrderBy(c => c.CustomerID);
            }).ToList();

            return Json(new GridViewDataResult<CustomerModel>(customers, filter, totalCount));
        }

        [HttpPost]
        public ActionResult Add(CustomerModel customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            if (!ModelState.IsValid)
                throw new CustomerValidateFailException("The customer data was invalid");

            _ds.AddCustomer(customer);

            return Json(customer);
        }

        [HttpPost]
        public ActionResult Edit(CustomerModel customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var current = _ds.GetCustomer(customer.CustomerID);
            if (current == null)
                throw new CustomerNotFoundException("The customer was not found");

            _ds.UpdateCustomer(current, customer);

            return Json(customer);
        }

        [HttpPost]
        public ActionResult Remove(string[] customerIds)
        {
            if (customerIds == null || !customerIds.Any())
                throw new ArgumentNullException("customerIds");

            foreach (var customerId in customerIds)
            {
                var customer = _ds.GetCustomer(customerId);
                if (customer == null)
                    throw new CustomerNotFoundException("The customer was not found");

                _ds.RemoveCustomer(customer);
            }

            return Json(customerIds);
        }

        [HttpPost]
        public ActionResult Countries()
        {
            return Json(_ds.GetCountries());
        }

        [HttpPost]
        public ActionResult Cities(string country)
        {
            if (string.IsNullOrEmpty(country))
                throw new ArgumentNullException("country");

            return Json(_ds.GetCities(country));
        }
    }
}
