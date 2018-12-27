﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MaaAahwanam.Models;
using MaaAahwanam.Service;
using MaaAahwanam.Repository;

namespace AhwanamAPI.Controllers
{
    public class resultsController : ApiController
    {
        ResultsPageService resultsPageService = new ResultsPageService();

        [HttpGet]
        [Route("api/results/getall")]
        public IHttpActionResult getrecords(string type)
        {
            type = (type == null) ? "Venue" : type;
            var data = resultsPageService.GetAllVendors(type);
            return Json(data);
        }

        [HttpGet]
        [Route("api/results/search")]
        public IHttpActionResult searchvendor(string name, string type)
        {
            //Filter Vendors by Name
            type = (type == null) ? "Venue" : type;
            var data = resultsPageService.GetVendorsByName(type, name);
            return Json(data);
        }

        [HttpGet]
        [Route("api/results/load")]
        public IHttpActionResult loadmore(string type, string range)
        {
            // CheckBox Checked
            type = (type == null) ? "Venue" : type;
            var selectedservices = type.Split(',');
            var data = vendorlist(6, selectedservices, "first", 6);
            if (range != null)
                data = data.Where(m => m.cost1 >= decimal.Parse(range.Split(',')[0]) && m.cost1 <= decimal.Parse(range.Split(',')[1])).ToList();
            return Json(data);
        }

        [HttpGet]
        [Route("api/results/lazy")]
        public IHttpActionResult lazyload(string count, string type, string range)
        {
            // On Scroll/Lazy Load
            if (range == null) range = "0,0";
            int takecount = (count == "" || count == null) ? 6 : int.Parse(count) * 6;
            List<GetVendors_Result> vendorslist = vendorlist(6, type.Split(','), "next", takecount).Where(m => m.cost1 >= decimal.Parse(range.Split(',')[0]) && m.cost1 <= decimal.Parse(range.Split(',')[1])).ToList();
            return Json(vendorslist);
        }

        public List<GetVendors_Result> vendorlist(int count, string[] selectedservices, string command, int takecount)
        {
            List<GetVendors_Result> list = new List<GetVendors_Result>();
            int recordcount = count / selectedservices.Count();
            takecount = takecount / selectedservices.Count();
            for (int i = 0; i < selectedservices.Count(); i++)
            {
                selectedservices[i] = (selectedservices[i] == "Convention" || selectedservices[i] == "Banquet" || selectedservices[i] == "Function") ? selectedservices[i] + " Hall" : selectedservices[i];
                var getrecords = resultsPageService.GetAllVendors(selectedservices[i]);
                if (command == "next")
                    getrecords = getrecords.Skip(takecount).Take(recordcount).ToList();
                else if (command == "first")
                    getrecords = getrecords.Take(recordcount).ToList();
                list.AddRange(getrecords);
            }
            return list;
        }
    }
}
