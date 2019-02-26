﻿using AhwanamAPI.Custom;
using MaaAahwanam.Models;
using MaaAahwanam.Service;
using MaaAahwanam.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using System.Web.Http.Cors;
using System.Collections;
using AhwanamAPI.Models;
using System.Net.Http.Headers;

namespace AhwanamAPI.Controllers
{
    public class UserAuthController : ApiController
    {
        ResultsPageService resultsPageService = new ResultsPageService();
        UserLoginDetailsService userlogindetailsservice = new UserLoginDetailsService();
        VenorVenueSignUpService venorvenuesignupservice = new VenorVenueSignUpService();
        cartservices cartserve = new cartservices();
        VendorMasterService vendorMasterService = new VendorMasterService();
        VendorsCatering vendorsCatering = new VendorsCatering();
        VendorsDecorator vendorsDecorator = new VendorsDecorator();
        VendorDecoratorService vendorDecoratorService = new VendorDecoratorService();
        VendorsPhotography vendorsPhotography = new VendorsPhotography();
        VendorVenue vendorVenue = new VendorVenue();
        VendorsOther vendorsOther = new VendorsOther();
        VendorCateringService vendorCateringService = new VendorCateringService();
        VendorPhotographyService vendorPhotographyService = new VendorPhotographyService();
        VendorVenueService vendorVenueService = new VendorVenueService();
        VendorOthersService vendorOthersService = new VendorOthersService();
        VenorVenueSignUpService vendorVenueSignUpService = new VenorVenueSignUpService();
        Vendormaster vendormaster = new Vendormaster();

        public class outputresponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public userdata userdata { get; set; }
        }

        public class userdata
        {
            public string email { get; set; }
            public string password { get; set; }
            public string phoneno { get; set; }
            public string name { get; set; }
        }

        public class loginresponse
        {
            public string status { get; set; }
            public string message { get; set; }
            public loginuser loginuser { get; set; }
        }

        public class loginuser
        {
            public string email { get; set; }
            public string password { get; set; }
            public string phoneno { get; set; }
            public string name { get; set; }
        }

        public class slogin
        {
            public string access_token { get; set; }
            public string auth_type { get; set; }
        }

        public class sloginresponse
        {
            public string id { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string link { get; set; }
            public string gender { get; set; }
            public string locale { get; set; }
            public string picture { get; set; }
            public string timezone { get; set; }
            public string verified { get; set; }
            public string age_range { get; set; }
            public string name { get; set; }
            public string phoneno { get; set; }
        }

        [HttpPost]
        [Route("api/UserAuth/sociallogin")]
        public IHttpActionResult sociallogin([FromBody]slogin login)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Dictionary<string, object> dict1 = new Dictionary<string, object>();
            if (login.auth_type == "facebook")
            {
                client.BaseAddress = new Uri("https://graph.facebook.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.GetAsync("v3.2/me?fields=id,first_name,last_name,email,link,gender,locale&locale=en_US&access_token=" + login.access_token).Result;
            }
            else if (login.auth_type == "google")
            {
                client.BaseAddress = new Uri("https://www.googleapis.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.GetAsync("oauth2/v1/userinfo?access_token=" + login.access_token).Result;
            }
            if (response.IsSuccessStatusCode)
            {
                sloginresponse data = response.Content.ReadAsAsync<sloginresponse>().Result;
                dict = checkemail(data, login);
            }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "Invalid Token");
            }
            return Json(dict);
        }

