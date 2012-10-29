using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing;

namespace GoocaBoocaBase.Controllers
{
    public class ImageController : Controller
    {
        //
        // GET: /Image/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Download(string id)
        {
            if (id != null)
            {
                GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase();
                int idd;
                if (int.TryParse(id, out idd))
                {
                    var item = db.Images.Where(n => n.ImageId == idd);
                    if (item.Any())
                    {
                        var image = item.First();
                        if (image.ImageData != null)
                        {
                            if (image.ImageType == GoocaBoocaDataModels.ImageType.jpeg.ToString() || image.ImageType == "jpg")
                            {
                                return File(item.First().ImageData, "image/jpeg");
                            }
                            if (image.ImageType == GoocaBoocaDataModels.ImageType.png.ToString())
                            {
                                return File(item.First().ImageData, "image/png");
                            }
                        }
                    }
                }
            }
            using (var font = new Font("MS UI Gothic", 16))
            using (var brash = new SolidBrush(Color.White))
            using (var brash2 = new SolidBrush(Color.Red))
            using (Bitmap img = new Bitmap(100, 200))
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(img);
                g.FillEllipse(brash2, 0, 0, 200, 400);
                g.DrawString("NoImage", font, brash, new PointF(5, 5));
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                var array = ms.ToArray();
                return File(ms.ToArray(), "image/png", "");
            }
        }


        public ActionResult ItemDownload(string id)
        {
            if (id != null)
            {
                GoocaBoocaDataModels.GoocaBoocaDataBase db = new GoocaBoocaDataModels.GoocaBoocaDataBase(); 
                int idd;
                if (int.TryParse(id, out idd))
                {
                    var item = db.Items.Where(n => n.ItemId == idd);
                    if (item.Any())
                    {
                        var image = item.First();
                        if (image.ImageData != null)
                        {
                            if (image.ImageType == GoocaBoocaDataModels.ImageType.jpeg.ToString() || image.ImageType == "jpg")
                            {
                                return File(item.First().ImageData, "image/jpeg");
                            }
                            if (image.ImageType == GoocaBoocaDataModels.ImageType.png.ToString())
                            {
                                return File(item.First().ImageData, "image/png");
                            }
                        }
                    }
                }
            }
            using (var font = new Font("MS UI Gothic", 16))
            using (var brash = new SolidBrush(Color.White))
            using (var brash2 = new SolidBrush(Color.Red))
            using (Bitmap img = new Bitmap(100, 200))
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(img);
                g.FillEllipse(brash2, 0, 0, 200, 400);
                g.DrawString("NoImage", font, brash, new PointF(5, 5));
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                var array = ms.ToArray();
                return File(ms.ToArray(), "image/jpeg", "");
            }




        }
    }
}
