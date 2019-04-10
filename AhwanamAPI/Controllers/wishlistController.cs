﻿using MaaAahwanam.Models;
using MaaAahwanam.Service;
using MaaAahwanam.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AhwanamAPI.Controllers
{
    public class wishlistController : ApiController
    {
        WhishListService wishlistservice = new WhishListService();
        UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();


        public class wishlist
        {
            public long wishlist_id { get; set; }
            public long user_id { get; set; }
            public string name { get; set; }
            public List<wishlistitems> wishlistitems { get; set; }
        }
        public class listItems
        {
            public long vendor_id { get; set; }
            public long wishlist_id { get; set; }
            //public long NotesId { get; set; }
            //public long collabratorid { get; set; }
        }
        public class AddNote
        {
            public long wishlist_id { get; set; }
            public long vendor_id { get; set; }
            public string Notes { get; set; }
        }

        public class EditNote
        {
            public long notes_id { get; set; }
            public string notes { get; set; }
        }

        public class detailids
        {
            public long vendor_id { get; set; }
            public long wishlist_id { get; set; }
            //public long user_id { get; set; }
        }

        public class UserCollabrator
        {
            //public long user_id { get; set; }
            public long wishlist_id { get; set; }
            public string email { get; set; }
            public string phoneNo { get; set; }
            //public DateTime UpdatedDate { get; set; }
        }

        public class DetailsCollabrator
        {
            public long collabrator_id { get; set; }
            public long user_id { get; set; }
            public long wishlist_id { get; set; }
            public string email { get; set; }
            public string phoneNo { get; set; }
            public string code { get; set; }
            //public DateTime UpdatedDate { get; set; }
        }
        //public class WishList
        //{
        //    public long UserId { get; set; }
        //    public string Name { get; set; }
        //    public string Event { get; set; }
        //    public DateTime? StartDate { get; set; }
        //    public DateTime? EndDate { get; set; }
        //    public string Description { get; set; }
        //    public DateTime UpdatedDate { get; set; }
        //}

        public class category
        {
            public long category_id { get; set; }
            public string category_name { get; set; }
            public vendor vendor { get; set; }
            
        }

        public class vendor
        {
            public string name { get; set; }
            public long vendor_id { get; set; }
            public int category_id { get; set; }
            public string category_name { get; set; }
            public long userwishlist_id { get; set; }
            public long reviews_count { get; set; }
            public decimal rating { get; set; }
            public string charge_type { get; set; }
            public string city { get; set; }
            public price price { get; set; }
            public string pic_url { get; set; }
            public long? collabrator_id { get; set; }
            public List<notes> notes { get; set; }
        }
        public class notes
        {
            public long notes_id { get; set; }
            public string notes_text { get; set; }
            public DateTime updatedatetime { get; set; }
        }

        public class userwishlist
        {
            public long wishlist_id { get; set; }
            public long user_id { get; set; }
            public string name { get; set; }
            public List<wishlistitems> wishlistitems { get; set; }
        }

        public class wishlistitems
        {
            public int category_id { get; set; }
            public string category_name { get; set; }
            public string page_name { get; set; }
            public List<vendors> vendors { get; set; } 
        }
        public class price
        {
            //public string Rentalprice { get; set; }
            public decimal minimum_price { get; set; }
            //public string maxprice { get; set; }
            //public string vegprice { get; set; }
            //public string nonvegprice { get; set; }
        }
        public class vendors
        {
            public long vendor_id { get; set; }
            //public long vendor_serviceId { get; set; }
            public int category_id { get; set; }
            public string name { get; set; }
            public string category_name { get; set; }
            public long reviews_count { get; set; }
            public decimal rating { get; set; }
            public string charge_type { get; set; }
            public string city { get; set; }
            public price price { get; set; }
            public string pic_url { get; set; }
            public  long contributor_id { get; set; }
        }

        public class UseNotes
        {
            public long notes_id { get; set; }
            public long wishlist_id { get; set; }
            public  long vendor_id { get; set; }
            public string notes { get; set; }
            public DateTime added_datetime { get; set; }
            public DateTime edited_datetime { get; set; }
        }

        [HttpGet]
        [Route("api/wishlist/getmywishlist")]
        public IHttpActionResult Addusertowishlist()
        {
            WishlistDetails wishlists = new WishlistDetails();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var details = userlogindetailsservice.Getmyprofile(token);
                if(details!=null)
                {
                    WhishListService wishlistservice1 = new WhishListService();
                    wishlists.UserId = details.UserLoginId;
                    wishlists.Name = details.FirstName + ' ' + details.LastName;
                    wishlists.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                  var userdata = wishlistservice1.Getuserfromwishlistbyuserid(wishlists.UserId);
                    if(userdata == null)
                    {
                    var data = wishlistservice1.AddUserwishlist(wishlists);
                    if (data != null)
                    {
                        wishlist list = new wishlist();
                        list.wishlist_id = data.WishlistdetailId;
                        list.user_id = data.UserId;
                        list.name = data.Name;
                       FilterServices filterServices = new FilterServices();
                       var categories = filterServices.AllCategories();
                            List<wishlistitems> categorys = new List<wishlistitems>();
                            for (int i = 0; i < categories.Count(); i++)
                            {
                                wishlistitems category = new wishlistitems();
                                category.category_id = categories[i].servicType_id;
                                category.category_name = categories[i].name;
                                category.page_name = categories[i].display_name;
                                var vendordata = wishlistservice1.getwishlistvendors(list.wishlist_id, category.category_id);
                                if(vendordata!=null)
                                {
                                    List<vendors> vendorslst = new List<vendors>();
                                    for (int j = 0; j < vendordata.Count(); j++)
                                    {
                                        vendors v = new vendors();
                                       v.vendor_id = vendordata[j].VendormasterId;
                                        v.category_id = vendordata[j].Category_TypeId;
                                        v.category_name = vendordata[j].name;
                                        v.name = vendordata[j].BusinessName;
                                        v.city = vendordata[j].City;
                                        v.rating = vendordata[j].Rating;
                                        v.reviews_count = vendordata[j].ReviewsCount;
                                        v.charge_type = vendordata[j].Type_of_price;
                                        price p = new price();
                                        if (vendordata[j].name == "Venues" || vendordata[j].name == "Caterers")
                                        {
                                            
                                            p.minimum_price = vendordata[j].VegPrice;
                                        }
                                        else
                                        {
                                            p.minimum_price = vendordata[j].MinPrice;
                                        }
                                        v.price = p;
                                        v.pic_url = vendordata[j].pic_url;
                                        v.contributor_id = vendordata[j].UserId;
                                        vendorslst.Add(v);
                                    }
                                    category.vendors = vendorslst;
                                }
                                
                                categorys.Add(category);
                            }
                            list.wishlistitems = categorys;
                                dict.Add("status", true);
                        dict.Add("message", "Success");
                        dict.Add("data", list);
                    }
                     else
                     {
                        dict.Add("status", false);
                        dict.Add("message", "failed");

                     }
                  }
                    else 
                    {
                        wishlist list = new wishlist();
                        list.wishlist_id = userdata.WishlistdetailId;
                        list.user_id = userdata.UserId;
                        list.name = userdata.Name;
                        FilterServices filterServices = new FilterServices();
                        var categories = filterServices.AllCategories();
                        List<wishlistitems> categorys = new List<wishlistitems>();
                        for (int i = 0; i < categories.Count(); i++)
                        {
                            wishlistitems category = new wishlistitems();
                            category.category_id = categories[i].servicType_id;
                            category.category_name = categories[i].name;
                            category.page_name = categories[i].display_name;
                            var vendordata = wishlistservice1.getwishlistvendors(list.wishlist_id, category.category_id);
                            if (vendordata != null)
                            {
                                List<vendors> vendorslst = new List<vendors>();
                                for (int j = 0; j < vendordata.Count(); j++)
                                {
                                    vendors v = new vendors();
                                    v.vendor_id = vendordata[j].VendormasterId;
                                    v.category_id = vendordata[j].Category_TypeId;
                                    v.category_name = vendordata[j].name;
                                    v.name = vendordata[j].BusinessName;
                                    v.city = vendordata[j].City;
                                    v.rating = vendordata[j].Rating;
                                    v.reviews_count = vendordata[j].ReviewsCount;
                                    v.charge_type = vendordata[j].Type_of_price;
                                    price p = new price();
                                    if (vendordata[j].name == "Venues" || vendordata[j].name == "Caterers")
                                    {
                                        p.minimum_price = vendordata[j].VegPrice;
                                    }
                                    else
                                    {
                                        p.minimum_price = vendordata[j].MinPrice;
                                    }
                                    v.price = p;
                                    v.pic_url = vendordata[j].pic_url;
                                    v.contributor_id = vendordata[j].UserId;
                                    vendorslst.Add(v);
                                }
                                category.vendors = vendorslst;
                            }

                            categorys.Add(category);
                        }
                        list.wishlistitems = categorys;
                        dict.Add("status", true);
                        dict.Add("message", "Success");
                        dict.Add("data", list);
                    }
                }
            }
            return Json(dict);
        }

        [HttpPost]
        [Route("api/wishlist/additem")]
        public IHttpActionResult AddItemToWishlist([FromBody]listItems list)
        {
            Userwishlistdetails userwishlistdetails = new Userwishlistdetails();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            WhishListService wishlistservices = new WhishListService();
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if(userdetails.Token == token)
                { 
                long item = wishlistservices.Getvendordetailsbyvendorid(list.vendor_id,list.wishlist_id);
                if (item == 0)
                {
                    var details = wishlistservices.Getwishlistdetail(list.wishlist_id);
                    ResultsPageService resultsPageService = new ResultsPageService();
                    category categorys = new category();
                    vendor v = new vendor();
                    userwishlistdetails.UserId = details.UserId;
                    userwishlistdetails.wishlistId = list.wishlist_id;
                    userwishlistdetails.vendorId = list.vendor_id;
                    userwishlistdetails.IPAddress = HttpContext.Current.Request.UserHostAddress;
                    userwishlistdetails.WhishListedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var data = wishlistservices.Adduserwishlistitem(userwishlistdetails);
                    if (data != null)
                    {
                        var vdata = wishlistservices.Getdetailsofvendorbyid(list.vendor_id);
                        categorys.category_id = vdata.Category_TypeId;
                        categorys.category_name = vdata.name;
                        v.vendor_id = vdata.VendorId;
                        v.category_id = vdata.Category_TypeId;
                        v.category_name = vdata.name;
                        v.userwishlist_id = vdata.wishlistId;
                        v.name = vdata.BusinessName;
                        v.city = vdata.City;
                        v.charge_type = vdata.Type_of_price;
                        v.collabrator_id = vdata.UserId;
                        v.rating = vdata.Rating;
                        v.reviews_count = vdata.ReviewsCount;
                            price p = new price();
                        if (vdata.name == "Venues" || vdata.name == "Caterers")
                        {
                                p.minimum_price = vdata.VegPrice;
                        }
                        else
                        {
                                p.minimum_price = vdata.MinPrice;
                        }
                           v.price = p;
                         v.pic_url = vdata.Image;
                        v.notes = null;
                        categorys.vendor = v;
                        dict.Add("status", true);
                        dict.Add("message", "Success");
                        dict.Add("data", categorys);
                    }
                       else
                        {
                            dict.Add("status", false);
                            dict.Add("message", "failed");
                        }
                    }
                    else
                    {
                        dict.Add("status", false);
                        dict.Add("message", "This service already existed in wishlist");
                    }
                }
                else
                {
                    dict.Add("status", false);
                    dict.Add("message", "failed");
                }
            }      
            return Json(dict);
        }

        [HttpDelete]
        [Route("api/wishlist/removeitem")]
        public IHttpActionResult RemoveWishList([FromBody]detailids ids)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            WhishListService wishlistservices = new WhishListService();

            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if (userdetails.Token == token)
                {
                    if (userdetails != null)
                    {
                        int count = wishlistservice.Removeitem(ids.vendor_id, ids.wishlist_id, userdetails.UserLoginId);
                        if (count != 0)
                        {
                            dict.Add("status", true);
                            dict.Add("message", "Success");
                        }
                        else
                        {
                            dict.Add("status", true);
                            dict.Add("message", "This service is already removed");
                        }
                    }
                    else
                    {
                        dict.Add("status", false);
                        dict.Add("message", "Failed");
                    }

                  }
                }
            return Json(dict);
        }
        //var details = wishlistservices.Getwishlistdetail(ids.wishlist_id);
        // if(details!=null)
        // {
        //    long user_id = details.UserId;
        // int count = wishlistservice.Removeitem(ids.vendor_id,ids.wishlist_id,user_id);
        // if(count!=0)
        // {
        //     dict.Add("status", true);
        //     dict.Add("message", "Success");
        // }
        //}
        // else
        // {
        //     dict.Add("status", false);
        //     dict.Add("message", "Failed");
        // }

        [HttpGet]
        [Route("api/getallnotes")]
        public IHttpActionResult Getnote(long wishlist_id,long vendor_id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if (userdetails.Token == token)
                {
                    var data = wishlistservice.Getnote(wishlist_id, vendor_id);
                    if(data!=null)
                    {
                        List<UseNotes> usernotes = new List<UseNotes>();
                        foreach (var item in data)
                        {
                            UseNotes usernote = new UseNotes();
                            usernote.notes_id = item.NotesId;
                            usernote.wishlist_id = item.wishlistId;
                            usernote.vendor_id = item.VendorId;
                            usernote.notes = item.Notes;
                            usernote.added_datetime = item.AddedDate;
                            usernote.edited_datetime = item.UpdatedDate;
                            usernotes.Add(usernote);
                        }

                        Dictionary<string, object> dict1 = new Dictionary<string, object>();
                        dict1.Add("results", usernotes);
                        dict.Add("status", true);
                        dict.Add("message", "Success");
                        dict.Add("data", dict1);
                    }
                }
                else
                {
                    dict.Add("status", false);
                    dict.Add("message", "Failed");
                }

            }
            return Json(dict);

        }

        [HttpPost]
        [Route("api/addnote")]
        public IHttpActionResult AddNotes([FromBody]AddNote note)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if (userdetails.Token == token)
                {
                    Note notes = new Note();
                    notes.wishlistId = note.wishlist_id;
                    notes.VendorId = note.vendor_id;
                    notes.UserId = userdetails.UserLoginId;
                    notes.Notes = note.Notes;
                    notes.Name = userdetails.FirstName.Trim()+" "+userdetails.LastName;
                    notes.AddedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    notes.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    var notedata = wishlistservice.AddNotes(notes);
                    UseNotes usernotes = new UseNotes();
                    usernotes.notes_id = notedata.NotesId;
                    usernotes.wishlist_id = notedata.wishlistId;
                    usernotes.vendor_id = notedata.VendorId;
                    usernotes.notes = notedata.Notes;
                    usernotes.added_datetime = notedata.AddedDate;
                    if (notedata != null)
                    {
                        // Dictionary<string, object> dict1 = new Dictionary<string, object>();
                        //dict1.Add("results", usernotes);
                        dict.Add("status", true);
                        dict.Add("message", "Success");
                        dict.Add("data", usernotes);
                        
                    }
                }
                else
                {
                    dict.Add("status", false);
                    dict.Add("message", "Failed");
                }
            }

            return Json(dict);
        }

        [HttpPost]
        [Route("api/updatenote")]
        public IHttpActionResult UpdateNote([FromBody] EditNote enote)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            Note notes = new Note();
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if (userdetails.Token == token)
                {
                    var notedata = wishlistservice.UpdateNotes(enote.notes_id, enote.notes);
                    if (notedata != null)
                    {
                        UseNotes usernotes = new UseNotes();
                        usernotes.notes_id = notedata.NotesId;
                        usernotes.wishlist_id = notedata.wishlistId;
                        usernotes.vendor_id = notedata.VendorId;
                        usernotes.notes = notedata.Notes;
                        usernotes.added_datetime = notedata.AddedDate;
                        usernotes.edited_datetime = notedata.UpdatedDate;
                        dict.Add("status", true);
                        dict.Add("message", "Success");
                        return Json(dict);
                    }
                }
                else { 
                dict.Add("status", false);
                dict.Add("message", "Failed");
                }
            }
                 
          
            return Json(dict);
        }
        [HttpDelete]
        [Route("api/removenote")]
        public IHttpActionResult RemoveNotes(long notes_id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
           int count= wishlistservice.RemoveNotes(notes_id);
            if (count != 0)
            {
                dict.Add("status", true);
                dict.Add("message", "Success");
            }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "Failed");
            }
            return Json(dict);

        }
        
        [HttpPost]
        [Route("api/addcollabrator")]
        public IHttpActionResult Addcollabrator(UserCollabrator collabrator)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            Collabrator collbratordata = new  Collabrator();
            var re = Request;
            var customheader = re.Headers;
            UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
            if (customheader.Contains("Authorization"))
            {
                string token = customheader.GetValues("Authorization").First();
                var userdetails = userlogindetailsservice.Getmyprofile(token);
                if (userdetails.Token == token)
                {
                    collbratordata.UserId = userdetails.UserLoginId;
                    collbratordata.wishlistid = collabrator.wishlist_id;
                    collbratordata.PhoneNo = collabrator.phoneNo;
                    collbratordata.Email = collabrator.email;
                    collbratordata.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    string name = collbratordata.UserId.ToString() + ',' + collbratordata.wishlistid.ToString() + ',' + collabrator.email;
                    encptdecpt encrypt = new encptdecpt();
                    string encrypted = encrypt.Encrypt(name);
                    collbratordata.wishlistlink = encrypted;
                    long details = wishlistservice.GetcollabratorDetailsByEmail(collabrator.email);
                    if(details == 0)
                    { var data = wishlistservice.AddCollabrator(collbratordata);
                        DetailsCollabrator cdetails = new DetailsCollabrator();
                        if (data!=null)
                        {
                            cdetails.email = data.Email;
                            cdetails.phoneNo = data.PhoneNo;
                            cdetails.user_id = data.UserId;
                            cdetails.collabrator_id = data.Id;
                            cdetails.wishlist_id = data.wishlistid;
                            cdetails.code = data.wishlistlink;
                            //string url = "http://sandbox.ahwanam.com/verify?activation_code=" + userlogin.ActivationCode + "&email=" + userlogin.UserName;
                            //FileInfo File = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath("/mailtemplate/welcome.html"));
                            //string readFile = File.OpenText().ReadToEnd();
                            //readFile = readFile.Replace("[ActivationLink]", url);
                            //readFile = readFile.Replace("[name]", data.Email);
                            //readFile = readFile.Replace("[phoneno]", data.PhoneNo);
                            //TriggerEmail(data.Email, readFile, "Account Activation", null);
                            //dict.Add("status", true);
                            //dict.Add("message", "Success");
                            //dict.Add("result", cdetails);
                        }
                        else
                        {
                            dict.Add("status", false);
                            dict.Add("message", "failed");
                          
                        }
                    }
                    //string decrypted = encrypt.Decrypt(encrypted);
                    else
                    {
                        dict.Add("status", false);
                        dict.Add("message", "This collabrator already added");
                    }
                }
            }
            return Json(dict);
        }

        public void TriggerEmail(string txtto, string txtmsg, string subject, HttpPostedFileBase attachment)
        {
            EmailSendingUtility emailSendingUtility = new EmailSendingUtility();
            emailSendingUtility.Wordpress_Email(txtto, txtmsg, subject, attachment);
        }

        //[HttpGet]
        //public IHttpActionResult checkemail(string email)
        //{
        //    long data = userlogindetailsservice.GetLoginDetailsByEmail(email);
        //    if(data==0)
        //    {

        //    }
        //}

        [HttpDelete]
        [Route("api/removecollabrator")]
        public IHttpActionResult removecollabrator(long collabrator_id)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            int count = wishlistservice.RemoveCollabrator(collabrator_id);
            if (count != 0)
            {
                dict.Add("status", true);
                dict.Add("message", "Success");
            }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "Failed");
            }
            return Json(dict);

        }
    }
}