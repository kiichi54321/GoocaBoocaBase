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
            if (research.ResearchType == "GoocaBoocaText")
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
            if (research == null) return View();
            ViewBag.ResearchName = research.ResearchName;
            if (research.ResearchType.Contains("GoocaBooca") == false)
            {
                return RedirectToAction("Index", new { research_id = research_id, uid = uid });
            }
            var key = db.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId && n.Tag == "Key").FirstOrDefault();
            if (key != null) ViewBag.KeyChoice = key.AnswerString;

            var list = db.GoocaBoocaResult(research_id, uid, this.Request.UserHostAddress);

            ViewBag.CategoryList = list.GroupBy(n => n.Item1).Select(n => "\"" + n.Key.ItemCategoryName + "\"").Aggregate((n, m) => n + "," + m );

            var data = list.GroupBy(n => n.Item1).Select(n => n.Where(m => m.Item2.Tag == "Key").FirstOrDefault().Item3 * 100 / (double)n.Sum(m => m.Item3));
            ViewBag.StringData = data.Select(n=>n.ToString()).Aggregate((n,m)=>n+","+m);


            var vList = db.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId).OrderBy(n => n.ItemCategoryId).Select(n => n.ItemCategoryName).ToArray().Aggregate((n, m) => n + ",\"" + m + "\"");


            return View(list);
        }

        public ActionResult GoocaBoocaCategoryArrary(string research_id)
        {
            if (Request.IsAjaxRequest())
            {
                GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
                var research = db.GetResearch(research_id);
                var list = db.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId).OrderBy(n => n.ItemCategoryId).Select(n => n.ItemCategoryName).ToArray();
                return Json(list);
            }
            else
            {
                return new EmptyResult();
            }
        }


        public ActionResult Compare(string research_id, string uid)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(research_id);
            if (research == null) return View();
            ViewBag.ResearchName = research.ResearchName;
            if (research.ResearchType != GoocaBoocaDataModels.ResearchType.Compare.ToString())
            {
                return RedirectToAction("Index", new { research_id = research_id, uid = uid });
            }
            var list = GoocaBoocaDataModels.Tool.ConverMuitiRelation(db.CompareResult(research_id, uid, this.Request.UserHostAddress), 0.7);
            int max = 0;
            foreach (var item in list)
            {
                max = Math.Max(max, item.Item1.Count);
            }
            ViewBag.maxLen = max;
            if (list.Any())
            {
                ViewBag.ItemList = db.GetItemSelectedByAttribute(list.FirstOrDefault().Item1);
            }
            else
            {
                ViewBag.ItemList = new List<GoocaBoocaDataModels.Item>();
            }
            return View(list);

        }

    }
}