        [HttpPost]
        [Route("api/UserAuth/login")]
        public IHttpActionResult login([FromBody]registerdetails details)
        {
            UserLogin userlogin = new UserLogin();
            userlogin.UserName = details.email;
            userlogin.Password = details.password;
            Dictionary<string, object> dict = new Dictionary<string, object>();
            UserLogin data = new UserLogin();
            userdata user = new userdata();
            Dictionary<string, object> u1 = new Dictionary<string, object>();
            if (userlogin.UserName == null || userlogin.Password == null)
            {
                dict.Add("status", false);
                dict.Add("message", "Field missing");
                u1.Add("user", null);
                dict.Add("data", u1);
                return Json(dict);
            }
            var userResponce = resultsPageService.GetUserLogin(userlogin);
            if (userResponce != null)
            {
                if (userResponce.Status == "InActive")
                {
                    dict.Add("status", false);
                    dict.Add("message", "email id not verified");
                    u1.Add("user", null);
                    dict.Add("data", u1);
                    return Json(dict);
                }
                UserLoginDetailsService userLoginDetailsService = new UserLoginDetailsService();
                var userdetails = userLoginDetailsService.GetUser((int)userResponce.UserLoginId);
                    encptdecpt encrypt = new encptdecpt();
                string encrypted = encrypt.Encrypt(userResponce.UserName);
                dict.Add("status", true);
                dict.Add("message", "Login Success");
                loginuser loginuser = new loginuser();
                loginuser.email = userlogin.UserName;
                loginuser.name = userdetails.FirstName;
                loginuser.phoneno = userdetails.UserPhone;
                u1.Add("token", encrypted);
                u1.Add("user", loginuser);
                dict.Add("data", u1);
              
            }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "email and password doesnot match");
                u1.Add("user", null);
                dict.Add("data", u1);
            }

            return Json(dict);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/UserAuth/register")]
        public IHttpActionResult register([FromBody]registerdetails details)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (details.name == null || details.email == null || details.phoneno == null || details.password == null)
            {
                dict.Add("status", false);
                dict.Add("message", "Field missing");
                return Json(dict);
            }
            UserLogin userlogin = new UserLogin();
            UserDetail userdetail = new UserDetail();
            userlogin.ActivationCode = Guid.NewGuid().ToString();
            userdetail.FirstName = details.name;
            userdetail.UserPhone = details.phoneno;
            userdetail.AlternativeEmailID = details.email;
            userlogin.Password = details.password;
            userlogin.UserName = details.email;
            userlogin.Status = "InActive";
            var responce = "";
            userlogin.UserType = "User";
            long data = userlogindetailsservice.GetLoginDetailsByEmail(details.email);
            if (data == 0)
            { responce = userlogindetailsservice.AddUserDetails(userlogin, userdetail); }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "Email already used");
            }
            if (responce == "sucess")
            {
                string url = "https://ahwanam-sandbox.herokuapp.com/verify?activation_code=" + userlogin.ActivationCode + "&email=" + userlogin.UserName;
                FileInfo File = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath("/mailtemplate/welcome.html"));
                string readFile = File.OpenText().ReadToEnd();
                readFile = readFile.Replace("[ActivationLink]", url);
                readFile = readFile.Replace("[name]", Capitalise(userdetail.FirstName));
                readFile = readFile.Replace("[phoneno]", userdetail.UserPhone);
                TriggerEmail(userlogin.UserName, readFile, "Account Activation", null); // A Mail will be triggered
                dict.Add("status", true);
                dict.Add("message", "Successfully registered");
            }
            return Json(dict);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/UserAuth/verify")]
        public IHttpActionResult activateemail(string activation_code, string email)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            if (activation_code == "") { activation_code = null; }
            var userresponce = venorvenuesignupservice.GetUserdetails(email);
            if (activation_code == userresponce.ActivationCode)
            {

                dict.Add("status", true);
                dict.Add("message", "Email successfully verified");
                return Json(dict);
            }
            dict.Add("status", false);
            dict.Add("message", "Link Expired");
            return Json(dict);

        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/UserAuth/ActivateEmail1")]
        public IHttpActionResult ActivateEmail1(string ActivationCode, string Email)
        {
            try
            {
                UserLogin userLogin = new UserLogin();
                UserDetail userDetails = new UserDetail();
                if (ActivationCode == "")
                { ActivationCode = null; }
                var userResponse = venorvenuesignupservice.GetUserdetails(Email);
                if (userResponse.Status != "Active")
                {
                    if (ActivationCode == userResponse.ActivationCode)
                    {
                        userLogin.Status = "Active";
                        userDetails.Status = "Active";
                        string email = userLogin.UserName;
                        var userid = userResponse.UserLoginId;
                        userlogindetailsservice.changestatus(userLogin, userDetails, (int)userid);
                        if (userResponse.UserType == "Vendor")
                        {
                            vendormaster = vendorMasterService.GetVendorByEmail(email);
                            string vid = vendormaster.Id.ToString();
                            if (vendormaster.ServicType == "Catering")
                            {
                                var catering = venorvenuesignupservice.GetVendorCatering(long.Parse(vid)).FirstOrDefault();
                                vendorsCatering.Status = vendormaster.Status = "Active";
                                vendorsCatering = vendorCateringService.activeCatering(vendorsCatering, vendormaster, long.Parse(catering.Id.ToString()), long.Parse(vid));
                            }
                            else if (vendormaster.ServicType == "Decorator")
                            {
                                var decorators = venorvenuesignupservice.GetVendorDecorator(long.Parse(vid)).FirstOrDefault();
                                vendorsDecorator.Status = vendormaster.Status = "Active";
                                vendorsDecorator = vendorDecoratorService.activeDecorator(vendorsDecorator, vendormaster, long.Parse(decorators.Id.ToString()), long.Parse(vid));
                            }
                            else if (vendormaster.ServicType == "Photography")
                            {
                                var photography = venorvenuesignupservice.GetVendorPhotography(long.Parse(vid)).FirstOrDefault();
                                vendorsPhotography.Status = vendormaster.Status = "Active";
                                vendorsPhotography = vendorPhotographyService.ActivePhotography(vendorsPhotography, vendormaster, long.Parse(photography.Id.ToString()), long.Parse(vid));
                            }
                            else if (vendormaster.ServicType == "Venue")
                            {
                                var venues = venorvenuesignupservice.GetVendorVenue(long.Parse(vid)).FirstOrDefault();
                                vendorVenue.Status = vendormaster.Status = "Active";
                                vendorVenue = vendorVenueService.activeVenue(vendorVenue, vendormaster, long.Parse(venues.Id.ToString()), long.Parse(vid));
                            }
                            else if (vendormaster.ServicType == "Other")
                            {
                                var others = venorvenuesignupservice.GetVendorOther(long.Parse(vid)).FirstOrDefault();
                                vendorsOther.Status = vendormaster.Status = "Active";
                                vendorsOther = vendorOthersService.activationOther(vendorsOther, vendormaster, long.Parse(others.Id.ToString()), long.Parse(vid));
                            }
                        }

                        return Json("Thanks for Verifying the Email");
                    }
                }
                else
                {
                    return Json("Your Account is already Verified Please login");
                }
                return Json("Email not found");
            }
            catch (Exception)
            {
                return Json("Error");
            }
        }

        public void TriggerEmail(string txtto, string txtmsg, string subject, HttpPostedFileBase attachment)
        {
            EmailSendingUtility emailSendingUtility = new EmailSendingUtility();
            emailSendingUtility.Wordpress_Email(txtto, txtmsg, subject, attachment);
        }

        #region References
        public IHttpActionResult checkAuthentication()
        {
            string name = "";
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var user = (CustomPrincipal)System.Web.HttpContext.Current.User;
                if (user.UserType == "User")
                {
                    var userdata = userlogindetailsservice.GetUser((int)user.UserId);
                    if (userdata.FirstName != "" && userdata.FirstName != null)
                        name = userdata.FirstName;
                    else if (userdata.FirstName != "" && userdata.FirstName != null && userdata.LastName != "" && userdata.LastName != null)
                        name = "" + userdata.FirstName + " " + userdata.LastName + "";
                    else
                        name = userlogindetailsservice.GetUserId((int)user.UserId).UserName; //userdata.AlternativeEmailID;

                    var cart = cartserve.CartItemsList((int)user.UserId).Where(m => m.Status == "Active");

                    return Json(cart);
                }
            }
            return Json("error");
        }

        public string Capitalise(string str)
        {
            if (String.IsNullOrEmpty(str))
                return String.Empty;
            return Char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        public Dictionary<string, object> checkemail(sloginresponse sloginresponse, slogin slogin)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            long data = userlogindetailsservice.GetLoginDetailsByEmail(sloginresponse.email);
            if (data == 0)
            {
                UserLogin userlogin = new UserLogin();
                UserDetail userdetail = new UserDetail();
                userlogin.ActivationCode = Guid.NewGuid().ToString();
                userdetail.FirstName = sloginresponse.first_name;
                userdetail.LastName = sloginresponse.last_name;
                userdetail.UserPhone = sloginresponse.phoneno;
                userdetail.AlternativeEmailID = sloginresponse.email;
                userlogin.Password =
                userlogin.UserName = sloginresponse.email;
                userlogin.Status = "InActive";
                userlogin.UserType = "User";
                var responce = userlogindetailsservice.AddUserDetails(userlogin, userdetail);
                if (responce == "sucess")
                {
                    string url = "https://ahwanam-sandbox.herokuapp.com/verify?activation_code=" + userlogin.ActivationCode + "&email=" + userlogin.UserName;
                    FileInfo File = new FileInfo(System.Web.Hosting.HostingEnvironment.MapPath("/mailtemplate/welcome.html"));
                    string readFile = File.OpenText().ReadToEnd();
                    readFile = readFile.Replace("[ActivationLink]", url);
                    readFile = readFile.Replace("[name]", Capitalise(userdetail.FirstName));
                    readFile = readFile.Replace("[phoneno]", userdetail.UserPhone);
                    TriggerEmail(userlogin.UserName, readFile, "Account Activation", null);
                    dict.Add("status", true);
                    dict.Add("message", "Successfully registered");
                }
            }
            encptdecpt encrypt = new encptdecpt();
            string encrypted = encrypt.Encrypt(sloginresponse.email);
            dict.Add("status", true);
            dict.Add("message", "Login Success");
            loginuser loginuser = new loginuser();
            loginuser.email = sloginresponse.email;
            //loginuser.password = userlogin.Password;
            if (slogin.auth_type == "facebook")
                loginuser.name = sloginresponse.first_name + " " + sloginresponse.last_name;
            else
                loginuser.name = sloginresponse.name;
            loginuser.phoneno = sloginresponse.phoneno;
            Dictionary<string, object> u1 = new Dictionary<string, object>();
            u1.Add("token", encrypted);
            u1.Add("user", loginuser);
            dict.Add("data", u1);
            return dict;
        }
        #endregion


        #region Password
        [AllowAnonymous]
        [HttpGet]
        [Route("api/UserAuth/updatepassword")]
        public IHttpActionResult updatepassword(string Email)
        {
            try
            {

                return Json(Email);
            }
            catch (Exception)
            {
                return Json("error");
            }
        }

        [HttpPost]
        [Route("api/UserAuth/reset")]
        public IHttpActionResult changepassword([FromBody]registerdetails details)
        {
            VenorVenueSignUpService venorVenueSignUpService = new VenorVenueSignUpService();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            UserLogin userLogin = new UserLogin();
            UserLogin u1 = new UserLogin();
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            userLogin.Password = details.password;
            if (details.email != null)
                u1 = venorVenueSignUpService.GetUserdetails(userLogin.UserName);
            else
                u1 = venorVenueSignUpService.GetUserLoginByCode(details.code);
            if (details.code == u1.ActivationCode)
            {
                try
                {
                    userLogin.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    userLogin = userlogindetailsservice.changepassword(userLogin, (int)u1.UserLoginId);
                    dict.Add("status", true);
                    dict.Add("message", "Password Changed");
                    return Json(dict);
                }
                catch (Exception)
                {
                    dict.Add("status", false);
                    dict.Add("message", "Link Expired");
                    return Json(dict);
                }
            }
            else
            {
                dict.Add("status", false);
                dict.Add("message", "Link Expired");
                return Json(dict);
            }
        }


        //[AllowAnonymous]
        //[HttpPost]
        //[Route("api/UserAuth/changepassword")]
        //public IHttpActionResult changepassword([FromBody]registerdetails details)
        //{
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    UserLogin userLogin = new UserLogin();
        //    userLogin.UserName = details.email;
        //    userLogin.Password = details.password;
        //    try
        //    {
        //        var userResponse = venorvenuesignupservice.GetUserdetails(userLogin.UserName);
        //        userlogindetailsservice.changepassword(userLogin, (int)userResponse.UserLoginId);
        //        var userdetails = userlogindetailsservice.GetUser(int.Parse(userResponse.UserLoginId.ToString()));
        //        string url = Request.RequestUri.GetLeftPart(UriPartial.Authority) + "/home";
        //        FileInfo File = new FileInfo(HttpContext.Current.Server.MapPath("/mailtemplate/change-email.html"));
        //        string readFile = File.OpenText().ReadToEnd();
        //        readFile = readFile.Replace("[ActivationLink]", url);
        //        readFile = readFile.Replace("[name]", Capitalise(userdetails.FirstName));
        //        TriggerEmail(userLogin.UserName, readFile, "Your Password is changed", null); // A mail will be triggered
        //        dict.Add("status", true);
        //        dict.Add("message", "Password Changed");
        //        return Json(dict);
        //    }
        //    catch (Exception)
        //    {
        //        dict.Add("status", false);
        //        dict.Add("message", "Failure");
        //        return Json(dict);
        //    }
        //}

        [AllowAnonymous]
        [HttpPost]
        [Route("api/UserAuth/forgotpassword")]
        public IHttpActionResult forgotpass([FromBody]registerdetails details)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            UserLogin userLogin = new UserLogin();
            userLogin.UserName = details.email;
            userLogin = venorvenuesignupservice.GetUserLogdetails(userLogin);
            if (userLogin != null)
            {
                var userdetails = userlogindetailsservice.GetUser(int.Parse(userLogin.UserLoginId.ToString()));
                userLogin.ActivationCode = Guid.NewGuid().ToString();
                userLogin.UpdatedDate = DateTime.Now;
                var data = userlogindetailsservice.UpdateActivationCode(userLogin);
                string url = "https://ahwanam-sandbox.herokuapp.com/resetpassword?code=" + userLogin.ActivationCode;
                FileInfo File = new FileInfo(HttpContext.Current.Server.MapPath("/mailtemplate/mailer.html"));
                string readFile = File.OpenText().ReadToEnd();
                readFile = readFile.Replace("[ActivationLink]", url);
                readFile = readFile.Replace("[name]", Capitalise(userdetails.FirstName));
                TriggerEmail(details.email, readFile, "Password reset information", null);// A mail will be triggered
                dict.Add("status", true);
                dict.Add("message", "Reset Password Request Sent");
                return Json(dict);
            }
            dict.Add("status", false);
            dict.Add("message", "Failure");
            return Json(dict);
        }
        #endregion

        //#region Signout
        //public IHttpActionResult SignOut()
        //{
        //    // Response.Cookies.Clear();
        //    FormsAuthentication.SignOut();
        //    return Json("logout");
        //}
        //#endregion
    }
}

