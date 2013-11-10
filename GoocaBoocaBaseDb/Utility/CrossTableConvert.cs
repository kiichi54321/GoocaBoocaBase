using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoocaBoocaDataModels.Utility
{
    public static class CrossTableConvert
    {
        public static string CreateQuestionData(string researchIdName)
        {
            StringBuilder sb = new StringBuilder();
            GoocaBoocaDataModels.GoocaBoocaDataBase gb = new GoocaBoocaDataBase();
            var research = gb.GetResearch(researchIdName);

            sb.AppendLine("Key\tText\tType\tAnswers");
            List<QuestionLine> list = new List<QuestionLine>();
            foreach (var item in research.Questiones.Where(n => n.IsActive == true))
            {
                QuestionLine ql = new QuestionLine()
                {
                    Key = "q_" + item.QuestionId.ToString(),
                    Text = item.QuestionText,
                    Type = "順序"
                };
                if (item.QuestionType == "FreeText")
                {
                    ql.Type = "文字列";
                }
                foreach (var questionChoices in item.QuestionChoices.Where(n => n.IsActive == true))
                {
                    ql.AddAnswer(questionChoices.QuestionChoiceId.ToString(), questionChoices.QuestionChoiceText);
                }
                list.Add(ql);
            }
            foreach (var item in research.ItemCategory.Where(n => n.IsActive == true))
            {
                QuestionLine ql = new QuestionLine()
                {
                    Key = "c_" + item.ItemCategoryId.ToString(),
                    Text = item.ItemCategoryName,
                    Type = "数値"
                };
                int cc = (research.AnswerCount / research.ItemCategory.Where(n => n.IsActive).Count());
                for (int i = 0; i < cc + 1; i++)
                {
                    ql.AddAnswer(i.ToString(), i.ToString());
                }
                list.Add(ql);
            }

            foreach (var itemCategory in research.ItemCategory.Where(n => n.IsActive))
            {
                foreach (var item in itemCategory.Items)
                {
                    QuestionLine ql = new QuestionLine()
                    {
                        Key = "i_" + item.ItemId.ToString(),
                        Text = item.Category.ItemCategoryName + ":" + item.ItemName+"{"+research.Tag+ item.ItemName +"}",
                        Type = "数値"
                    };
                    list.Add(ql);
                    //foreach (var itemAnswerChoice in research.ItemAnswerChoice.Where(n => n.IsActive))
                    //{
                    //    ql.AddAnswer(itemAnswerChoice.ItemAnswerChoiceId.ToString(), itemAnswerChoice.AnswerString);
                    //}
                }
            }
            Dictionary<string, List<string>> attribuuteDic = new Dictionary<string, List<string>>();
            foreach (var item in gb.ItemAttributes.Where(n => n.Item.Resarch.ResearchId == research.ResearchId && n.IsActive == true))
            {
                if (attribuuteDic.ContainsKey(item.AttributeName))
                {
                    attribuuteDic[item.AttributeName].Add(item.Value);
                }
                else
                {
                    attribuuteDic.Add(item.AttributeName, new List<string>() { item.Value });
                }
            }
            foreach (var item in attribuuteDic)
            {

                foreach (var item2 in item.Value.Distinct())
                {
                    QuestionLine ql = new QuestionLine()
                    {
                        Key = "attribute_" + item.Key + "_" + item2,
                        Text = "属性:" + item.Key + "_" + item2,
                        Type = "数値"
                    };
                    list.Add(ql);
                }

            }
            foreach (var item in list)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }

        public static string CreateAnswerData(string researchIdName, int skip)
        {
            StringBuilder sb = new StringBuilder();
            GoocaBoocaDataModels.GoocaBoocaDataBase gb = new GoocaBoocaDataBase();
            var research = gb.GetResearch(researchIdName);
            TsvBuilder tsvBuilder = new TsvBuilder();

            foreach (var user in gb.UserAnswerCompleted.Where(n => n.Research.ResearchId == research.ResearchId).OrderBy(n => n.UserAnswerCompletedId).Select(n => n.User).Skip(skip))
            {
                foreach (var questionAnsweres in user.QuestionAnswer)
                {
                    tsvBuilder.Add("q_" + questionAnsweres.Question.QuestionId, questionAnsweres.QuestionChoice.QuestionChoiceId);
                }
                foreach (var item in user.FreeAnswer)
                {
                    tsvBuilder.Add("q_" + item.Question.QuestionId, item.FreeTest);
                }
                foreach (var item in user.ItemAnswer.GroupBy(n => n.Item.Category).Select(n => new { n.Key, Count = n.Where(m => m.ItemAnswerChoice.Tag == "Key").Count() }))
                {
                    tsvBuilder.Add("c_" + item.Key.ItemCategoryId, item.Count);
                }
                foreach (var item in user.ItemAnswer)
                {
                    if (item.ItemAnswerChoice.Tag == "Key")
                    {
                        tsvBuilder.Add("i_" + item.Item.ItemId, 1);
                    }
                }
                Dictionary<string, int> countDic = new Dictionary<string, int>();
                foreach (var item in user.ItemAnswer.Where(n => n.ItemAnswerChoice.Tag == "Key"))
                {
                    foreach (var attribute in item.Item.ItemAttribute)
                    {
                        var key = attribute.AttributeName + "_" + attribute.Value;
                        if (countDic.ContainsKey(key))
                        {
                            countDic[key]++;
                        }
                        else
                        {
                            countDic.Add(key, 1);
                        }
                    }
                }
                foreach (var item in countDic)
                {
                    tsvBuilder.Add("attribute_" + item.Key, item.Value);
                }
                tsvBuilder.NextLine();
            }
            return tsvBuilder.ToString();
        }

        public class QuestionLine
        {
            public string Key { get; set; }
            public string Text { get; set; }
            public string Type { get; set; }
            public Dictionary<string, string> AnswerDic { get; set; }

            public void AddAnswer(string key, string val)
            {
                if (AnswerDic == null) AnswerDic = new Dictionary<string, string>();

                if (AnswerDic.ContainsKey(key))
                {
                    AnswerDic[key] = val;
                }
                else
                {
                    AnswerDic.Add(key, val);
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Key + "\t" + Text + "\t" + Type + "\t");
                if (AnswerDic != null)
                {
                    foreach (var item in AnswerDic)
                    {
                        sb.Append(item.Key + ":" + item.Value + ",");
                    }
                }
                return sb.ToString();
            }
        }

        public class TsvBuilder
        {
            List<TsvLine> line = new List<TsvLine>();
            System.Collections.Generic.HashSet<string> columList = new HashSet<string>();

            TsvLine current = new TsvLine();

            public void NextLine()
            {
                line.Add(current);
                current = new TsvLine();
            }

            public void Add(string key, string val)
            {
                current.Add(key, val.Replace("\n", " ").Replace("\r", "").Replace("\t", " "));
                columList.Add(key);
            }

            public void Add(string key, int val)
            {
                Add(key, val.ToString());
            }

            public class TsvLine
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                public Dictionary<string, string> Dic
                {
                    get { return dic; }
                    set { dic = value; }
                }

                public void Add(string key, string value)
                {
                    if (dic.ContainsKey(key))
                    {
                        dic[key] = value;
                    }
                    else
                    {
                        dic.Add(key, value);
                    }

                }

                public string GetValue(string key)
                {
                    if (dic.ContainsKey(key))
                    {
                        return dic[key];
                    }
                    else
                    {
                        return "0";
                    }
                }
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in columList)
                {
                    sb.Append(item).Append("\t");
                }
                sb.AppendLine();
                foreach (var item in line)
                {
                    foreach (var col in columList)
                    {
                        sb.Append(item.GetValue(col)).Append("\t");
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }

        }


    }
}
