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
            var research = db.GetResearch(research_id);
            if (research != null)
            {
                if (research.ResearchType == GoocaBoocaDataModels.ResearchType.GoocaBooca.ToString())
                {
                    return RedirectToAction("Simple", new { research_id = research_id, uid = uid });
                }
                else if (research.ResearchType == GoocaBoocaDataModels.ResearchType.Compare.ToString())
                {
                    return RedirectToAction("SimpleCompare", new { research_id = research_id, uid = uid });
                }
                else if (research.ResearchType == "GoocaBoocaText")
                {
                    return RedirectToAction("SimpleText", new { research_id = research_id, uid = uid });
                }
            }
            return RedirectToAction("Simple", new { research_id = research_id, uid = uid });

        }

        public ActionResult StartPage(string researchIdName)
        {
            if (researchIdName == null)
            {
                return RedirectToAction("Index", "Home");
            }

            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(researchIdName);
            if (research == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Title = research.ResearchName;
            return View(research);
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
            var research = db.GetResearch(research_id);
            ViewBag.Title = research.ResearchName;
            if (research.ResearchType == GoocaBoocaDataModels.ResearchType.GoocaBooca.ToString())
            {
                if (image_id != null && answer_id != null)
                {
                    db.InsertItemAnswer(research_id, image_id, uid, answer_id, this.Request.UserHostAddress);
                }
                ViewBag.Description = research.Description.Replace("\n","<br>");
                ViewBag.QuestionText = research.QuestionText.Replace("\n", "<br>");


                var data = db.GetNextImageId(research_id, uid, this.Request.UserHostAddress);

                if (data.Success)
                {
                    ViewBag.item_id = data.ImageId;
                    ViewBag.ImageUrl = data.ImageUrl;
                    ViewBag.answerCount = data.AnswerCount;
//                    ViewBag.Message = data.Message;
                    ViewBag.MaxCount = research.AnswerCount;
                }
                else
                {
                    return RedirectToAction("SimpleQuestion", new { research_id = research_id, uid = uid });
                }


                return View(db.ItemAnswerChoice.Where(n=>n.Research.ResearchId == research.ResearchId).ToArray());
            }
            else if (research.ResearchType == GoocaBoocaDataModels.ResearchType.Compare.ToString())
            {
                return RedirectToAction("SimpleCompare", new { research_id = research_id, uid = uid });
            }
            else 
            {
                return View();
            }
        }

        public ActionResult SimpleText(string research_id, string image_id, string answer_id, string uid)
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
            var research = db.GetResearch(research_id);
            ViewBag.Title = research.ResearchName;
            if (research.ResearchType == "GoocaBoocaText")
            {
                if (image_id != null && answer_id != null)
                {
                    db.InsertItemAnswer(research_id, image_id, uid, answer_id, this.Request.UserHostAddress);
                }
                ViewBag.Description = research.Description.Replace("\n", "<br>");
                ViewBag.QuestionText = research.QuestionText.Replace("\n", "<br>");


                var data = db.GetNextImageId(research_id, uid, this.Request.UserHostAddress);

                if (data.Success)
                {
                    ViewBag.item_id = data.ImageId;
                    ViewBag.ImageUrl = data.ImageUrl;
                    ViewBag.answerCount = data.AnswerCount;
                    ViewBag.TextTitle = db.ItemAttributes.Where(n => n.Item.ItemId == data.Item.ItemId && n.AttributeName == "Title").FirstOrDefault().Value;

                    var subTitle=  MyLib.Web.GetTagContent(data.Item.Tag, "[[", "]]");
                    ViewBag.SubTitle = subTitle;
                    ViewBag.Text = data.Item.Tag.Replace("[[" + subTitle + "]]", string.Empty);
                    //                    ViewBag.Message = data.Message;
                    ViewBag.MaxCount = research.AnswerCount;
                }
                else
                {
                    return RedirectToAction("SimpleQuestion", new { research_id = research_id, uid = uid });
                }


                return View(db.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId).ToArray());
            }
            else if (research.ResearchType == GoocaBoocaDataModels.ResearchType.Compare.ToString())
            {
                return RedirectToAction("SimpleCompare", new { research_id = research_id, uid = uid });
            }
            else
            {
                return View();
            }
        }


        public ActionResult SimpleCompare(string research_id, string selected_image_id, string noSelected_image_id, string uid)
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
            var research = db.GetResearch(research_id);
            ViewBag.QuestionText = research.QuestionText;
            ViewBag.Description = research.Description;
            ViewBag.MaxCount = research.AnswerCount;
            ViewBag.Title = research.ResearchName;  
            if (selected_image_id != null && noSelected_image_id != null)
            {
                db.InsertItemCompareAnswer(research_id, uid, this.Request.UserHostAddress, selected_image_id, noSelected_image_id);
            }

            var data = db.GetNextCompareImage(research_id, uid, this.Request.UserHostAddress);
            if (data.Success)
            {
                ViewBag.ItemA = data.ImageAId;
                ViewBag.ItemB = data.ImageBId;
                ViewBag.ImageAUrl = data.ImageAUrl;
                ViewBag.ImageBUrl = data.ImageBUrl;
                ViewBag.Message = data.Message;
                ViewBag.answerCount = data.AnswerCount;
            }
            if (data.Completed)
            {
                return RedirectToAction("SimpleQuestion", new { research_id = research_id, uid = uid });
            }

            return View();

        }


        public ActionResult SimpleQuestion(string research_id, string uid,string flag , FormCollection paraList)
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
            var research = db.GetResearch(research_id);
            ViewBag.Title = research.ResearchName;
            if (flag != null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                foreach (var item in paraList.Keys)
                {
                    string[] v = paraList.GetValue(item.ToString()).RawValue as string[];
                    dic.Add(item.ToString(), v.First());
                }
                var f = db.InsertQuestionAnswer(research_id, uid, this.Request.UserHostAddress, dic);

                if (f)
                {
                    return RedirectToAction("SimpleLastQuestion", new { research_id = research_id, uid = uid });
                }
                else
                {
                    ViewBag.ErrMessage = "すべてを回答してください";
                }

            }
            return View(db.GetQuestion(research_id).ToArray());
        }

        public ActionResult SimpleLastQuestion(string research_id, string uid, FormCollection paraList)
        {
            ViewBag.research_id = research_id;
            ViewBag.uid = uid;
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var research = db.GetResearch(research_id);
            ViewBag.Title = "これで終わりです。";
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
