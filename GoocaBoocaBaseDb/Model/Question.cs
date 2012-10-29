using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoocaBoocaDataModels
{
    public enum QuestionType : byte
    {
        FreeText = 1, Choice
    }
    public class Question
    {
        public int QuestionId { get; set; }
        [StringLength(200)]
        public string QuestionName { get; set; }
        public string QuestionText { get; set; }
        public int QuestionOrder { get; set; }
        public virtual Research Research { get; set; }
        [StringLength(200)]       
        public string QuestionType { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public virtual ICollection<QuestionChoice> QuestionChoices { get; set; }
        public virtual Image Image { get; set; }
    }

    public class QuestionChoice
    {
        public int QuestionChoiceId { get; set; }
        [StringLength(200)]
        public string QuestionChoiceText { get; set; }
        public virtual Question Question { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public virtual Image Image { get; set; }
    }

    public class QuestionAnswer
    {
        public int QuestionAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Research Research { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionChoice QuestionChoice { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
    }

    public class QuestionCollection : List<Question>
    {

    }

    //public class QuestionType
    //{
    //    public int QuestionTypeId { get; set; }
    //    public string Name { get; set; }
    //}



    public class FreeAnswer
    {
        public int FreeAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Question Question { get; set; }
        public string FreeTest { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
    }
}