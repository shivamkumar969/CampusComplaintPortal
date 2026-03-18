using CampusComplaintPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CampusComplaintPortal.Controllers
{
    public class HomeController : Controller
    {
        // DataBase Connect
        //cfcrdbEntites db = new cfcrdbEntites();
        //cfcrdbEntities1 db = new cfcrdbEntities1();
        cfcrdbEntities db = new cfcrdbEntities();

        // GET: Home
        public ActionResult Index()
        {
            var data = db.notificationmasters
                         .OrderByDescending(x => x.AddedOn)
                         .ToList();

            return View(data);
        }

        // About
        public ActionResult About()
        {
            return View();
        }

        // Team Page
        public ActionResult Team()
        {
            return View();
        }

        // Contact Us
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]

        public ActionResult Contact(enquirymaster enq)
        {
            string msg = "";
            try
            {
                enq.EnquiryDate =  DateTime.Now;
                db.enquirymasters.Add(enq);
                db.SaveChanges();
                msg = "Thanks for giving the valuable Feed";
            }
            catch
            {
                msg = "Technical Error Occurd.";
            }
            ViewBag.Message = msg;
            return View();
        }



        // Report Complaint
        public ActionResult ReportComplaint()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReportComplaint(complaintmaster cmp, HttpPostedFileBase ImageFile)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login");
            }

            int lastId = db.complaintmasters
                           .OrderByDescending(x => x.Id)
                           .Select(x => x.Id)
                           .FirstOrDefault();
            cmp.ComplaintNo = "CFCR-" + (100 + lastId + 1);
            cmp.StudentId = Convert.ToInt32(Session["UserId"]);
            cmp.Status = "Pending";
            cmp.ReportDate = DateTime.Now;

            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string entension = System.IO.Path.GetFileName(ImageFile.FileName);
                string fileName = cmp.ComplaintNo+ "_" + entension;
                string folder = Server.MapPath("~/ComplaintImages");

                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                string path = System.IO.Path.Combine(folder, fileName);
                ImageFile.SaveAs(path);

                cmp.ImageFile = fileName;
            }

            db.complaintmasters.Add(cmp);
            db.SaveChanges();
            registermaster rm = db.registermasters.Find(cmp.StudentId);
            Mailer mail = new Mailer();
            string mailmsg = "Dear "+rm.Name+",\r\n\r\nThank you for contacting us.\r\n\r\nThis email is to confirm that we have successfully received your complaint regarding [brief description of the issue]. Your complaint has been registered with the following details:\r\n\r\nComplaint ID: "+ cmp.ComplaintNo + "\r\nDate of Registration: "+ cmp.ReportDate + "\r\n\r\nOur team is currently reviewing your complaint and will work to resolve the issue as soon as possible. Please keep this Complaint ID for your reference in any future communication.\r\n\r\nIf you need further assistance, feel free to contact us.\r\n\r\nThank you for your patience and cooperation.\r\n\r\nBest regards,\r\nShivam Kumar\r\nSchool of Management Sciences Varansi\r\ncompreg@gmail.com";
            string sub = "Your Complaint Registed";
            
            mail.SendMyEmail(rm.Email, sub, mailmsg);

            TempData["Message"] = "Complaint Submitted Successfully. Complaint ID: " + cmp.ComplaintNo;
            
            
            return RedirectToAction("ReportComplaint");
        }

        // Join Now
        [HttpGet]
        public ActionResult JoinNow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult JoinNow(registermaster reg)
        {
            string msg = "";
            try
            {
                // Check if email already exists
                var checkEmail = db.registermasters.Where(x => x.Email == reg.Email).FirstOrDefault();
                if (checkEmail != null)
                {
                    msg = "This email is already registered. Please use another email.";
                }
                else
                {
                    db.registermasters.Add(reg);
                    db.SaveChanges();
                    msg = "Registration Successful";
                }
            }
            catch
            {
                msg = "Sorry! Technical error occurred.";
            }
            ViewBag.Message = msg;
            return View();
        }

        // Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            var user = db.registermasters.Where(x => x.Email == Email && x.Password == Password).FirstOrDefault();
            if (user != null) 
            {
                Session["UserId"] = user.Id;
                Session["UserName"]= user.Name;
                return RedirectToAction("ReportComplaint");
            }
            else
            {
                ViewBag.Message = "Invalid Id and Password.";
                return View();
            }
            
        }

        // Track Complaint
        [HttpGet]
        public ActionResult TrackComplaint()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TrackComplaint(string ComplaintNo)
        {
            var complaint = db.complaintmasters
                              .FirstOrDefault(x => x.ComplaintNo == ComplaintNo);
            if (complaint != null)
            {
                return View(complaint);
            }
            ViewBag.Message = "Complaint not found!";
            return View();
        }

        // LogOut
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        // Developer
        public ActionResult Developer()
        {
            string msg = "Dear Sir/Madam,\n\r\nI hope you are doing well.\r\n\r\nI would like to inquire about your [product/service]. I am interested in getting more information regarding the details, pricing, availability, and any other relevant specifications.\r\n\r\nKindly provide the necessary information at your earliest convenience. Your response will help me make a better decision.";
            return View();
        }
    }
}