using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoocaBoocaDataModels.Model
{
    public class QuestionChoiceCount
    {
        public QuestionChoice QuestionChoice { get; set; }
        public Question Question { get; set; }
        public int Count { get; set; }
        public double Rate { get; set; }
    }

    public class GoocaBoocaAnswerData
    {
        public int UserId { get; set; }
        public double[] Data { get; set; }
        public List<Tuple<string, string>> Question { get; set; }
        public List<Tuple<string, string>> FreeAnswer { get; set; }
    }


    public class SummarizeData
    {
        GoocaBoocaDataBase db;
        public SummarizeData(GoocaBoocaDataBase db)
        {
            this.db = db;
        }

        public class CategoryChoiceCount
        {
            public ItemCategory Category { get; set; }
            public ItemAnswerChoice Choice { get; set; }
            public int Count { get; set; }

        }

        public class CategoryRate
        {
            public ItemCategory Category { get; set; }
            public double Rate { get; set; }
        }


        public IEnumerable<GoocaBoocaAnswerData> CreateGoocaBoocaAnswerData(string research_id)
        {
            List<GoocaBoocaAnswerData> list = new List<GoocaBoocaAnswerData>();
            var research = db.GetResearch(research_id);
            if (research == null) return list;

            var result_tmp = db.ItemAnsweres.Join(db.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId), n => n.User.UserId, n => n.User.UserId, (m, n) => m).Where(n => n.Research.ResearchId == research.ResearchId).GroupBy(n => new { n.User, n.ItemCategory, n.ItemAnswerChoice }).Select(n => new { User = n.Key.User, CategoryChoice = new CategoryChoiceCount() { Category = n.Key.ItemCategory, Choice = n.Key.ItemAnswerChoice, Count = n.Count() } });

            var result_tmp2 = result_tmp.GroupBy(n => n.User);


            var answerPattern = from choice in db.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId).ToArray()
                                from category in db.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId).ToArray()
                                select new CategoryChoiceCount() { Category = category, Choice = choice, Count = 0 };

            Dictionary<int, GoocaBoocaAnswerData> dic = new Dictionary<int, GoocaBoocaAnswerData>();
            foreach (var item in result_tmp.GroupBy(n => n.User))
            {
                var rate = item.Select(n => n.CategoryChoice).Concat(answerPattern).GroupBy(n => new { n.Category }).Select(n => new CategoryRate() { Category = n.Key.Category, Rate = (double)n.Where(m => m.Choice.Tag == "Key").Sum(m => m.Count) / (double)n.Sum(m => m.Count) }).OrderBy(n=>n.Category.ItemCategoryId);
                dic.Add(item.Key.UserId, new GoocaBoocaAnswerData() { UserId = item.Key.UserId, Data = rate.Select(n => n.Rate).ToArray() });
            }

            var questionAnswer = db.QuestionAnsweres.Where(n => n.Research.ResearchId == research.ResearchId).GroupBy(n => n.User.UserId);

            foreach (var item in questionAnswer)
            {
                if (dic.ContainsKey(item.Key))
                {
                    dic[item.Key].Question = new List<Tuple<string, string>>();                
                    foreach (var item2 in item)
                    {
                        dic[item.Key].Question.Add(new Tuple<string, string>(item2.Question.QuestionName, item2.QuestionChoice.QuestionChoiceText));
                    }
                }
            }
            foreach (var item in db.FreeAnsweres.Where(n=>n.Question.Research.ResearchId == research.ResearchId).GroupBy(n=>n.User.UserId))
            {
                if (dic.ContainsKey(item.Key))
                {
                    dic[item.Key].FreeAnswer = new List<Tuple<string, string>>();
                    foreach (var item2 in item)
                    {
                        dic[item.Key].FreeAnswer.Add(new Tuple<string, string>(item2.Question.QuestionName, item2.FreeTest));
                    }
                }              
            }
            return dic.Values;
        }


        public IEnumerable<Tuple<QuestionChoice, int>> GetQuestionChoiceAll(string research_id)
        {
            var research = db.GetResearch(research_id);
            if (research == null) return new List<Tuple<QuestionChoice, int>>();

            var resultAll = from answer in db.QuestionAnsweres.Where(n => n.Research.ResearchId == research.ResearchId)
                            join complete in db.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId)
                            on answer.User.UserId equals complete.User.UserId into z
                            select answer;
            return resultAll.GroupBy(n => n.QuestionChoice).Select(n => new { n.Key, Count = n.Count() }).ToArray().Select(n => new Tuple<QuestionChoice, int>(n.Key, n.Count));
        }


        public IEnumerable<CompareAnalyzeData> CompareData(string research_id)
        {
            var research = db.GetResearch(research_id);

            if (research == null) return new List<CompareAnalyzeData>();
            var items = db.Items.Where(n => n.Resarch.ResearchId == research.ResearchId).Select(n => n.ItemId).ToArray();
            var itemAttribute = db.ItemAttributes.Where(n => items.Contains(n.Item.ItemId)).ToArray();
            var users = db.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId).Select(n => n.User).ToArray();
            var resultAll = from answer in db.ItemCompareAnsweres.Where(n => n.Research.ResearchId == research.ResearchId)
                            join complete in db.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId)
                            on answer.User.UserId equals complete.User.UserId into z
                            select answer;
            var groupUser = resultAll.GroupBy(n => n.User.UserId);
            var count = db.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId).Count();
            Dictionary<int, List<Tuple<string, string, double>>> userData = new Dictionary<int, List<Tuple<string, string, double>>>();

            foreach (var data in groupUser)
            {
                List<Tuple<string, string>> list = new List<Tuple<string, string>>();

                //属性の大小関係書き出し
                foreach (var item in data)
                {
                    if (item.ItemBad != null && item.ItemGood != null)
                    {
                        if (item.ItemBad.ItemAttribute != null && item.ItemGood.ItemAttribute != null)
                        {
                            var atribute = from good in item.ItemGood.ItemAttribute
                                           from bad in item.ItemBad.ItemAttribute
                                           where good.AttributeName == bad.AttributeName && good.Value != bad.Value
                                           select new { good = good.Value, bad = bad.Value };
                            foreach (var pair in atribute)
                            {
                                list.Add(new Tuple<string, string>(pair.good, pair.bad));
                            }
                        }
                    }
                }
                //ペアの組み合わせの数をかぞえる。
                var result = list.GroupBy(n => new { n.Item1, n.Item2 }).Select(n => new Tuple<string, string, int, string>(n.Key.Item1, n.Key.Item2, n.Count(), Tool.GetSortText(n.Key.Item1, n.Key.Item2))).GroupBy(n => n.Item4);

                List<Tuple<string, string, double>> list2 = new List<Tuple<string, string, double>>();
                foreach (var item in result)
                {
                    var tmp = item.OrderByDescending(n => n.Item3).ToArray();
                    if (tmp.Length > 1)
                    {
                        list2.Add(new Tuple<string, string, double>(tmp.First().Item1, tmp.First().Item2, (double)tmp.First().Item3 / (double)(tmp.First().Item3 + tmp.Last().Item3)));
                    }
                    else
                    {
                        list2.Add(new Tuple<string, string, double>(tmp.First().Item1, tmp.First().Item2, (double)tmp.First().Item3 / (double)(tmp.First().Item3)));
                    }
                }
                if (list2.Count > 0)
                {
                    userData.Add(data.Key, list2);
                }
            }

            System.Collections.Concurrent.ConcurrentBag<Tuple<string, List<string>, int>> stockData = new System.Collections.Concurrent.ConcurrentBag<Tuple<string, List<string>, int>>();

            System.Threading.Tasks.Parallel.ForEach(userData, (data) =>
                {
                    foreach (var item in Tool.ConverMuitiRelation(data.Value, 0.7, false))
                    {
                        stockData.Add(new Tuple<string, List<string>, int>(item.Item1.Aggregate((n, m) => n + "\t" + m), item.Item1, data.Key));
                    }
                });

            var result3 = stockData.GroupBy(n => n.Item1).Select(n => new CompareAnalyzeData()
           {
               AttributeOrder = n.First().Item2,
               Rate = (double)n.Count() * 100 / (double)userData.Count,
               Count = n.Count(),
               UserIds = n.Select(m => m.Item3).ToList()
           }).ToList();

            int i = 0;
            foreach (var item in result3)
            {
                item.Id = "compare" + i.ToString();
                List<int> idList = new List<int>(item.UserIds);
                item.QuestionAnswer = db.QuestionAnsweres.Where(n => idList.Contains(n.User.UserId)).GroupBy(n => n.QuestionChoice).Select(n => new { n.Key, Count = n.Count() }).ToArray().Select(n => new Tuple<QuestionChoice, int>(n.Key, n.Count));
                i++;
            }
            return result3;
        }

    }


    public class CompareAnalyzeData
    {
        public string Id { get; set; }
        public List<int> UserIds { get; set; }
        public List<string> AttributeOrder { get; set; }
        public double Rate { get; set; }
        public int Count { get; set; }
        public string Key { get; set; }
        public IEnumerable<Tuple<QuestionChoice, int>> QuestionAnswer { get; set; }
    }


    public class SummarizeCompletedUser
    {
        public DateTime DateTime { get; set; }
        public int SumCount { get; set; }
        public IEnumerable<int> CountList { get; set; }
    }
}
