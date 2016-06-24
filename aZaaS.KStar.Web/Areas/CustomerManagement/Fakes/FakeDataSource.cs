using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using aZaaS.KStar.Web.Areas.CustomerManagement.Models;

namespace aZaaS.KStar.Web.Areas.CustomerManagement.Fakes
{
    public class FakeDataSource
    {
        private readonly static List<CityModel> _fakeCities = new List<CityModel>();
        private readonly static List<CountryModel> _fakeCountries = new List<CountryModel>();
        private readonly static List<CustomerModel> _fakeCustomers = new List<CustomerModel>();

        static FakeDataSource()
        {
            _fakeCustomers = new List<CustomerModel>()
            {
                new CustomerModel(){ CustomerID = "LONEP", CompanyName = "Lonesome Pine Restaurant", ContactName = "Fran Wilson" , ContactTitle = "Sales Manager", Country = "USA", City = "Albuquerque",Region="OR", Address ="89 Chiaroscuro Rd.", PostalCode="97219", Phone="(503) 555-9573",Fax="(503) 555-9646"},
                new CustomerModel(){ CustomerID = "MAGAA", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "MEREP", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "MORGK", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "OCEAN", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "OTTIK", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "PRINI", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "QUEDE", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "QUEEN", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "QUICK", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "RATTC", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "RICAR", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"},
                new CustomerModel(){ CustomerID = "PERIC", CompanyName = "Magazzini Alimentari Riuniti", ContactName = "Giovanni Rovelli" , ContactTitle = "Marketing Manager", Country = "Italy", City = "Bergamo",Region="OR", Address ="Via Ludovico il Moro 22", PostalCode="24100", Phone="035-640230",Fax="035-640230"}
            };

            _fakeCountries = new List<CountryModel>()
            {
                new CountryModel(){ CountryCode = "USA", CountryName = "USA"},
                new CountryModel(){ CountryCode = "Italy", CountryName = "Italy"},
                new CountryModel(){ CountryCode = "UK", CountryName = "UK"},
                new CountryModel(){ CountryCode = "Sweden", CountryName = "Sweden"},
                new CountryModel(){ CountryCode = "Poland", CountryName = "Poland"},
                new CountryModel(){ CountryCode = "Spain", CountryName = "Spain"},
                new CountryModel(){ CountryCode = "Canada", CountryName = "Canada"}
            };

            _fakeCities = new List<CityModel>()
            {
                new CityModel(){ CityCode = "Albuquerque", CityName ="Albuquerque", Country = "USA"},
                new CityModel(){ CityCode = "Boise", CityName ="Boise", Country = "USA"},
                new CityModel(){ CityCode = "Kirkland", CityName ="Kirkland", Country = "USA"},
                new CityModel(){ CityCode = "San Francisco", CityName ="San Francisco", Country = "USA"},

                new CityModel(){ CityCode = "Bergamo", CityName ="Bergamo", Country = "Italy"},
                new CityModel(){ CityCode = "Torino", CityName ="Torino", Country = "Italy"},

                new CityModel(){ CityCode = "Cowes", CityName ="Cowes", Country = "UK"},
                new CityModel(){ CityCode = "London", CityName ="London", Country = "UK"},

                new CityModel(){ CityCode = "Bräcke", CityName ="Bräcke", Country = "Sweden"},

                new CityModel(){ CityCode = "Warszawa", CityName ="Warszawa", Country = "Poland"},

                new CityModel(){ CityCode = "Barcelona", CityName ="Barcelona", Country = "Spain"},

                new CityModel(){ CityCode = "Sevilla", CityName ="Sevilla", Country = "Spain"},

                new CityModel(){ CityCode = "Tsawassen", CityName ="Tsawassen", Country = "Canada"},

            };

        }

        public CustomerModel GetCustomer(string customerID)
        {
            return _fakeCustomers.FirstOrDefault(c => c.CustomerID == customerID);
        }

        public IList<CustomerModel> GetCustomers()
        {
            return _fakeCustomers;
        }

        public void RemoveCustomer(CustomerModel customer)
        {
            if (customer != null)
                _fakeCustomers.Remove(customer);
        }

        public void UpdateCustomer(CustomerModel current,CustomerModel customer)
        {
            if (current != null && customer != null)
            {
                current.CompanyName = customer.CompanyName;
                current.ContactName = customer.ContactName;
                current.ContactTitle = customer.ContactTitle;
                current.Country = customer.Country;
                current.Region = customer.Region;
                current.City = customer.City;
                current.PostalCode = customer.PostalCode;
                current.Phone = customer.Phone;
                current.Fax = customer.Fax;
                current.Address = customer.Address;
            }
        }

        public IList<CountryModel> GetCountries()
        {
            return _fakeCountries;
        }

        public IList<CityModel> GetCities(string country)
        {
            return _fakeCities.Where(c => c.Country.Equals(country)).ToList();
        }

        public void AddCustomer(CustomerModel customer)
        {
            _fakeCustomers.Add(customer);
        }

    }
}