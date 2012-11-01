using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GoocaBoocaDataModels
{
    public enum ImageType {jpeg,png }

    public class Item
    {
        public int ItemId { get; set; }
        [StringLength(32)]
        public string ItemName { get; set; }
        public virtual Research Resarch { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public virtual ItemCategory Category { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public string Tag { get; set; }
    }

    public class Image
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageType { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime Upd_Date { get; set; }
        public string Tag { get; set; }
    }
}