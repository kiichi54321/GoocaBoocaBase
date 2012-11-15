using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoocaBoocaBase.Controllers
{
    public class HomeController : Controller
    {
        [HandleError]
        public ActionResult Index()
        {
      //      ViewBag.Message = "GoocaBoocaへようこそ";
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            ViewBag.DB = db;
            return View(db.Researches.ToArray());
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ViewImages()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.Items.ToArray());
        }

        public ActionResult ViewAnswerData()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.ItemAnsweres.ToArray());
        }

        public ActionResult ViewQuestionAnswer()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.QuestionAnsweres.ToArray());

        }

        public ActionResult ViewCompletedUserByHour()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.QuestionAnsweres.ToArray());
        }

        public ActionResult ViewFreeAnswer()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.FreeAnsweres.ToArray());
        }
    }
}
