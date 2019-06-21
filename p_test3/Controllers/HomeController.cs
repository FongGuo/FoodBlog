using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using p_test3.Models;
using System.IO;


namespace p_test3.Controllers
{
    public class HomeController : Controller
    {

        FarmerEntities4 db = new FarmerEntities4();

        // GET: Home
        public ActionResult Index()
        {
            var query = from o in db.Activities
                        select o;
            var x = query.Take(4);
            return View(x.ToList());
        }

        public ActionResult newMember()
        {
            return View();
        }

        [HttpPost]
        public ActionResult newMember(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();
            return Redirect("~/Home/Index");
        }


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var query = from o in db.Users
                        where email == o.Email && password == o.Password
                        select o;

            User user = query.FirstOrDefault();

            HttpCookie c = Request.Cookies["User"];
            HttpCookie UserId = Request.Cookies["UserId"];

            if (query.Count() > 0)
            {
                Response.Cookies.Remove("User");
                Response.Cookies.Remove("UserId");
                Response.Cookies["UserId"].Value = user.UserId.ToString();
                Response.Cookies["User"].Value = user.UserName.ToString();
                return Redirect("~/Home/IndexWithLogout");

            }
            else { return Redirect("~/Home/Login"); }

        }


        public ActionResult Activity()
        {
            HttpCookie c = Request.Cookies["User"];
            if ( c == null || c.Value.ToString()=="")
            {
                return Redirect("~/Home/Login");
            }
            else {
                
                return View(c);
            }

            
           
        }

        [HttpPost]
        public ActionResult Activity(HttpPostedFileBase photo, Activity activity)
        {
            //圖片儲存到server
            string fileName = "";
            if (photo != null)
            {
                if (photo.ContentLength > 0)
                {
                    fileName = Path.GetFileName(photo.FileName);
                    var path = Path.Combine(Server.MapPath("~/images"), activity.Title + ".jpg");
                    photo.SaveAs(path);
                    //System.IO.Directory.CreateDirectory()
                }
            }
            //寫入資料庫的圖片名稱
            string serverPath = activity.Title + ".jpg";
            activity.ServerPath = serverPath;
            activity.UserName = Request.Cookies["User"].Value;
            activity.UserId = Convert.ToInt32(Request.Cookies["UserId"].Value);
            //寫入資料庫
            try
            {
                db.Activities.Add(activity);
                db.SaveChanges();
                return Redirect("~/Home/IndexWithLogout");
            }
            catch (Exception)
            {
                return Redirect("~/Home/Activity");
            }           
        }


        public ActionResult More()
        {
            HttpCookie c = Request.Cookies["User"];
            if (c == null || c.Value.ToString() == "") { 
                return Redirect("~/Home/Login");
            }
            else {
                var query = from o in db.Activities
                            select o;
                return View(query.ToList());
            }
        }

        public Activity Act;


        public ActionResult Plus()
        {

            HttpCookie c = Request.Cookies["User"];
            if (c == null || c.Value.ToString() == "")
            {
                return Redirect("~/Home/Login");
            }
            else
            {
                string id = Request.QueryString["id"];
                int actId = Convert.ToInt32(id);

                var query = from o in db.Activities
                            where o.ActivityId == actId
                            select o;

                Act = query.FirstOrDefault();

                if (string.IsNullOrEmpty(id))
                {
                    id = "1";
                    return Redirect("/Home/More");
                }
                else { return View(Act); }
            }
        }

        [HttpPost]
        public ActionResult Plus(Activity activity)
        {

            var query = from o in db.Activities
                        where o.ActivityId == activity.ActivityId
                        select o;
            Activity act = query.FirstOrDefault();


            if (activity.Participation < 0)
            {

                return Redirect("~/Home/More");
            }
            else
            {
                act.Participation = activity.Participation + act.Participation;
                db.SaveChanges();
                return Redirect("~/Home/More");

            }
        }

        public ActionResult Vip()
        {

            HttpCookie c = Request.Cookies["User"];
            if (c == null || c.Value.ToString() == "")
            {
                return Redirect("~/Home/Login");
            }
            else
            {
                string cookie = Request.Cookies["User"].Value.ToString();
                var query = from o in db.Activities
                            where o.UserName == cookie
                            select o;
                var Activity = query.ToList();
                if (Activity != null)
                {
                    return View(Activity);
                }
                else { return Redirect("~/Home/VipEmpty"); }
            }
        }


        //沒發起活動的會員頁
        public ActionResult VipEmpty()
        {
         
             return View(); 
           
        }


        public ActionResult LogOut()
        {
            if (Response.Cookies["UserId"].Value == null && Response.Cookies["User"].Value == null || Request.Cookies["User"].Value == "" && Response.Cookies["UserId"].Value == "")
            {
                return Redirect("~/Home/Index");
            }
            else { return Redirect("~/Home/IndexWithLogout"); }

        }

        public ActionResult IndexWithLogout()
        {
            HttpCookie c = Request.Cookies["User"];
            HttpCookie UserId = Request.Cookies["UserId"];

            if (c==null && UserId==null)
            {
                return Redirect("~/Home/Index");
            }
            else 
            {
                var query = from o in db.Activities     
                select o;
                var x = query.Take(4);
                return View(x.ToList());
            } 
        }


        public Activity EditActivity;
        public ActionResult Edit()
        {
            string id = Request.QueryString["id"];
            int actId = Convert.ToInt32(id);
            var query = from o in db.Activities
                        where o.ActivityId == actId
                        select o;

            Activity EditActivity = query.FirstOrDefault();

            return View(EditActivity);
        }

        [HttpPost]
        public ActionResult Edit(Activity activity)
        {

            var query = from o in db.Activities
                        where o.ActivityId == activity.ActivityId
                        select o;

            Activity EditActivity = query.FirstOrDefault();

            EditActivity.DateTime = activity.DateTime;
            EditActivity.Location = activity.Location;
            EditActivity.Paragraphy = activity.Paragraphy;
            EditActivity.PersonLimitation = activity.PersonLimitation;
            EditActivity.Title = activity.Title;
            EditActivity.UserName = activity.UserName;
            EditActivity.Participation = 0;
            EditActivity.UserName = activity.UserName;
            EditActivity.UserId = activity.UserId;

            db.SaveChanges();

            return Redirect("~/Home/Vip");
        }

        public ActionResult Delete()
        {
            string id = Request.QueryString["id"];
            int actId = Convert.ToInt32(id);
            var query = from o in db.Activities
                        where o.ActivityId == actId
                        select o;

            Activity EditActivity = query.FirstOrDefault();

            db.Activities.Remove(EditActivity);
            db.SaveChanges();
            return Redirect("~/Home/Vip");
        }

    }
}