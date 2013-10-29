using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace GoocaBoocaInputDataForm
{
    public class XMLInport
    {
        public static void AllDelete()
        {
            
        }
        
        public static void Inport(string fileName, GoocaBoocaDataModels.GoocaBoocaDataBase db)
        {
            var xml = XElement.Load(fileName);
            var ns = xml.GetDefaultNamespace();
            var researchXML = xml.DescendantsAndSelf("Research").First();
            if (researchXML != null)
            {
                var researchBase = researchXML.Element(ns + "Base");
                var researchIdName = researchBase.Elements(ns + "ResearchIdName").First().Value;
                var research = db.Researches.Where(n => n.ResearchIdName == researchIdName).DefaultIfEmpty(null).FirstOrDefault();

                if (research == null)
                {
                    research = new GoocaBoocaDataModels.Research();
                    db.Researches.Add(research);
                }

                research.AnswerCount = int.Parse(researchBase.Element(ns + "AnswerCount").Value);
                research.ResearchName = researchBase.Element(ns + "ResearchName").Value;
                research.QuestionText = researchBase.Element(ns + "QuestionText").Value;
                research.ResearchIdName = researchBase.Element(ns + "ResearchIdName").Value;
                research.ExtendAnlyzeResultUrl = researchBase.Element(ns + "ExtendAnlyzeResultUrl").Value;
                research.Description = researchBase.Element(ns + "Description").Value;
                research.ResearchType = researchBase.Element(ns + "ResearchType").Value;
                research.Tag = researchBase.Element(ns + "ImageBaseUrl").Value;
                var mainImage = new GoocaBoocaDataModels.Image() { ImageName = researchBase.Element("ResearchMainImage").Value };
                mainImage.SetDate();
                research.ResearchMainImage = mainImage;
                research.SetDate();
                if (researchBase.Element(ns + "Hidden").Value == "True")
                {
                    research.Hidden = true;
                }
                else
                {
                    research.Hidden = false;
                }

                if (research.ItemAnswerChoice != null && research.ItemAnswerChoice.Any())
                {
                    var itemAnswerChoice = researchXML.Element(ns + "ItemAnswerChoiceList");
                    for (int i = 0; i < Math.Max(itemAnswerChoice.Elements(ns + "AnswerString").Count(), research.ItemAnswerChoice.Count); i++)
                    {
                        var DBitemAnswerChoice = research.ItemAnswerChoice.OrderBy(n => n.ItemAnswerChoiceId).ElementAtOrDefault(i);
                        var XMLitemAnswerChoice = itemAnswerChoice.Elements(ns + "AnswerString").ElementAtOrDefault(i);
                        var tagAnswerChoice = XMLitemAnswerChoice.Attribute("Tag") != null ? XMLitemAnswerChoice.Attribute("Tag").Value : null;
                        if (DBitemAnswerChoice != null && XMLitemAnswerChoice != null)
                        {
                            DBitemAnswerChoice.AnswerString = XMLitemAnswerChoice.Value;
                            DBitemAnswerChoice.IsActive = true;
                            DBitemAnswerChoice.Tag = tagAnswerChoice;
                        }
                        if (DBitemAnswerChoice == null && XMLitemAnswerChoice != null)
                        {
                            var choice = new GoocaBoocaDataModels.ItemAnswerChoice()
                            {
                                AnswerString = XMLitemAnswerChoice.Value,
                                Research = research,
                                Tag = tagAnswerChoice
                            };
                            choice.SetDate();
                            research.ItemAnswerChoice.Add(choice);
                        }
                        if (DBitemAnswerChoice != null && XMLitemAnswerChoice == null)
                        {
                            DBitemAnswerChoice.IsActive = false;
                        }
                        DBitemAnswerChoice.Upd_Date = DateTime.Now;
                    }
                }
                else
                {
                    foreach (var item in researchXML.Element(ns + "ItemAnswerChoiceList").Elements(ns + "AnswerString"))
                    {
                        var tagAnswerChoice = item.Attribute("Tag") != null ? item.Attribute("Tag").Value : null;

                        GoocaBoocaDataModels.ItemAnswerChoice choice = new GoocaBoocaDataModels.ItemAnswerChoice()
                        {
                            AnswerString = item.Value,
                            Research = research,
                            Tag = tagAnswerChoice
                        };
                        choice.SetDate();
                        db.ItemAnswerChoice.Add(choice);
                    }
                }
                int qOrder = 1;
                foreach (var item in researchXML.Element(ns + "Questions").Elements(ns + "Question"))
                {
                    if (item.Element(ns + "QuestionName") != null)
                    {
                        GoocaBoocaDataModels.Question question = null;
                        if (research.Questiones != null)
                        {
                            question = research.Questiones.Where(n => n.QuestionName == item.Element(ns + "QuestionName").Value).DefaultIfEmpty(null).FirstOrDefault();
                        }
                        if (question == null)
                        {
                            question = new GoocaBoocaDataModels.Question() { Research = research, QuestionName = item.Element(ns + "QuestionName").Value };
                            question.SetDate();
                            db.Questiones.Add(question);
                        }
                        question.QuestionOrder = qOrder;
                        if (item.Element(ns + "QuestionText") != null)
                        {
                            question.QuestionText = item.Element(ns + "QuestionText").Value;
                        }
                        if (item.Element(ns + "QuestionType") != null)
                        {
                            question.QuestionType = item.Element(ns + "QuestionType").Value;
                        }
                        else
                        {
                            question.QuestionType = "Choice";
                        }
                        if (question.QuestionType != "FreeText")
                        {
                            List<string> list = new List<string>();
                            foreach (var questionChoices in item.Element(ns + "QuestionChoices").Elements(ns + "QuestionChoiceText"))
                            {
                                if (question.QuestionChoices == null || question.QuestionChoices.Where(n => n.QuestionChoiceText == questionChoices.Value).Any() == false)
                                {
                                    var qq = new GoocaBoocaDataModels.QuestionChoice() { Question = question, QuestionChoiceText = questionChoices.Value };
                                    qq.SetDate();
                                    db.QuestionChoices.Add(qq);
                                }
                                list.Add(questionChoices.Value);
                            }
                            foreach (var item2 in question.QuestionChoices)
                            {
                                if (list.Contains(item2.QuestionChoiceText) == false)
                                {
                                    item2.IsActive = false;
                                }
                            }
                        }
                    }

                }

                List<string> categoryList = new List<string>();
                foreach (var item in researchXML.DescendantsAndSelf(ns + "Category"))
                {
                    GoocaBoocaDataModels.ItemCategory category = null;
                    categoryList.Add(item.Attribute(ns + "CategoryName").Value);
                    if (research.ItemCategory != null)
                    {
                        category = research.ItemCategory.Where(n => n.ItemCategoryName == item.Attribute(ns + "CategoryName").Value).DefaultIfEmpty(null).FirstOrDefault();
                    }
                    if (category == null)
                    {
                        category = new GoocaBoocaDataModels.ItemCategory() { Research = research, ItemCategoryName = item.Attribute(ns + "CategoryName").Value };
                        category.SetDate();
                        db.ItemCategories.Add(category);
                    }
                    List<string> imageUrlList = new List<string>();
                    foreach (var item2 in item.Descendants(ns + "Item"))
                    {
                        var imageUrl = item2.Attribute(ns + "ImageUrl").Value;
                       
                        if (imageUrl != null)
                        {
                            imageUrlList.Add(imageUrl);
                            GoocaBoocaDataModels.Item itemNode = null;
                            if (category.Items != null)
                            {
                                itemNode = category.Items.Where(n => n.ItemName == imageUrl).DefaultIfEmpty(null).FirstOrDefault();
                                itemNode.IsActive = true;
                            }
                            if (itemNode == null)
                            {
                                itemNode = new GoocaBoocaDataModels.Item() { ItemName = imageUrl, Resarch = research, Category = category };
                                itemNode.SetDate();
                                db.Items.Add(itemNode);

                                //                                itemNode.ItemAttribute.Add(new GoocaBoocaDataModels.ItemAttribute(){ 
                            }
                            var itemAttributeText = item2.Attribute(ns + "ItemAttribute");
                            if (itemAttributeText != null)
                            {
                                var attribute = itemAttributeText.Value.Split(',');
                                List<string> itemAttributeList = new List<string>();
                                foreach (var item3 in attribute)
                                {
                                    var d = item3.Split(':');
                                    if (d.Length > 1)
                                    {
                                        if (itemNode.ItemAttribute != null && itemNode.ItemAttribute.Where(n => n.AttributeName == d[0]).Any())
                                        {
                                            itemNode.ItemAttribute.Where(n => n.AttributeName == d[0]).First().Value = d[1];
                                            itemNode.ItemAttribute.Where(n => n.AttributeName == d[0]).First().Upd_Date = DateTime.Now;
                                        }
                                        else
                                        {
                                            var itemattribute = new GoocaBoocaDataModels.ItemAttribute()
                                            {
                                                Item = itemNode,
                                                AttributeName = d[0],
                                                Value = d[1]
                                            };
                                            itemattribute.SetDate();
                                            db.ItemAttributes.Add(itemattribute);
                                        }
                                        itemAttributeList.Add(d[0]);

                                    }
                                }
                                if (itemNode.ItemAttribute != null)
                                {
                                    foreach (var attributeItem in itemNode.ItemAttribute)
                                    {
                                        if (itemAttributeList.Contains(attributeItem.AttributeName) == false)
                                        {
                                            attributeItem.IsActive = false;
                                        }
                                    }
                                }
                            }

                        }
                        foreach (var item3 in category.Items)
                        {
                            if (imageUrlList.Contains(item3.ItemName) == false)
                            {
                                item3.IsActive = false;
                            }
                        }
                    }



                }
                foreach (var item in research.ItemCategory)
                {
                    if (categoryList.Contains(item.ItemCategoryName)==false)
                    {
                        item.IsActive = false;
                        foreach (var item2 in item.Items)
                        {
                            item2.IsActive = false;
                        }
                    }
                }
            }



            db.SaveChanges();

        }
    }
}
