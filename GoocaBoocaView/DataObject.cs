using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace GoocaBoocaView
{
    public class DataObject
    {

    }

    public class GoocaBoocaAnswerData
    {
        public int UserId { get; set; }
        public double[] Data { get; set; }
        public List<Tuple<string, string>> Question { get; set; }
        public List<Tuple<string, string>> FreeAnswer { get; set; }
    }
}
