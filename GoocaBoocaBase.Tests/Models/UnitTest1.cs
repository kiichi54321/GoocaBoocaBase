using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;

namespace GoocaBoocaBase.Tests.Models
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            //var result = db.GoocaBoocaResult("cutomo2012", "20", "");

            var research = db.GetResearch("cutomo2012");
            var user = db.GetUser("20", "");
            var result_tmp = db.ItemAnsweres.Where(n => n.Research.ResearchId == research.ResearchId && n.User.UserId == user.UserId).GroupBy(n => new { n.ItemCategory, n.ItemAnswerChoice }).Select(n => new { ItemCategory = n.Key.ItemCategory, ItemAnswerChoice = n.Key.ItemAnswerChoice, Count = n.Count() }).OrderBy(n => n.ItemCategory.ItemCategoryId).ThenBy(n => n.ItemAnswerChoice.ItemAnswerChoiceId);

           var r =  result_tmp.ToArray();
            var answerPattern = from choice in db.ItemAnswerChoice.Where(n => n.Research.ResearchId == research.ResearchId)
                                from category in db.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId)
                                select new { Category = category, Choice = choice };

           var r1 =  answerPattern.ToArray();

            var result2 = from ap in answerPattern
                         join userAnswer in result_tmp
                            on new { ap.Category.ItemCategoryId, ap.Choice.ItemAnswerChoiceId } equals new { userAnswer.ItemCategory.ItemCategoryId, userAnswer.ItemAnswerChoice.ItemAnswerChoiceId } into z
                         from a in z.DefaultIfEmpty()
                         select new { ap.Category, ap.Choice, a.Count };
           var r3 = result2.ToArray();
        }
    }
}
