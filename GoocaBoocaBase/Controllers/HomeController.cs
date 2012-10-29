using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoocaBoocaBase.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "ASP.NET MVC へようこそ";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();

        public ActionResult ViewImages()
        {
            return View(db.Items.ToArray());
        }

        public ActionResult ViewAnswerData()
        {
            return View(db.ItemAnsweres.ToArray());
        }

    }
}
