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
using GoocaBoocaDataModels.Model;

namespace GoocaBoocaWpfApplication
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            wm.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(wm_ProgressChanged);
            wm.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(wm_RunWorkerCompleted);
        }

        List<GoocaBoocaDataModels.Model.GoocaBoocaAnswerData> datalist = new List<GoocaBoocaDataModels.Model.GoocaBoocaAnswerData>();
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            Run();

        }
        List<GoocaBoocaDataModels.ItemCategory> categoryList = new List<GoocaBoocaDataModels.ItemCategory>();
        public void LoadData()
        {
            using (GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase())
            {
                var research = db.GetResearch("cutomo2012");
                GoocaBoocaDataModels.Model.SummarizeData sd = new GoocaBoocaDataModels.Model.SummarizeData(db);
                datalist = new List<GoocaBoocaDataModels.Model.GoocaBoocaAnswerData>(sd.CreateGoocaBoocaAnswerData("cutomo2012"));
                categoryList = db.ItemCategories.Where(n => n.Research.ResearchId == research.ResearchId).OrderBy(n => n.ItemCategoryId).ToList();
            }
        }

        MyLib.Analyze.WardMethod wm = new MyLib.Analyze.WardMethod();



        public void Run()
        {

            wm.DataClear();
            foreach (var item in datalist)
            {
                if (comboBox1.SelectedItem.ToString().Contains("男性"))
                {
                    if (item.Question !=null && item.Question.Where(n => n.Item2 == "男性").Any())
                    {
                        wm.AddData(new MyLib.Analyze.WardMethod.Data() { Value = item.Data.ToArray(), OriginValue = item.Data.ToArray(), Tag = item });
                    }
                }
                if (comboBox1.SelectedItem.ToString().Contains("女性") && item.Question != null && item.Question.Where(n => n.Item2 == "女性").Any())
                {
                    wm.AddData(new MyLib.Analyze.WardMethod.Data() { Value = item.Data.ToArray(), OriginValue = item.Data.ToArray(), Tag = item });
                }
                if (comboBox1.SelectedItem.ToString().Contains( "すべて"))
                {
                    wm.AddData(new MyLib.Analyze.WardMethod.Data() { Value = item.Data.ToArray(), OriginValue = item.Data.ToArray(), Tag = item });
                }
            }
            wm.Run();
        }

        void wm_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        void wm_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            CreateViewData(intUpDown.Value.Value);
        }

        public void CreateViewData(int cNum)
        {
            var cList = wm.GetCluster(cNum);
            List<ClasterData> list = new List<ClasterData>();
            foreach (var item in cList)
            {
                
                list.Add(new ClasterData()
                {
                    Data = MyLib.Analyze.WardMethod.Data.GetOriginCenter(item.Datas),
                    FreeAnswer = item.Datas.Select(n => n.Tag).OfType<GoocaBoocaAnswerData>().Select(n => (n.FreeAnswer == null) ? string.Empty : n.FreeAnswer.First().Item2).Where(n => n.Length > 0).ToArray(),
                    DataStr = item.CenterData.Value.Select(n => n.ToString()).Aggregate((m, n) => m + "," + n),
                    AnswerData = item.Datas.Select(n => n.Tag).OfType<GoocaBoocaAnswerData>()
                });
            }
            itemsControl.ItemsSource = list;

            webBrowser1.NavigateToString(CreateHTML(list));
        }

        public string CreateHTML(List<ClasterData> list)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html><body>");

            var r = datalist.Where(n => n.Question != null).Select(n => n.Question);
            List<string> qqList = new List<string>();
            foreach (var item2 in r)
            {
                foreach (var item3 in item2)
                {
                    qqList.Add(item3.Item1 + "</th><th>" + item3.Item2);
                }
            }
            sb.Append("<h2>全体</h2><table>");
            foreach (var item in qqList.GroupBy(n=>n).Select(n=>new { Key = n.Key,Count= n.Count()}).OrderByDescending(n=>n.Count))
            {
                sb.Append("<tr><th>" + item.Key + "</th><td>" + item.Count + "</td></tr>");                
            }
            sb.Append("</table>");

            int i = 1;
            foreach (var item in list)
            {
                sb.Append("<hr><h2>クラスター" + i.ToString() + "(" + item.AnswerData.Count().ToString() + ")</h2>");
                sb.Append("<table>");
                for (int k = 0; k < item.Data.Length; k++)
                {
                    sb.Append("<tr><th>" + categoryList[k].ItemCategoryName + "</th><td>" + (item.Data[k] * 100).ToString("F2") + "%</td></tr>");
                }
                sb.Append("</table>");
                sb.Append("<ul>");

                List<string> qList = new List<string>();
                foreach (var item2 in item.AnswerData)
                {
                    if (item2.Question != null)
                    {
                        foreach (var item3 in item2.Question)
                        {
                            qList.Add(item3.Item1 + "</th><th>" + item3.Item2);
                        }
                    }
                }

                sb.Append("<table>");
                foreach (var item2 in qList.GroupBy(n => n).Select(n => new { Key = n.Key, Count = n.Count() }).OrderByDescending(n => n.Count))
                {
                    sb.Append("<tr><th>" + item2.Key + "</th><td>" + item2.Count + "</td></tr>");
                }
                sb.Append("</table>");

                foreach (var item2 in item.AnswerData)
                {
                    if (item2.FreeAnswer != null)
                    {
                        sb.Append("<li>" + item2.FreeAnswer.FirstOrDefault().Item2);
                        sb.Append("<small>");
                        foreach (var item3 in item2.Question)
                        {
                            sb.Append("[" + item3.Item2 + "]");
                        }
                        sb.Append("</small>");
                        sb.Append("</li>");
                    }
                }
                sb.Append("</ul>");
                i++;
            }

            return sb.ToString();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CreateViewData(intUpDown.Value.Value);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Run();

        }
    }


    public class ClasterData
    {
        public double[] Data { get; set; }
        public string[] FreeAnswer { get; set; }
        public string DataStr { get; set; }
        public IEnumerable<GoocaBoocaAnswerData> AnswerData { get; set; }
    }
}
