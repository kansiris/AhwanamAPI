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
            //public List<filters> filters { get; set; }
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
            for (int i = 0; i < citylist.Count; i++)
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
        public IHttpActionResult ceremonydetails(string ceremony, int? city = -1)
        {
            int count = 0;
            FilterServices filterServices = new FilterServices();
            string cityvalue = (city != -1 && city != null) ? getcity((int)city) : null;
            CeremonyServices ceremonyServices = new CeremonyServices();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            ceremony d = new ceremony();
            var detail = ceremonyServices.Getall().Where(c => c.page_name == ceremony).FirstOrDefault();
            d.ceremony_id = detail.Id;
            d.ceremony_name = detail.Title;
            d.thumb_image = detail.Image;
            d.page_name = detail.page_name;
            d.description = detail.Description;
            d.cermony_image = detail.ceremonyImage;
            d.city = cityvalue;
            VendorMasterService vendorMasterService = new VendorMasterService();
                var result = vendorMasterService.SearchVendors();
                var citylist = result.Select(m => m.City).Distinct().ToList();
                if (city != null)
                    result = result.Where(m => m.City == cityvalue).ToList();
                filters f = new filters();
                List<filters> f1 = new List<filters>();
                f.name = "City";
                f.display_name = "city"; List<values> test = new List<values>();
                for (int j = 0; j < citylist.Count; j++)
                {
                    values v = new values();
                    v.name = citylist[j];
                    v.id = j;
                    test.Add(v);
                }
                f.values = test;
                f1.Add(f);
            List<categories> categories = new List<categories>();
            var details = ceremonyServices.getceremonydetails(d.ceremony_id).ToList();
            for (int i = 0; i < details.Count; i++)
            {  
               categories categories1 = new categories();
                categories1.name = details[i].Category;
                categories1.thumb_image = details[i].image;
                categories1.sub_title = details[i].Description;
                categories1.page_name = details[i].page_name; 
                categories1.serviceId = details[i].Id;
                ResultsPageService resultsPageService = new ResultsPageService();
                List<vendors> param = new List<vendors>();
                if (details[i].page_name == "venue")
                {
                    var data1 = resultsPageService.GetAllVendors("Venue").Take(7).ToList();
                    if(data1.Count>0)
                    { 
                    foreach (var item in data1)
                    {
                          
                        decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                        vendors p = new vendors();
                        //prices Section
                        price price = new price();
                        price.actual_price = item.cost1.ToString();
                        price.offer_price = item.normaldays;
                        price.service_price = item.ServiceCost.ToString();
                        //p.filters = f1;
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
                }
                else if (details[i].page_name == "decorator")
                {
                    var data1 = resultsPageService.GetAllDecorators().Take(7).ToList();
                    if (data1.Count > 0)
                    {
                        foreach (var item in data1)
                        {

                            decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                            vendors p = new vendors();
                            //prices Section
                            price price = new price();
                            price.actual_price = item.cost1.ToString();
                            price.offer_price = item.cost1.ToString();
                            price.service_price = "";
                            //p.filters = f1;
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
                    };
                }
                else if (details[i].page_name == "catering")
                {
                    var data1 = resultsPageService.GetAllCaterers().Take(7).ToList();
                    if (data1.Count > 0 && data1.Count <= 7)
                    {
                        foreach (var item in data1)
                        {
                            decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                            vendors p = new vendors();
                            //prices Section
                            price price = new price();
                            price.actual_price = item.Veg.ToString();
                            price.offer_price = item.Veg.ToString(); // Add Normal Days price here
                            price.service_price = "";
                            //p.filters = f1;
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
                }
                else if (details[i].page_name == "photography")
                {
                    var data1 = resultsPageService.GetAllPhotographers().Take(7).ToList();
                    if (data1.Count > 0 && data1.Count <= 7)
                    {
                        foreach (var item in data1)
                        {

                            decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                            vendors p = new vendors();
                            //prices Section
                            price price = new price();
                            price.actual_price = item.cost1.ToString();
                            price.offer_price = item.cost1.ToString(); // Add Normal Days price here
                            price.service_price = "";
                            //p.filters = f1;
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
                }
                else if (details[i].page_name == "pandit" || details[i].page_name == "mehendi")
                {
                    var data1 = resultsPageService.GetAllOthers(details[i].page_name).Take(7).ToList();
                    if (data1.Count > 0 && data1.Count <= 7)
                    {
                        foreach (var item in data1)
                        {
                            decimal trating = (item.fbrating != null && item.googlerating != null && item.jdrating != null) ? decimal.Parse(item.fbrating) + decimal.Parse(item.googlerating) + decimal.Parse(item.jdrating) : 0;
                            vendors p = new vendors();

                            //prices Section
                            price price = new price();
                            price.actual_price = item.ItemCost.ToString();
                            price.offer_price = item.ItemCost.ToString(); // Add Normal Days price here
                            price.service_price = "";
                            //p.filters = f1;
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
                }
                if(param.Count!= 0)
                {
                    categories1.vendors = param;
                }
               categories.Add(categories1);
            }
            d.filters = f1;
            d.categories = categories;
            dict.Add("status", true);
            dict.Add("message", "Success");
            dict.Add("data", d);
            return Json(dict);
        }
    }
}
