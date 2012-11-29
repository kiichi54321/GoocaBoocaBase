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

        MyLib.Analyze.WardMethod wardMethod;

        public ActionResult GoocaBoocaAsync(string researchId, int ClusterNum, string start)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            GoocaBoocaDataModels.Model.SummarizeData sd = new GoocaBoocaDataModels.Model.SummarizeData(db);

            if (wardMethod == null || start != null)
            {
                wardMethod = new MyLib.Analyze.WardMethod();
                foreach (var item in sd.CreateGoocaBoocaAnswerData(researchId))
                {
                    wardMethod.AddData(new MyLib.Analyze.WardMethod.Data() { Value = item.Data, Tag = item });
                }
                AsyncManager.OutstandingOperations.Increment(8);
                wardMethod.Run();
                AsyncManager.Parameters.Add("researchId", researchId);
                AsyncManager.Parameters.Add("ClusterNum", ClusterNum);
                AsyncManager.OutstandingOperations.Decrement(8);
            }

            return View();
        }

        public ActionResult GoocaBoocaAsyncCompleted(string researchId, int ClusterNum, string start)
        {
            var clist = wardMethod.GetCluster(ClusterNum);
            List<Cluster> list = new List<Cluster>();
            foreach (var item in clist)
            {
                list.Add(new Cluster()
                {
                    DataStr = item.CenterData.Value.Select(n => n.ToString()).Aggregate((m, n) => m + "," + n),
                    FreeAnswer = ((GoocaBoocaDataModels.Model.GoocaBoocaAnswerData)item.Tag).FreeAnswer.Select(n => n.Item2).ToArray()
                });
            }

            return View(list);
        }

        public class Cluster
        {
            public string DataStr { get; set; }
            public string[] FreeAnswer { get; set; }

        }

        public ActionResult TestPage(string choiceid1, string choiceid2, string choiceid3)
        {
            int _choiceid1, _choiceid2, _choiceid3;
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            //  db.GetResearch("");
            int.TryParse(choiceid1, out _choiceid1);
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


            var goodItem = from compare in db.ItemCompareAnsweres
                           from users in choice1User
                           where compare.User.UserId == users.UserId
                           group compare by new { compare.User, compare.ItemGood } into n
                           select new CountData() { User = n.Key.User, Item = n.Key.ItemGood, Count = n.Count() };

            var badItem = from compare in db.ItemCompareAnsweres
                          from users in choice1User
                          where compare.User.UserId == users.UserId
                          group compare by new { compare.User, compare.ItemBad } into n
                          select new CountData() { User = n.Key.User, Item = n.Key.ItemBad, Count = n.Count() };


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

            var result2 = result.GroupBy(n => n.Item).Select(n => new { Name = n.Key, Avg = n.Average(m => m.Rate) }).ToArray().Select(n => new Tuple<GoocaBoocaDataModels.Item, double>(n.Name, n.Avg));



            return View(result2);
        }



        public ActionResult KeionAsync()
        {
            IEnumerable<CountData2> keionData = null;
            ViewBag.Start = DateTime.Now;
            DateTime start = DateTime.Now;
            AsyncManager.OutstandingOperations.Increment(8);

            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var choice1User = db.QuestionAnsweres.Where(n => n.Research.ResearchId == 2).Select(n => n.User);
            var answerData = (from compare in db.ItemCompareAnsweres
                              join users in choice1User on compare.User.UserId equals users.UserId
                              select compare).ToArray();
            var choiceUser = choice1User.ToArray();
            ViewBag.DBend = (DateTime.Now - start).TotalMinutes.ToString("F1");
            IEnumerable<CountData> goodItem = null;
            IEnumerable<CountData> badItem = null;
            IEnumerable<CountData> allPair = null;
            IEnumerable<CountData> itemCount = null;
            IEnumerable<CountData> goodItem2 = null;
            List<UserData> userData = new List<UserData>();
            var Items = db.Items.Where(n => n.Resarch.ResearchId == 2).ToArray();
            var task1 = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                goodItem = (from compare in answerData.AsParallel()
                            group compare by new { compare.User, compare.ItemGood } into n
                            select new CountData() { User = n.Key.User, Item = n.Key.ItemGood, Count = n.Count() }).ToArray();
            });
            var task2 = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    badItem = (from compare in answerData.AsParallel()
                               group compare by new { compare.User, compare.ItemBad } into n
                               select new CountData() { User = n.Key.User, Item = n.Key.ItemBad, Count = n.Count() }).ToArray();
                });
            var task3 = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    allPair = (from u in choiceUser
                               from item in Items
                               select new CountData() { User = u, Item = item, Count = 0 }).ToArray();
                });
            System.Threading.Tasks.Task.WaitAll(new System.Threading.Tasks.Task[] { task1, task2, task3 });

            var task4 = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    itemCount = goodItem.Concat(badItem).GroupBy(n => new { n.User, n.Item }).Select(n => new CountData() { User = n.Key.User, Item = n.Key.Item, Count = n.Sum(m => m.Count) }).ToArray();
                });
            var task5 = System.Threading.Tasks.Task.Factory.StartNew(() =>
                      {
                          goodItem2 = goodItem.Concat(allPair.AsParallel()).GroupBy(n => new { n.Item, n.User }).Select(n => new CountData() { User = n.Key.User, Item = n.Key.Item, Count = n.Sum(m => m.Count) }).ToArray();
                      });

            var task6 = System.Threading.Tasks.Task.Factory.StartNew(() =>
                     {
                         foreach (var item in db.QuestionAnsweres.Where(n => n.Research.ResearchId == 2).GroupBy(n => n.User))
                         {
                             userData.Add(UserData.Create(item.Key, item));
                         }
                     });
            System.Threading.Tasks.Task.WaitAll(new System.Threading.Tasks.Task[] { task4, task5, task6 });

            var result = from answer in itemCount.AsParallel()
                         join iCount in goodItem2.AsParallel() on new { answer.Item.ItemId, answer.User.UserId } equals new { iCount.Item.ItemId, iCount.User.UserId } into z
                         select new { answer.Item, answer.User, Rate = (double)z.FirstOrDefault().Count / (double)answer.Count };

            var result3 = from r in result.ToArray().AsParallel()
                          join u in userData.AsParallel() on r.User.UserId equals u.User.UserId into z
                          select new CountData2() { User = r.User, UserData = z.FirstOrDefault(), Item = r.Item, Rate = r.Rate };
            keionData = result3.ToArray();

            AsyncManager.Parameters.Add("dataList", keionData);
            AsyncManager.OutstandingOperations.Decrement(8);

            return View(keionData);
        }

        public ActionResult KeionCompleted(IEnumerable<CountData2> dataList)
        {
            return View(dataList);
        }

        public class CountData2 : CountData
        {
            public UserData UserData { get; set; }
            public double Rate { get; set; }
        }

        public class CountData
        {
            public GoocaBoocaDataModels.User User { get; set; }
            public GoocaBoocaDataModels.Item Item { get; set; }
            public int Count { get; set; }
        }

        public class UserData
        {
            public GoocaBoocaDataModels.User User { get; set; }
            public string niku { get; set; }
            public string keion { get; set; }
            public string gender { get; set; }

            public static UserData Create(GoocaBoocaDataModels.User user, IEnumerable<GoocaBoocaDataModels.QuestionAnswer> list)
            {
                UserData ud = new UserData();
                ud.User = user;
                ud.gender = list.Where(n => n.Question.QuestionId == 6).FirstOrDefault().QuestionChoice.QuestionChoiceText;
                ud.keion = list.Where(n => n.Question.QuestionId == 9).FirstOrDefault().QuestionChoice.QuestionChoiceText;
                ud.niku = list.Where(n => n.Question.QuestionId == 8).FirstOrDefault().QuestionChoice.QuestionChoiceText;

                return ud;
            }
        }
    }
}
