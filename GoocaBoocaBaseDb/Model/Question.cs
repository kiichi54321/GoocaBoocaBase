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
    public class Question:BaseModel
    {
        public int QuestionId { get; set; }
        [StringLength(200)]
        public string QuestionName { get; set; }
        public string QuestionText { get; set; }
        public int QuestionOrder { get; set; }
        public virtual Research Research { get; set; }
        [StringLength(200)]       
        public string QuestionType { get; set; }
        public virtual ICollection<QuestionChoice> QuestionChoices { get; set; }
        public virtual Image Image { get; set; }
        public ICollection<QuestionAttribute> QuestionAttributes { get; set; }
    }

    public class QuestionChoice:BaseModel
    {
        public int QuestionChoiceId { get; set; }
        [StringLength(200)]
        public string QuestionChoiceText { get; set; }
        public virtual Question Question { get; set; }
        public virtual Image Image { get; set; }
    }

    public class QuestionAnswer:BaseModel
    {
        public int QuestionAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Research Research { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionChoice QuestionChoice { get; set; }
    }

    public class FreeAnswer:BaseModel
    {
        public int FreeAnswerId { get; set; }
        public virtual User User { get; set; }
        public virtual Question Question { get; set; }
        public string FreeTest { get; set; }
    }

    public class QuestionAttribute : BaseModel
    {
        public int QuestionAttributeId { get; set; }
        public virtual Question Question { get; set; }
        public string AttributeCategory { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}