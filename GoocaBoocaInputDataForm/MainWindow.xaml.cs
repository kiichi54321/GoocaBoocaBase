using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GoocaBoocaDataModels;
using System.Data.Entity;

namespace GoocaBoocaInputDataForm
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (databaseDrop)
            {
                Database.SetInitializer(new DropCreateDatabaseAlways<GoocaBoocaDataModels.GoocaBoocaDataBase>());
            }
            else
            {
                Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GoocaBoocaDataModels.GoocaBoocaDataBase>());
            }
            //     Database.SetInitializer(new GoocaBoocaDataModels.CustomSeedInitializer());
         //  
            //  
        }

        bool databaseDrop = false;

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                this.textBox1.Text = ofd.FileName;
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                this.textBox2.Text = ofd.FileName;
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                this.textBox3.Text = ofd.FileName;
            }
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            if (ofd.ShowDialog() == true)
            {
                this.textBox4.Text = ofd.FileName;
            }
        }


        private void cutomo(GoocaBoocaDataBase db)
        {
            var research = db.Researches.Where(n => n.ResearchIdName == "cutomo2012").FirstOrDefault();
            if (research == null)
            {
                research = new Research()
                {
                    AnswerCount = 40,
                    ResearchIdName = "cutomo2012",
                    Description = "キュートモ！[キュートモ]はｹｰﾀｲを使ったﾘｻｰﾁｻｲﾄです。表示される女性を見て、 「仲良くなりたい！」と思ったら、「なりたい!」を、なりたくないと思えば「別に…」を選択してください。",
                    ResearchType = ResearchType.GoocaBooca.ToString(),
                    Hidden = true,
                    ResearchName = "キュートモ",
                    QuestionText = "「仲良くなりたい！」と思ったら、「なりたい!」を、なりたくないと思えば「別に…」",
                    Reg_Date = DateTime.Now,
                    Upd_Date = DateTime.Now
                };
                db.Researches.Add(research);
            }


            db.ItemAnswerChoice.Add(new ItemAnswerChoice() { AnswerString = "仲良くなりたい！", Research = research });
            db.ItemAnswerChoice.Add(new ItemAnswerChoice() { AnswerString = "別に・・・", Research = research });

            Dictionary<string, ItemCategory> categoryDic = new Dictionary<string, ItemCategory>();
            var basefolder = System.IO.Path.GetDirectoryName(this.textBox1.Text);
            foreach (var item in System.IO.File.ReadLines(this.textBox1.Text).Skip(1))
            {
                //brand_id	brand_name	category_id	image_file_name	concept
                var data = item.Split('\t');
                var brand_id = data[0];
                var brand_name = data[1];
                var category_id = data[2];
                var image_file_name = data[3];
                if (categoryDic.ContainsKey(brand_name) == false)
                {
                    var c = new ItemCategory() { ItemCategoryName = brand_name, Research = research, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
                    categoryDic.Add(data[1], c);
                    db.ItemCategories.Add(c);
                }
                var category = categoryDic[brand_name];

                var itemdata = db.Items.Where(n => n.Resarch.ResearchId == research.ResearchId && n.ItemName == image_file_name).FirstOrDefault();
                if (itemdata == null)
                {
                    itemdata = new Item()
                    {
                        Category = category,
                        ItemName = data[3],
                        Resarch = research,
                        Reg_Date = DateTime.Now,
                        Upd_Date = DateTime.Now
                    };
                }
                var image = System.IO.File.ReadAllBytes(basefolder + "/" + data[1] + "/" + data[3]);
                itemdata.ImageType = data[3].Split('.').Last();
                itemdata.ImageData = image;
                db.Items.Add(itemdata);
            }

            Question question = null;
            foreach (var item in System.IO.File.ReadLines(this.textBox2.Text))
            {
                if (item.First() == '\t')
                {
                    db.QuestionChoices.Add(new QuestionChoice() { QuestionChoiceText = item.Replace("\t", ""), Question = question, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now });
                }
                else
                {
                    question = new Question() { QuestionName = item, QuestionType = QuestionType.Choice.ToString(), QuestionText = item, Research = research, QuestionOrder = 1, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
                    db.Questiones.Add(question);
                }
            }
            db.SaveChanges();

        }

        void Keion(GoocaBoocaDataBase db)
        {
            var research = db.Researches.Where(n => n.ResearchIdName == "kawaero2012").FirstOrDefault();
            if (research == null)
            {
                research = new Research()
                {
                    AnswerCount = 30,
                    ResearchIdName = "kawaero2012",
                    Description = "かわいいけど、どっちがエロい？",
                    ResearchType = ResearchType.Compare.ToString(),
                    Hidden = true,
                    ResearchName = "かわいいけど、エロい？",
                    QuestionText = "かわいいけど、どっちがエロい？",
                    Reg_Date = DateTime.Now,
                    Upd_Date = DateTime.Now
                };
                db.Researches.Add(research);
            }



            Dictionary<string, ItemCategory> categoryDic = new Dictionary<string, ItemCategory>();
            var basefolder = System.IO.Path.GetDirectoryName(this.textBox3.Text);
            foreach (var item in System.IO.File.ReadLines(this.textBox3.Text))
            {
                //brand_id	brand_name	category_id	image_file_name	concept
                var data = item.Split('\t');
                var category_id = data[0];
                var file_name = data[1];
                var tag = data[2];
                var image_file_name = data[1];
                if (categoryDic.ContainsKey(category_id) == false)
                {
                    var c = new ItemCategory() { ItemCategoryName = category_id, Research = research, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
                    categoryDic.Add(category_id, c);
                    db.ItemCategories.Add(c);
                }
                var category = categoryDic[category_id];

                var itemdata = db.Items.Where(n => n.Resarch.ResearchId == research.ResearchId && n.ItemName == image_file_name).FirstOrDefault();
                if (itemdata == null)
                {
                    itemdata = new Item()
                    {
                        Category = category,
                        ItemName = file_name,
                        Resarch = research,
                        Reg_Date = DateTime.Now,
                        Upd_Date = DateTime.Now,
                        Tag = tag
                    };
                }
                var image = System.IO.File.ReadAllBytes(basefolder + "/" + "keion_resize" + "/" + image_file_name);
                itemdata.ImageType = image_file_name.Split('.').Last();
                itemdata.ImageData = image;
                db.Items.Add(itemdata);
            }

            Question question = null;
            foreach (var item in System.IO.File.ReadLines(this.textBox4.Text))
            {
                if (item.First() == '\t')
                {
                    db.QuestionChoices.Add(new QuestionChoice() { QuestionChoiceText = item.Replace("\t", ""), Question = question, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now });
                }
                else
                {
                    question = new Question() { QuestionName = item, QuestionType = QuestionType.Choice.ToString(), QuestionText = item, Research = research, QuestionOrder = 1, Reg_Date = DateTime.Now, Upd_Date = DateTime.Now };
                    db.Questiones.Add(question);
                }
            }
            db.SaveChanges();
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //     Database.SetInitializer(new GoocaBoocaDataModels.CustomSeedInitializer());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GoocaBoocaDataModels.GoocaBoocaDataBase>());
            //  Database.SetInitializer(new DropCreateDatabaseAlways<GoocaBoocaDataModels.GoocaBoocaDataBase>());
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            cutomo(db);
            Keion(db);
            MessageBox.Show("end");
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var data = db.Researches.Where(n => n.ResearchIdName == "kawaero2012").FirstOrDefault();
            if (data != null)
            {
             //   data.ResearchType = ResearchType.Compare.ToString();
                data.AnswerCount = 30;
                db.Researches.Add(data);
                db.SaveChanges();
            }
            MessageBox.Show("end");

        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
            var list = db.Researches.ToArray();
        }


    }
}
