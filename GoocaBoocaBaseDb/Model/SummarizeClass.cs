using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GoocaBoocaDataModel.Model
{
    public class SummarizeCompletedUser
    {
        public DateTime DateTime { get; set; }
        public int SumCount { get; set; }
        public IEnumerable<int> CountList { get; set; }
    }
}
