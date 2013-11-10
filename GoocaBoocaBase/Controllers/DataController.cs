using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoocaBoocaBase.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/

        public ActionResult Index()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase gb = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            
            return View(gb.Researches.Where(n => n.Hidden == false && n.IsActive == true).ToArray());
        }


        public ActionResult GetQuestion(string researchIdName)
        {
            var content = new ContentResult() { ContentEncoding = System.Text.Encoding.UTF8 };
            content.Content = GoocaBoocaDataModels.Utility.CrossTableConvert.CreateQuestionData(researchIdName);

            return content;
        }

        public ActionResult GetAnswer(string researchIdName,int skip)
        {
            var content = new ContentResult() { ContentEncoding = System.Text.Encoding.UTF8 };
            content.Content = GoocaBoocaDataModels.Utility.CrossTableConvert.CreateAnswerData(researchIdName,skip);

            return content;
        }

        public ActionResult GetResearch()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase gb = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var list = gb.Researches.Where(n => n.IsActive == true && n.Hidden == false).Select(n => n.ResearchIdName).ToArray();
            return Json(list,JsonRequestBehavior.AllowGet);
        }
    }
}
