using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoocaBoocaDataModels
{
    public class ItemAnswer
    {
        public int ItemAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Research Research { get; set; }
        public virtual ItemAnswerChoice ItemAnswerChoice { get; set; }
        public virtual Item Item { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
        public int Order { get; set; }
        public int Group { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public string Tag { get; set; }
    }

    public class ItemCompareAnswer
    {
        public int ItemCompareAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Research Research { get; set; }
        public virtual Item ItemGood { get; set; }
        public virtual Item ItemBad { get; set; }
        public string PairKey { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public string Tag { get; set; }

    }


    public class ItemAnswerChoice
    {
        public int ItemAnswerChoiceId { get; set; }
        public virtual Research Research { get; set; }
        public string AnswerString { get; set; }
        public virtual Image Image { get; set; }
        public string Tag { get; set; }
    }


}