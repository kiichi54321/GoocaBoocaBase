using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GoocaBoocaDataModels
{
    public class ItemCategory:BaseModel
    {
        public int ItemCategoryId { get; set; }
        [StringLength(50)]
        public string ItemCategoryName { get; set; }
        public virtual Research Research { get; set; }
        public virtual ICollection<Item> Items { get; set; }
        public virtual Image CategoryImage { get; set; }
    }
}