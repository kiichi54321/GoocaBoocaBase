using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoocaBoocaBase.Models;

namespace GoocaBoocaBase.Controllers
{
    public class AnalyzeController : AsyncController
    {
        //
        // GET: /Analyze/

        public ActionResult Index()
        {
            using (GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase())
            {
                var model = new GoocaBoocaBase.Models.AnalyzeIndexModel();
                model.Researches = db.Researches.ToArray();

                return View(model);
            }
        }

        public ActionResult CompareAsync(string researchId)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            {
                GoocaBoocaDataModels.Model.SummarizeData sd = new GoocaBoocaDataModels.Model.SummarizeData(db);
                AsyncManager.OutstandingOperations.Increment(4);

                AnalyzeCompareModel acm = new AnalyzeCompareModel();
                acm.Result = sd.CompareData(researchId);
                acm.ALLQuestionResult = sd.GetQuestionChoiceAll(researchId);
                AsyncManager.Parameters.Add("result", acm);
                AsyncManager.OutstandingOperations.Decrement(4);
                return View();
            }
        }

        public ActionResult CompareCompleted(AnalyzeCompareModel result)
        {
            return View(result);
        }

        public ActionResult TestPage(string choiceid1, string choiceid2,string choiceid3)
        {
            int _choiceid1, _choiceid2, _choiceid3;
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            
            int.TryParse(choiceid1,out _choiceid1);
            var choice1 = db.QuestionChoices.Where(n => n.QuestionChoiceId == _choiceid1).FirstOrDefault();
            if (choice1 == null) return View();
            var choice1User = db.QuestionAnsweres.Where(n => n.QuestionChoice.QuestionChoiceId == choice1.QuestionChoiceId).Select(n => n.User);
            ViewBag.Choice1 = choice1.QuestionChoiceText;
            if (int.TryParse(choiceid2, out _choiceid2))
            {
                var choice2 = db.QuestionChoices.Where(n => n.QuestionChoiceId == _choiceid2).FirstOrDefault();
                if (choice2 != null)
                {
                    var choice2User = db.QuestionAnsweres.Where(n => n.QuestionChoice.QuestionChoiceId == choice2.QuestionChoiceId).Select(n => n.User);
                    choice1User = choice1User.Intersect(choice2User);
                    ViewBag.Choice2 = choice2.QuestionChoiceText;
                }
            }
            if (int.TryParse(choiceid3, out _choiceid3))
            {
                var choice3 = db.QuestionChoices.Where(n => n.QuestionChoiceId == _choiceid3).FirstOrDefault();
                if (choice3 != null)
                {
                    var choice3User = db.QuestionAnsweres.Where(n => n.QuestionChoice.QuestionChoiceId == choice3.QuestionChoiceId).Select(n => n.User);
                    choice1User = choice1User.Intersect(choice3User);
                    ViewBag.Choice3 = choice3.QuestionChoiceText;
                }
            }
     

            var goodItem = db.ItemCompareAnsweres.Where(n => choice1User.Contains(n.User)).GroupBy(n => new { n.User, n.ItemGood }).Select(n => new CountData(){ User = n.Key.User, Item = n.Key.ItemGood, Count = n.Count() });
            var badItem = db.ItemCompareAnsweres.Where(n => choice1User.Contains(n.User)).GroupBy(n => new { n.User, n.ItemBad }).Select(n => new CountData() {User = n.Key.User, Item = n.Key.ItemBad, Count = n.Count() });

            var itemCount = goodItem.Concat(badItem).GroupBy(n => new { n.User, n.Item }).Select(n => new CountData() { User = n.Key.User, Item = n.Key.Item, Count = n.Sum(m => m.Count) });

            var allPair = from u in choice1User
                          from item in db.Items
                          where item.Resarch.ResearchId == 2
                          select new CountData() { User = u, Item = item, Count = 0 };

            var goodItem2 = goodItem.Concat(allPair).GroupBy(n => new { n.Item, n.User }).Select(n => new CountData() { User = n.Key.User, Item = n.Key.Item, Count = n.Sum(m => m.Count) });



            //db.ItemCompareAnsweres. Select(n=> new { n.User,Item = n.ItemBad }).Concat(db.ItemCompareAnsweres.Select(n=>new { n.User,Item = n.ItemGood }).GroupBy(n=>new { n.User,n.Item});.Select(n=>new { n.Key.User,n.Key.Item,Count = n.Count()});

            //var result = from answer in goodItem
            //             join iCount in itemCount on new { answer.Itme,answer.User} equals new { iCount.Itme,iCount.User} into z                       
            //             select new { answer.Itme, answer.User, answer.Count, Rate = (double)answer.Count / (double)z.FirstOrDefault().Count };
            var result = from answer in itemCount
                         join iCount in goodItem2 on new { answer.Item.ItemId, answer.User.UserId } equals new { iCount.Item.ItemId, iCount.User.UserId } into z                     
                         select new { answer.Item, answer.User, Rate = (double)z.FirstOrDefault().Count / (double)answer.Count };

            var result2 = result.GroupBy(n => n.Item).Select(n => new {Name = n.Key, Avg = n.Average(m => m.Rate)  }).ToArray().Select(n=> new Tuple<GoocaBoocaDataModels.Item,double>(n.Name,n.Avg));



            return View(result2);
        }

        class CountData
        {
            public GoocaBoocaDataModels.User User { get; set; }
            public GoocaBoocaDataModels.Item Item { get; set; }
            public int Count { get; set; }
        }
    }
}
