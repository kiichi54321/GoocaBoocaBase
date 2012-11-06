using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GoocaBoocaDataModels
{
    public enum ImageType {jpeg,png }

    public class Item:BaseModel
    {
        public int ItemId { get; set; }
        [StringLength(32)]
        public string ItemName { get; set; }
        public virtual Research Resarch { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public virtual ItemCategory Category { get; set; }
        public virtual ICollection<ItemAttribute> ItemAttribute { get; set; } 
    }

    public class Image:BaseModel
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
    }

    public class ItemAttribute:BaseModel
    {
        public int ItemAttributeId { get; set; }
        public virtual Item Item { get; set; }
        public string AttributeCategory { get; set; }
        public string AttributeName { get; set; }
        public string Value { get; set; }
    }
}