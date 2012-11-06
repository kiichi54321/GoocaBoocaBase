using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoocaBoocaBase.Controllers
{
    public class ResultController : Controller
    {
        //
        // GET: /Result/



        public ActionResult Index(string research_id, string uid)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(research_id);
            if (research.ResearchType == GoocaBoocaDataModels.ResearchType.GoocaBooca.ToString())
            {
                return RedirectToAction("GoocaBooca", new { research_id = research_id, uid = uid });
            }
            if (research.ResearchType == GoocaBoocaDataModels.ResearchType.Compare.ToString())
            {
                return RedirectToAction("Compare", new { research_id = research_id, uid = uid });
            }
            return View();
        }

        public ActionResult GoocaBooca(string research_id, string uid)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(research_id);
            if (research.ResearchType != GoocaBoocaDataModels.ResearchType.GoocaBooca.ToString())
            {
                return RedirectToAction("Index", new { research_id = research_id, uid = uid });
            }
            var key = db.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId && n.Tag == "Key").FirstOrDefault();
            if(key !=null) ViewBag.KeyChoice = key.AnswerString;
            return View(db.GoocaBoocaResult(research_id, uid, this.Request.UserHostAddress));
        }

        public ActionResult Compare(string research_id, string uid)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(research_id);
            if (research.ResearchType != GoocaBoocaDataModels.ResearchType.Compare.ToString())
            {
                return RedirectToAction("Index", new { research_id = research_id, uid = uid });
            }

            return View(db.CompareResult(research_id, uid, this.Request.UserHostAddress));

        }

    }
}
