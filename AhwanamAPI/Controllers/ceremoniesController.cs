﻿using MaaAahwanam.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AhwanamAPI.Controllers
{
    public class ceremoniesController : ApiController
    {
        public class ceremony
        {
            public long ceremony_id { get; set; }
            public string ceremony_name { get; set; }
            public string thumb_image { get; set; }
            public string page_name { get; set; }
            public string description { get; set; }
            public string short_description { get; set; }
            public string cermony_image { get; set; }
            public string city { get; set; }
            public List<filters> filters { get; set; }
            public List<categories> categories { get; set; }
           
        }

        public class filters
        {
            public string name { get; set; }
            public string display_name { get; set; }
            public List<values> values { get; set; }
        }

        public class values
        {
            public string name { get; set; }
            public int id { get; set; }
        }

        public class categories
        {
            public string name { get; set; }
            public string sub_title { get; set; }
            public string thumb_image { get; set; }
            public string page_name { get; set; }
            public long serviceId { get; set; }
          public List<vendors> vendors { get; set; }
            
        }

        public class price
        {
            public string actual_price { get; set; }
            public string offer_price { get; set; }
            public string service_price { get; set; }

        }

        public class vendors
        {
            public string category_name { get; set; }
            public string charge_type { get; set; }
            public string city { get; set; }
            public string name { get; set; }
            public string page_name { get; set; }
            public string pic_url { get; set; }
            public decimal rating { get; set; }
            public string reviews_count { get; set; }
            public price price { get; set; }
   
        }
        public class value
        {
            public string name { get; set; }
            public long id { get; set; }
        }
        public string getcity(int id)
        {
            VendorMasterService vendorMasterService = new VendorMasterService();
            var data = vendorMasterService.SearchVendors();
            var citylist = data.Select(m => m.City).Distinct().ToList();
            List<value> val1 = new List<value>();
            for (int i = 1; i < citylist.Count; i++)
            {
                value c = new value();
                c.name = citylist[i];
                c.id = i;
                val1.Add(c);
            }
            string city = val1.Where(m => m.id == id).FirstOrDefault().name;
            return city;
        }

        [HttpGet]
        [Route("api/ceremonies/details")]
        public IHttpActionResult ceremonydetail(string ceremony, int? city = 0)
        {
            FilterServices filterServices = new FilterServices();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            CeremonyServices ceremonyServices = new CeremonyServices();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            ceremony d = new ceremony();
            var details = ceremonyServices.Getdetails(ceremony).ToList();
            var detail = details.FirstOrDefault();
            d.ceremony_id = detail.Id;
            d.ceremony_name = detail.Title;
            d.thumb_image = detail.Image;
            d.page_name = detail.page_name1;
            d.description = detail.Description;
            d.cermony_image = detail.ceremonyImage;
            d.city = cityvalue;
            //VendorMasterService vendorMasterService = new VendorMasterService();
            //var result = vendorMasterService.SearchVendors();
            //var citylist = result.Select(m => m.City).Distinct().ToList();
            //filters f = new filters();
            //List<filters> f1 = new List<filters>();
            //f.name = "City";
            //f.display_name = "city"; List<values> test = new List<values>();
            //for (int j = 1; j < citylist.Count; j++)
            //{
            //    values v = new values();
            //    v.name = citylist[j];
            //    v.id = j;
            //    test.Add(v);
            //}
            //f.values = test;
            //f1.Add(f);
            List<categories> categories = new List<categories>();
            for (int i = 0; i < details.Count; i++)
            {
                categories categories1 = new categories();
                categories1.name = details[i].Category;
                categories1.sub_title = details[i].Description;
                categories1.page_name = details[i].page_name2;
                categories1.serviceId = details[i].Id;
                if(details[i].page_name2 == "venue")
                {
                    categories1.vendors = venuedetails(details[i].page_name2, city);
                }
             if (details[i].page_name2 == "photography")
                {
                    categories1.vendors = photographerdetails(details[i].page_name2, city);
                }
              if (details[i].page_name2 == "decorator")
                {
                    categories1.vendors = decoratordetails(details[i].page_name2, city);
                }
             if (details[i].page_name2 == "catering")
                {
                    categories1.vendors = catererdetails(details[i].page_name2, city);
                }

              if (details[i].page_name2 == "pandit" || details[i].page_name2 == "mehendi")
                {
                    categories1.vendors = otherdetails(details[i].page_name2, city);
                }
                    categories.Add(categories1);
            }
            //d.filters = f1;
            d.categories = categories;
            dict.Add("status", true);
            dict.Add("message", "Success");
            dict.Add("data", d);
            return Json(dict);
        }
       public List<vendors> venuedetails(string type, int? city = 0)
        {
            ResultsPageService resultsPageService = new ResultsPageService();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            type = (type == "venue") ? "Venue" : type;
            var data = resultsPageService.GetAllVendors(type);
            if (cityvalue != null)
                data = data.Where(m => m.City == cityvalue).ToList();
            List<vendors> param = new List<vendors>();
                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {

                        decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                        vendors p = new vendors();
                        //prices Section
                        price price = new price();
                        price.actual_price = item.cost1.ToString();
                        price.offer_price = item.normaldays;
                        price.service_price = item.ServiceCost.ToString();
                        //Data Section
                        p.name = item.BusinessName;
                        p.page_name = item.page_name;
                        p.category_name = item.ServicType;
                        ReviewService reviewService = new ReviewService();
                        p.reviews_count = reviewService.GetReview(int.Parse(item.Id.ToString())).Where(m => m.Sid == long.Parse(item.subid.ToString())).Count().ToString();
                        p.rating = (trating != 0) ? decimal.Parse((trating / 3).ToString().Substring(0, 4)) : 0;
                        p.charge_type = "Per Day";
                        p.city = item.City;
                        p.pic_url = "https://api.ahwanam.com/vendorimages/" + item.image;
                        p.price = price;
                        param.Add(p);
                    }
                }
            var records = param;
            var ratingvalue = "4";
            if (ratingvalue != null)
                records = records.Where(m => m.rating >= decimal.Parse(ratingvalue)).ToList();
            return records.Take(7).ToList();
        }
        public List<vendors> photographerdetails(string type, int? city = 0)
        {
            ResultsPageService resultsPageService = new ResultsPageService();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            var data = resultsPageService.GetAllPhotographers();
            if (cityvalue != null)
                data = data.Where(m => m.City == cityvalue).ToList();
            List<vendors> param = new List<vendors>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {

                    decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                    vendors p = new vendors();
                    //prices Section
                    price price = new price();
                    price.actual_price = item.cost1.ToString();
                    price.offer_price = item.cost1.ToString(); // Add Normal Days price here
                    price.service_price = "";
                    //Data Section
                    p.name = item.BusinessName;
                    p.page_name = item.page_name;
                    p.category_name = item.ServicType;
                    ReviewService reviewService = new ReviewService();
                    p.reviews_count = reviewService.GetReview(int.Parse(item.Id.ToString())).Where(m => m.Sid == long.Parse(item.subid.ToString())).Count().ToString();
                    p.rating = (trating != 0) ? decimal.Parse((trating / 3).ToString().Substring(0, 4)) : 0;
                    p.charge_type = "Per Day";
                    p.city = item.City;
                    p.pic_url = "https://api.ahwanam.com/vendorimages/" + item.image;
                    p.price = price;
                    param.Add(p);
                 
                }
            }
            var records = param;
            var ratingvalue = "4";
            if (ratingvalue != null)
                records = records.Where(m => m.rating >= decimal.Parse(ratingvalue)).ToList();
            return records.Take(7).ToList();
        }

        public List<vendors> decoratordetails(string type, int? city = 0)
        {
            ResultsPageService resultsPageService = new ResultsPageService();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            var data = resultsPageService.GetAllDecorators();
            if (cityvalue != null)
                data = data.Where(m => m.City == cityvalue).ToList();
            List<vendors> param = new List<vendors>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {

                    decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                    vendors p = new vendors();
                    //prices Section
                    price price = new price();
                    price.actual_price = item.cost1.ToString();
                    price.offer_price = item.cost1.ToString();
                    price.service_price = "";
                    //Data Section
                    p.name = item.BusinessName;
                    p.page_name = item.page_name;
                    p.category_name = item.ServicType;
                    ReviewService reviewService = new ReviewService();
                    p.reviews_count = reviewService.GetReview(int.Parse(item.Id.ToString())).Where(m => m.Sid == long.Parse(item.subid.ToString())).Count().ToString();
                    p.rating = (trating != 0) ? decimal.Parse((trating / 3).ToString().Substring(0, 4)) : 0;
                    p.charge_type = "Per Day";
                    p.city = item.City;
                    p.pic_url = "https://api.ahwanam.com/vendorimages/" + item.image;
                    p.price = price;
                    param.Add(p);
                }
            }
            var records = param;
            var ratingvalue = "4";
            if (ratingvalue != null)
                records = records.Where(m => m.rating >= decimal.Parse(ratingvalue)).ToList();
            return records.Take(7).ToList();
        }
        public List<vendors> catererdetails(string type, int? city = 0)
        {
            ResultsPageService resultsPageService = new ResultsPageService();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            int count = 0;
            var data = resultsPageService.GetAllCaterers();
            if (cityvalue != null)
                data = data.Where(m => m.City == cityvalue).ToList();
            List<vendors> param = new List<vendors>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                    vendors p = new vendors();
                    //prices Section
                    price price = new price();
                    price.actual_price = item.Veg.ToString();
                    price.offer_price = item.Veg.ToString(); // Add Normal Days price here
                    price.service_price = "";
                    //Data Section
                    p.name = item.BusinessName;
                    p.page_name = item.page_name;
                    p.category_name = item.ServicType;
                    ReviewService reviewService = new ReviewService();
                    p.reviews_count = reviewService.GetReview(int.Parse(item.Id.ToString())).Where(m => m.Sid == long.Parse(item.subid.ToString())).Count().ToString();
                    p.rating = (trating != 0) ? decimal.Parse((trating / 3).ToString().Substring(0, 4)) : 0;
                    p.charge_type = "Per Day";
                    p.city = item.City;
                    p.pic_url = "https://api.ahwanam.com/vendorimages/" + item.image;
                    p.price = price;
                    param.Add(p);
                }
             
            }
            var records = param;
            var ratingvalue = "4";
            if (ratingvalue != null)
                records = records.Where(m => m.rating >= decimal.Parse(ratingvalue)).ToList();
            return records.Take(7).ToList();
        }

        public List<vendors> otherdetails(string type, int? city = 0)
        {
            ResultsPageService resultsPageService = new ResultsPageService();
            string cityvalue = (city != 0 && city != null) ? getcity((int)city) : null;
            int count = 0;
            var data = resultsPageService.GetAllOthers(type);
            if (cityvalue != null)
                data = data.Where(m => m.City == cityvalue).ToList();
            List<vendors> param = new List<vendors>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                    vendors p = new vendors();

                    //prices Section
                    price price = new price();
                    price.actual_price = item.ItemCost.ToString();
                    price.offer_price = item.ItemCost.ToString(); // Add Normal Days price here
                    price.service_price = "";
                    //Data Section
                    p.name = item.BusinessName;
                    p.page_name = item.page_name;
                    p.category_name = item.ServicType;
                    ReviewService reviewService = new ReviewService();
                    p.reviews_count = reviewService.GetReview(int.Parse(item.Id.ToString())).Where(m => m.Sid == long.Parse(item.subid.ToString())).Count().ToString();
                    p.rating = (trating != 0) ? decimal.Parse((trating / 3).ToString().Substring(0, 4)) : 0;
                    p.charge_type = "Per Day";
                    p.city = item.City;
                    p.pic_url = "https://api.ahwanam.com/vendorimages/" + item.image;
                    p.price = price;
                    param.Add(p);

                }
            }
            var records = param;
            var ratingvalue = "4";
            if (ratingvalue != null)
                records = records.Where(m => m.rating >= decimal.Parse(ratingvalue)).ToList();
            return records.Take(7).ToList();
        }

    }
}
