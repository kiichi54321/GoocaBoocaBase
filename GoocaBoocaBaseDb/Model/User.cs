using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoocaBoocaDataModels
{
    public enum GenderType
    {
        male,
        female
    }
    public class User:BaseModel
    {
        public int UserId { get; set; }
        [StringLength(10)]
        public string Gender { get; set; }
        public int Age { get; set; }
        public string UserName { get; set; }
        public virtual TwitterLogin TwitterLogin { get; set; }
        public virtual FaceBookLogin FaceBookLogin { get; set; }
        public virtual ICollection<ItemAnswer> ItemAnswer { get; set; }
        public virtual ICollection<QuestionAnswer> QuestionAnswer { get; set; }
        public virtual ICollection<FreeAnswer> FreeAnswer { get; set; }
        public virtual UserImage UserImage { get; set; }
    }

    //public class GenderType
    //{
    //    public int GenderTypeId { get; set; }
    //    public string Name { get; set; }
    //}

    public class TwitterLogin:BaseModel
    {
        public int TwitterLoginId { get; set; }
        public string ScreenName { get; set; }
        public string Token { get; set; }
    }

    public class FaceBookLogin:BaseModel
    {
        public int FaceBookLoginId { get; set; }
        public string ScreenName { get; set; }
        public string Token { get; set; }
    }

    public class UserImage:BaseModel
    {
        public int UserImageId { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public string ImageName { get; set; }
    }

    public class UserAnswerCompleted:BaseModel
    {
        public int UserAnswerCompletedId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<ItemAnswer> ItemAnsweres { get; set; }
        public virtual Research Research { get; set; }

    }
}