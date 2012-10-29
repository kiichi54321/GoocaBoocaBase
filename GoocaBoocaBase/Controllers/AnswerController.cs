using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GoocaBoocaBase.Controllers
{
    public class AnswerController : Controller
    {
        //
        // GET: /Answer/

        public ActionResult Index()
        {
            return View();
        }

        private bool CheckReferrer()
        {
            if (this.HttpContext.Request.UrlReferrer == null || this.HttpContext.Request.UrlReferrer.Host != this.HttpContext.Request.Url.Host)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public ActionResult SimpleAddUser(string UserName, string research_id)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            string uid = db.GetUserId(UserName, this.Request.UserHostAddress);
            return RedirectToAction("Simple", new { research_id = research_id, uid = uid });
        }



        public ActionResult Simple(string research_id, string image_id, string answer_id, string uid)
        {
            if (CheckReferrer() == false) return RedirectToAction("Index", "Home");

            if (uid == null || research_id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.research_id = research_id;
                ViewBag.uid = uid;
            }

            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            if (image_id != null && answer_id != null)
            {
                db.InsertItemAnswer(research_id, image_id, uid, answer_id, this.Request.UserHostAddress);
            }

            var data = db.GetNextImageId(research_id, uid, this.Request.UserHostAddress);

            if (data.Success)
            {
                ViewBag.item_id = data.ImageId;
                ViewBag.answerCount = data.AnswerCount;
                ViewBag.Message = data.Message;
            }
            else
            {
                return RedirectToAction("SimpleQuestion", new { research_id = research_id, uid = uid });
            }
            

            return View();
        }

        public ActionResult SimpleQuestion(string research_id, string uid)
        {
            if (CheckReferrer() == false) return RedirectToAction("Index", "Home");

            if (uid == null || research_id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.research_id = research_id;
                ViewBag.uid = uid;
            }
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            return View(db.GetQuestion(research_id).ToArray());
        }

        [HttpPost]
        public ActionResult SimpleLastQuestion(string research_id, string uid, FormCollection paraList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in paraList.Keys)
            {
                string v = paraList.GetValue(item.ToString()).RawValue.ToString();
                dic.Add(item.ToString(), v);
            }
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            db.InsertQuestionAnswer(research_id, uid, this.Request.UserHostAddress, dic);
            
            return View();

        }


        public ActionResult Twitter()
        {
            if (CheckReferrer() == false) return RedirectToAction("Index", "Home");
            return View();

        }

        public ActionResult FaceBook()
        {
            if (CheckReferrer() == false) return RedirectToAction("Index", "Home");
            return View();
        }

        //public ActionResult test(string research_id,string image_id
    }
}
