using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GoocaBoocaDataModels;

namespace GoocaBoocaBase.Models
{
    public class Analyze
    {

    }

    public class AnalyzeIndexModel
    {
        public IEnumerable<Research> Researches { get; set; }

    }

    public class AnalyzeCompareModel
    {
        public int AllCount { get; set; }
        public IEnumerable<GoocaBoocaDataModels.Model.CompareAnalyzeData> Result { get; set; }
        public IEnumerable<Tuple<GoocaBoocaDataModels.QuestionChoice, int>> ALLQuestionResult { get; set; }
    }
}