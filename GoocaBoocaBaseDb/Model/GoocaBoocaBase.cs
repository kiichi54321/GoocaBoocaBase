using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Drawing;

namespace GoocaBoocaDataModels
{
    public class GoocaBoocaDataBase : DbContext
    {
        public GoocaBoocaDataBase()
            : base(Secret.connectionString)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<ItemAnswer> ItemAnsweres { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questiones { get; set; }
        public DbSet<QuestionAnswer> QuestionAnsweres { get; set; }
        public DbSet<QuestionChoice> QuestionChoices { get; set; }
        public DbSet<Research> Researches { get; set; }
        public DbSet<ItemAnswerChoice> ItemAnswerChoice { get; set; }
        public DbSet<TwitterLogin> TwitterLogins { get; set; }
        public DbSet<FaceBookLogin> FaceBookLogins { get; set; }
        public DbSet<UserImage> UserImage { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<UserAnswerCompleted> UserAnswerCompleted { get; set; }
        public DbSet<FreeAnswer> FreeAnsweres { get; set; }
        public DbSet<ItemCompareAnswer> ItemCompareAnsweres { get; set; }
        public DbSet<ItemAttribute> ItemAttributes { get; set; }
        public DbSet<QuestionAttribute> QuestionAttribute { get; set; }
        //    public DbSet<GenderType> GenderType { get; set; }
        //    public DbSet<QuestionType> QuestionType { get; set; }

        public void InsertItemAnswer(int research_id, int item_id, int user_id, int answer_id)
        {
            var research = this.Researches.Where(n => n.ResearchId == research_id).FirstOrDefault();
            var item = this.Items.Where(n => n.ItemId == item_id).FirstOrDefault();
            var user = this.Users.Where(n => n.UserId == user_id).FirstOrDefault();
            var choice = this.ItemAnswerChoice.Where(n => n.ItemAnswerChoiceId == (int)answer_id).FirstOrDefault();

            if (research == null || item == null || user == null || choice == null)
            {
                throw new Exception("値が不正です");
            }

            var a = this.ItemAnsweres.Where(n => n.User.UserId == user.UserId && n.Item.ItemId == item.ItemId).FirstOrDefault();
            if (a == null)
            {
                ItemAnswer answer = new ItemAnswer()
                {
                    Item = item,
                    ItemAnswerChoice = choice,
                    ItemCategory = item.Category,
                    Research = research,
                    User = user, 
                };
                answer.SetDate();
                this.ItemAnsweres.Add(answer);
            }
            else
            {
                a.ItemAnswerChoice = choice;
                a.Upd_Date = DateTime.Now;
                this.ItemAnsweres.Add(a);
            }
            this.SaveChanges();

        }

        public void InsertItemAnswer(string research_str, string item, string user, string answer, string ip)
        {
            int r, i, a;
            Nullable<int> u = Tool.GetUserId(user, ip);
            var research = GetResearch(research_str);
            if (research != null && int.TryParse(item, out i) && int.TryParse(answer, out a) && u.HasValue == true)
            {
                InsertItemAnswer(research.ResearchId, i, u.Value, a);
            }
            else
            {
                throw new Exception("値が不正です");
            }
        }


        public void InsertQuestion(int research_id, int question_id, int answer_id, int user_id)
        {
            var research = this.Researches.Where(n => n.ResearchId == research_id).FirstOrDefault();
            var question = this.Questiones.Where(n => n.QuestionId == question_id).FirstOrDefault();
            var user = this.Users.Where(n => n.UserId == user_id).FirstOrDefault();
            var choice = this.QuestionChoices.Where(n => n.QuestionChoiceId == answer_id).FirstOrDefault();

            if (research == null || question == null || user == null || choice == null)
            {
                throw new Exception("値が不正です");
            }

            QuestionAnswer q = new QuestionAnswer()
            {
                Question = question,
                QuestionChoice = choice,
                Research = research,
                User = user
            };
            q.SetDate();
            this.QuestionAnsweres.Add(q);
            this.SaveChanges();
        }

        public IEnumerable<Question> GetQuestion(string research_str)
        {
            var research = GetResearch(research_str);
            if (research != null)
            {
                return this.Questiones.Where(n => n.Research.ResearchId == research.ResearchId).OrderBy(n => n.QuestionOrder);
            }

            return new List<Question>();

        }



        public ImageDataStruct GetNextImageId(int research_id, int user_id)
        {
            ImageDataStruct data = new ImageDataStruct();

            var research = this.Researches.Where(n => n.ResearchId == research_id).FirstOrDefault();
            var user = this.Users.Where(n => n.UserId == user_id).FirstOrDefault();

            var all = this.Items.Where(n => n.Resarch.ResearchId == research.ResearchId);
            var answered = this.ItemAnsweres.Where(n => n.Research.ResearchId == research.ResearchId && n.User.UserId == user.UserId).Select(n => n.Item);
            var category = this.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId);

            var categoryCount = category.Count();
            var categoryMaxCount = research.AnswerCount / categoryCount;

            var diff = all.Except(answered);
            var answeredGroupby = answered.GroupBy(n => n.Category);

            data.AnswerCount = answered.Count();

            if (data.AnswerCount == research.AnswerCount)
            {
                data.Success = false;
                this.UserAnswerCompleted.Add(new UserAnswerCompleted() { Research = research, User = user, Reg_Date = DateTime.Now, IsActive = true, Upd_Date = DateTime.Now });
                this.SaveChanges();
                return data;
            }

            List<int> list = new List<int>();
            foreach (var item in answeredGroupby)
            {
                data.Message += item.Key.ItemCategoryName + ":" + item.Count().ToString() + "\r\n";
                if (item.Count() == categoryMaxCount)
                {
                    list.Add(item.Key.ItemCategoryId);
                    //                    diff =  diff.Where(n => n.Category.ItemCategoryId != item.Key.ItemCategoryId);                   
                }
            }

            if (diff.Any())
            {
                data.ImageId = diff.Where(n => list.Contains(n.Category.ItemCategoryId) == false).OrderBy(n => Guid.NewGuid()).First().ItemId;
                data.Success = true;
                return data;
            }
            else
            {
                data.Success = false;
                return data;
            }
        }

        public struct ImageDataStruct
        {
            public int ImageId { get; set; }
            public int AnswerCount { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public struct ImageCompareStruct
        {
            public int ImageAId { get; set; }
            public int ImageBId { get; set; }
            public int AnswerCount { get; set; }
            public bool Success { get; set; }
            public bool Completed { get; set; }
            public string Message { get; set; }
        }

        public ImageCompareStruct GetNextCompareImage(string research_str, string uid, string ip)
        {
            var userid = Tool.GetUserId(uid, ip);
            var research = GetResearch(research_str);
            var user = GetUser(uid, ip);
            if (research != null && userid.HasValue == true)
            {
                var itemCompareAnsweres = this.ItemCompareAnsweres.Where(n => n.Research.ResearchId == research.ResearchId && n.User.UserId == userid.Value);
                if (itemCompareAnsweres.Count() < research.AnswerCount)
                {
                    List<string> list = new List<string>();

                    var items = this.Items.Where(n => n.Resarch.ResearchId == research.ResearchId).OrderBy(n => n.ItemId);

                    int i = 1;
                    foreach (var item in items.Take(items.Count() - 1))
                    {
                        foreach (var item2 in items.Skip(i))
                        {
                            if (item.Category.ItemCategoryId != item2.Category.ItemCategoryId)
                            {
                                list.Add(item.ItemId.ToString() + "_" + item2.ItemId.ToString());
                            }
                        }
                        i++;
                    }

                    var answerKey = itemCompareAnsweres.Select(n => n.PairKey).ToList<string>();
                    list = new List<string>(list.Except(answerKey));
                    var key = list.OrderBy(n => Guid.NewGuid()).FirstOrDefault();

                    if (key != null)
                    {
                        var keys = key.Split('_').OrderBy(n => Guid.NewGuid()).ToArray();
                        return new ImageCompareStruct() { ImageAId = int.Parse(keys.First()), ImageBId = int.Parse(keys.Last()), Success = true, AnswerCount = itemCompareAnsweres.Count(), Completed = false };
                    }

                }
                else
                {
                    if (user != null)
                    {
                        var ua = new UserAnswerCompleted() { Research = research, User = user };
                        ua.SetDate();
                        this.UserAnswerCompleted.Add(ua);
                    }
                    this.SaveChanges();
                    return new ImageCompareStruct() { Completed = true, Success = true };
                }

            }
            return new ImageCompareStruct() { Success = false };

        }

        public void InsertItemCompareAnswer(string research_str, string uid, string ip, string selectedItem, string unSelectedItem)
        {
            var userid = Tool.GetUserId(uid, ip);
            var research = GetResearch(research_str);
            int seleted, unSelected;
            if (research != null && userid.HasValue && int.TryParse(selectedItem, out seleted) && int.TryParse(unSelectedItem, out unSelected))
            {
                var user = this.Users.Where(n => n.UserId == userid).FirstOrDefault();
                var good = this.Items.Where(n => n.ItemId == seleted).FirstOrDefault();
                var bad = this.Items.Where(n => n.ItemId == unSelected).FirstOrDefault();

                if (user != null && good != null && bad != null)
                {
                    string key;
                    if (seleted < unSelected)
                    {
                        key = seleted + "_" + unSelected;
                    }
                    else
                    {
                        key = unSelected + "_" + seleted;
                    }

                    ItemCompareAnswer a = new ItemCompareAnswer()
                    {
                        User = user,
                        ItemGood = good,
                        Research = research,
                        ItemBad = bad,
                        PairKey = key
                    };
                    a.SetDate();
                    this.ItemCompareAnsweres.Add(a);
                    this.SaveChanges();
                }
            }
        }


        public ImageDataStruct GetNextImageId(string research_str, string uid, string ip)
        {
            var userid = Tool.GetUserId(uid, ip);
            var research = GetResearch(research_str);
            if (research != null && userid.HasValue)
            {
                return GetNextImageId(research.ResearchId, userid.Value);
            }
            else
            {
                return new ImageDataStruct() { Success = false };
            }
        }

        public string GetUserId(string userName, string ip)
        {
            //var user = this.Users.Where(n => n.UserName == userName).FirstOrDefault();
            //if (user == null)
            {
              var  user = new User()
                {
                    UserName = userName,
                };
              user.SetDate();
                this.Users.Add(user);
                this.SaveChanges();
                return Tool.ChangeUserId(user.UserId, ip);
            }
        }

        public bool InsertQuestionAnswer(string research_str, string uid, string ip, Dictionary<string, string> dic)
        {
            var userId = Tool.GetUserId(uid, ip);
            var research = GetResearch(research_str);
            if (research != null && userId.HasValue)
            {
                var user = this.Users.Where(n => n.UserId == userId.Value).FirstOrDefault();

                int count = 0;
                foreach (var item in dic)
                {
                    var d = item.Key.Split('_');
                    if (d.First() == "q" || d.First() == "f")
                    {
                        count++;
                    }
                }
                if (count != this.Questiones.Where(n => n.Research.ResearchId == research.ResearchId).Count())
                {
                    return false;
                }

                foreach (var item in dic)
                {
                    var d = item.Key.Split('_');
                    if (d.First() == "q")
                    {
                        int q_id;
                        int a_id;
                        if (int.TryParse(d.Last(), out q_id) && int.TryParse(item.Value.Split('_').LastOrDefault(), out a_id))
                        {
                            var question = this.Questiones.Where(n => n.QuestionId == q_id).FirstOrDefault();
                            var choice = this.QuestionChoices.Where(n => n.QuestionChoiceId == a_id).FirstOrDefault();
                            if (question != null && choice != null)
                            {
                                var answer = this.QuestionAnsweres.Where(n => n.User.UserId == user.UserId && n.Question.QuestionId == question.QuestionId).FirstOrDefault();
                                if (answer != null)
                                {
                                    answer.QuestionChoice = choice;
                                    answer.Upd_Date = DateTime.Now;
                                }
                                else
                                {
                                    QuestionAnswer qa = new QuestionAnswer()
                                    {
                                        Question = question,
                                        Research = research,
                                        User = user,
                                        QuestionChoice = choice,
                                    };
                                    qa.SetDate();
                                    this.QuestionAnsweres.Add(qa);
                                }
                            }
                        }
                    }
                    if (d.First() == "f")
                    {
                        int q_id;
                        if (int.TryParse(d.Last(), out q_id))
                        {
                            var question = this.Questiones.Where(n => n.QuestionId == q_id).FirstOrDefault();
                            if (question != null)
                            {
                                var ff = this.FreeAnsweres.Where(n => n.User.UserId == user.UserId && n.Question.QuestionId == question.QuestionId).FirstOrDefault();
                                if (ff != null)
                                {
                                    ff.FreeTest = item.Value;
                                    ff.Upd_Date = DateTime.Now;
                                }
                                else
                                {
                                    FreeAnswer fa = new FreeAnswer()
                                    {
                                        FreeTest = item.Value,
                                        Question = question,
                                        User = user,
                                    };
                                    fa.SetDate();
                                    this.FreeAnsweres.Add(fa);
                                }
                            }
                        }
                    }
                }
                this.SaveChanges();
                return true;
            }

            return false;
        }

        public Research GetResearch(string research_str)
        {
            return this.Researches.Where(n => n.ResearchIdName == research_str).FirstOrDefault();
        }

        public User GetUser(string uid, string ip)
        {
            var userId = Tool.GetUserId(uid, ip);
            if (userId.HasValue)
            {
                var user = this.Users.Where(n => n.UserId == userId.Value).FirstOrDefault();
                return user;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<ItemAnswerChoice> GetItemAnswerChoice(string research_str)
        {
            var research = GetResearch(research_str);
            if (research != null)
            {
                return this.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId).ToArray();
            }
            return new List<ItemAnswerChoice>();
        }

        public IEnumerable<Tuple<ItemCategory, ItemAnswerChoice, int>> GoocaBoocaResult(string research_str, string uid, string ip)
        {
            var research = GetResearch(research_str);
            var userId = Tool.GetUserId(uid, ip);
            var user = this.Users.Where(n => n.UserId == userId.Value).FirstOrDefault();

            if (research != null && user != null)
            {
                var result_tmp = this.ItemAnsweres.Where(n => n.Research.ResearchId == research.ResearchId && n.User.UserId == user.UserId).GroupBy(n => new { n.ItemCategory, n.ItemAnswerChoice }).Select(n => new { ItemCategory = n.Key.ItemCategory, ItemAnswerChoice = n.Key.ItemAnswerChoice, Count = n.Count() }).OrderBy(n=>n.ItemCategory.ItemCategoryId).ThenBy(n=>n.ItemAnswerChoice.ItemAnswerChoiceId) ;
                var answerPattern = from choice in this.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId).ToArray()
                                    from category in this.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId).ToArray()
                                    select new { Category = category, Choice = choice };

                var result = from ap in answerPattern.ToArray()
                             join userAnswer in result_tmp
                                on new { ap.Category.ItemCategoryId, ap.Choice.ItemAnswerChoiceId } equals new { userAnswer.ItemCategory.ItemCategoryId, userAnswer.ItemAnswerChoice.ItemAnswerChoiceId } into z
                             from a in z.DefaultIfEmpty()
                             select new { ap.Category, ap.Choice,Count = (a == null) ? 0 : a.Count };

                return result.OrderBy(n => n.Category.ItemCategoryId).ThenBy(n => n.Choice.ItemAnswerChoiceId).ToArray().Select(item => new Tuple<ItemCategory, ItemAnswerChoice, int>(item.Category, item.Choice, item.Count));

            }

            return new List<Tuple<ItemCategory, ItemAnswerChoice, int>>();
        }

        public IEnumerable<Tuple<string, string, double>> CompareResult(string research_str, string uid, string ip)
        {
            var research = GetResearch(research_str);
            var user = GetUser(uid, ip);
            if (research != null && user != null)
            {
                var result_tmp = this.ItemCompareAnsweres.Where(n => n.Research.ResearchId == research.ResearchId && n.User.UserId == user.UserId);
                List<Tuple<string, string>> list = new List<Tuple<string, string>>(); 

                foreach (var item in result_tmp)
                {
                    //list.Add(new Tuple<string, string>(item.ItemGood.Category.ItemCategoryName, item.ItemBad.Category.ItemCategoryName));
                    //var g = item.ItemGood.Tag.Split(',');
                    //var b = item.ItemBad.Tag.Split(',');

                    var atribute = from good in item.ItemGood.ItemAttribute
                                   from bad in item.ItemBad.ItemAttribute
                                   where good.AttributeName == bad.AttributeName && good.Value != bad.Value
                                   select new { good = good.Value, bad = bad.Value };
                    foreach (var pair in atribute)
                    {
                        list.Add(new Tuple<string, string>(pair.good, pair.bad));
                    }

                    //for (int i = 0; i < Math.Min(g.Length,b.Length); i++)
                    //{
                    //    if(g[i] != b[i]) list.Add(new Tuple<string, string>(g[i], b[i]));
                    //}
                }

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
                        list2.Add(new Tuple<string, string, double>(tmp.First().Item1, tmp.First().Item2,  (double)tmp.First().Item3 / (double)(tmp.First().Item3)));
                    }
                }
                return list2.OrderByDescending(n=>n.Item3);
            }

            return new List<Tuple<string, string, double>>();

        }

