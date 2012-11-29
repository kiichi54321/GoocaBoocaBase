using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Json;

namespace GoocaBoocaView
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
            var uri = System.Windows.Browser.HtmlPage.Document.DocumentUri;
        }

        public string ResearchId { get; set; }

        public string DataUrl { get; set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
           
  
 


        }


    }
}
