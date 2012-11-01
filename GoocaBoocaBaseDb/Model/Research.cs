using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GoocaBoocaDataModels
{
    public class Research
    {
        public int ResearchId { get; set; }
        [StringLength(20)]
        public string ResearchName { get; set; }
        [StringLength(20)]
        public string ResearchIdName { get; set; }
        public virtual User CreateUser { get; set; }
        public string Description { get; set; }
        public string QuestionText { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public int AnswerCount { get; set; }
        public bool Hidden { get; set; }
        [StringLength(20)]
        public string ResearchType { get; set; }
        public virtual ICollection<ItemCategory> ItemCategory { get; set; }
        public virtual ICollection<Question> Questiones { get; set; }
        public virtual ICollection<ItemAnswerChoice> ItemAnswerChoice { get; set; }
        public virtual Image ResearchMainImage { get; set; }
        public string Tag { get; set; }
    }

    public enum ResearchType
    {
        GoocaBooca,Order,Compare,None
    }



}