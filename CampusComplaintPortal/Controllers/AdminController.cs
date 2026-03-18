using CampusComplaintPortal.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CampusComplaintPortal.Controllers
{
    public class AdminController : Controller
    {
        cfcrdbEntities db = new cfcrdbEntities();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Username, string Password)
        {
            var admin = db.adminmasters
                          .FirstOrDefault(x => x.Username == Username && x.Password == Password);

            if (admin != null)
            {
                Session["AdminId"] = admin.Id;
                Session["AdminName"] = admin.Username;

                return RedirectToAction("Dashboard");
            }

            ViewBag.Message = "Invalid Username or Password";
            return View();
        }
        // Dashboard
        public ActionResult Dashboard()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.total = db.complaintmasters.Count();

            ViewBag.pending = db.complaintmasters
                                .Count(x => x.Status == "Pending");
            ViewBag.progress = db.complaintmasters
                                 .Count(x => x.Status == "In Progress");
            ViewBag.fixeds = db.complaintmasters
                               .Count(x => x.Status == "Fixed");
            var complaints = db.complaintmasters
                               .OrderByDescending(x => x.Id)
                               .ToList();
            return View(complaints);
        }


        // Manage Complaints
        public ActionResult ManageComplaints()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }

            var complaints = db.complaintmasters
                               .OrderByDescending(x => x.Id)
                               .ToList();

            return View(complaints);
        }

        [HttpPost]
        public ActionResult UpdateStatus(int id, string status)
        {
            var complaint = db.complaintmasters.Find(id);

            if (complaint != null)
            {
                complaint.Status = status;
                db.SaveChanges();
            }

            return RedirectToAction("ManageComplaints");
        }

        // Manage Enquiries
        public ActionResult ManageEnquiries()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }
            var  enquery= db.enquirymasters
                               .OrderByDescending(x => x.EnqId)
                               .ToList();

            return View(enquery);
        }

        // Notifications
        [HttpGet]
        public ActionResult Notifications()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Message = TempData["Message"];

            var data = db.notificationmasters
                         .OrderByDescending(x => x.AddedOn)
                         .ToList();

            return View(data);
        }

        [HttpPost]
        public ActionResult Notifications(notificationmaster nm)
        {
            string msg = "";
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Login");
            }
            try
            {
                nm.AddedOn = DateTime.Now;
                db.notificationmasters.Add(nm);
                db.SaveChanges();
                msg = "Notification Added";
            }
            catch
            {
                msg = "Sorry!  technical issue";
            }
            ViewBag.Message = msg;
            var data = db.notificationmasters
                .OrderByDescending(x => x.AddedOn)
                .ToList();

            return RedirectToAction("Notifications");
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // Delete
        public ActionResult DeleteNotification(int NotiId)
        {
            try
            {
                var data = db.notificationmasters.Find(NotiId);

                if (data != null)
                {
                    db.notificationmasters.Remove(data);
                    db.SaveChanges();
                }
            }
            catch
            {
                TempData["Message"] = "Sorry! Technical Issue";
            }

            return RedirectToAction("Notifications");
        }


    }
}