        public int GetAnswerUserCount(Research research)
        {
            return this.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId).Count();
        }
    }

    public static class Tool
    {
        public static Nullable<int> GetUserId(string userStr, string ip)
        {
            int id;
            if (int.TryParse(userStr, out id))
            {
                return id;
            }
            return null;
        }

        public static string ChangeUserId(int user_id, string ip)
        {
            return user_id.ToString();
        }

        public static string GetSortText(string t1, string t2)
        {
            if (t1.CompareTo(t2) > 0)
            {
                return t1 + "," + t2;
            }
            else
            {
                return t2 + "," + t1;
            }

        }

        public static IEnumerable<Tuple<IEnumerable<string>, double>> ConverMuitiRelation(IEnumerable<Tuple<string, string, double>> source,double minValue)
        {
            var data = source.Where(n => n.Item3 >= minValue);
            var data2 = data.GroupBy(n => n.Item1).Where(n => n.Count() > 1);


            List<Tuple<List<string>, double>> list = new List<Tuple<List<string>, double>>();
            foreach (var item in data2)
            {
                foreach (var item2 in item)
                {
                    foreach (var item3 in item)
                    {
                        if (item2.Item2 != item3.Item2)
                        {
                            var d = data.Where(n => n.Item1 == item2.Item2 && n.Item2 == item3.Item2).FirstOrDefault();
                            if(d !=null) list.Add(new Tuple< List<string>,double>(new List<string>(){item.Key,item2.Item2,item3.Item2},item2.Item3*item3.Item3*d.Item3));
                            
                        }
                    }
                }
            }
            return null;
        }

    }

    public class CustomSeedInitializer : DropCreateDatabaseAlways<GoocaBoocaDataBase>
    {
        // Seedメソッドをオーバーライド
        protected override void Seed(GoocaBoocaDataBase context)
        {
            // 基底クラスのSeedメソッド呼び出し
            base.Seed(context);

            context.ItemAnswerChoice.Add(new ItemAnswerChoice() { AnswerString = "いいね！" });
            context.ItemAnswerChoice.Add(new ItemAnswerChoice() { AnswerString = "よくない" });

            //return;

            //var user = new User() {  Age = 17, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
            //var research = new Research() { ResearchIdName = "test", Description = "testtesttest", ResearchName = "testResearchName", Reg_Date = DateTime.Now, Upd_Date = DateTime.Now, CreateUser = user, AnswerCount = 20, ResearchType = ResearchType.GoocaBooca.ToString() };

            //context.Users.Add(user);
            //context.Researches.Add(research);



            //var question1 = new Question(){ QuestionName = "q1", QuestionOrder = 1, QuestionText ="q1", QuestionType = GoocaBoocaDataModels.QuestionType.FreeText.ToString(), Research = research, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now};
            //var question2 = new Question() { QuestionName = "q2", QuestionOrder = 1, QuestionText = "q2", QuestionType = GoocaBoocaDataModels.QuestionType.Choice.ToString(), Research = research, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
            //context.Questiones.Add(question1);
            //context.Questiones.Add(question2);

            //for (int i = 0; i < 3; i++)
            //{
            //    var q = new QuestionChoice() { QuestionChoiceText = "回答" + i.ToString(), Question = question2, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
            //    context.QuestionChoices.Add(q);
            //}

            ////画像作成

            //var font = new Font("MS UI Gothic", 16);
            //var brash = new SolidBrush(Color.Black);
            //var brash2 = new SolidBrush(Color.White);
            //for (int i = 0; i < 5; i++)
            //{
            //    var itemcategory = new ItemCategory() { ItemCategoryName = "Category" + i.ToString(), Research = research, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
            //    context.ItemCategories.Add(itemcategory);
            //    for (int l = 0; l < 10; l++)
            //    {
            //        using (Bitmap img = new Bitmap(200, 400))
            //        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            //        {
            //            //ImageオブジェクトのGraphicsオブジェクトを作成する
            //            Graphics g = Graphics.FromImage(img);
            //            g.FillRectangle(brash2, 0, 0, 200, 400);
            //            g.DrawString("Category" + i.ToString() + "-" + l.ToString(), font, brash, new PointF(5, 5));
            //            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //            var array = ms.ToArray();
            //            var itemdata = new Item() { Category = itemcategory, ImageData = array, ImageType = ImageType.jpeg.ToString(), ItemName = "Category" + i.ToString() + "-" + l.ToString(), Reg_Date = DateTime.Now, Upd_Date = DateTime.Now, Resarch = research };
            //            context.Items.Add(itemdata);
            //        }
            //    }
            //}
            //font.Dispose();
            //brash.Dispose();
            //brash2.Dispose();


            // 変更をデータベースに反映
            int recordsAffected = context.SaveChanges();
        }
    }
}