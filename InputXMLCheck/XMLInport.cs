using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace InputXMLCheck
{
    class XMLInport
    {
        public static string RenameImage(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            var folder = System.IO.Path.GetDirectoryName(fileName) + "/";
            var xml = XElement.Load(fileName);
            var ns = xml.GetDefaultNamespace();
            if (System.IO.Directory.Exists(folder + "Copy/") == false)
            {
                System.IO.Directory.CreateDirectory(folder + "Copy/");
            }
            int count = 1;
            foreach (var item in xml.DescendantsAndSelf(ns + "Category"))
            {
                foreach (var item2 in item.Elements("item"))
                {
                    var url = item2.Attribute("ImageUrl") != null ? item2.Attribute("ImageUrl").Value : null;
                    if (url != null)
                    {
                        url = System.IO.Path.GetFileNameWithoutExtension(url.Trim()) + ".jpg";
                        bool flag = false;
                        if (System.IO.File.Exists(folder + url))
                        {
                            flag = true;
                        }
                        else
                        {
                            url = System.IO.Path.GetFileNameWithoutExtension(url.Trim()) + ".jpeg";
                            if (System.IO.File.Exists(folder + url))
                            {
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            System.IO.File.Copy(folder + url, folder + "Copy/" + count.ToString("D4") + System.IO.Path.GetExtension(url), true);
                            item2.Attribute("ImageUrl").Value = count.ToString("D4") + System.IO.Path.GetExtension(url);
                        }
                        else
                        {
                      //      item2.Remove();
                            sb.AppendLine(url + "が見つかりませんでした");
                        }

                    }
                    else
                    {
                       // item2.Remove();
                        sb.AppendLine(count + "が見つかりませんでした");
                    }
                    count++;
                }
            }
            xml.Save(folder + System.IO.Path.GetFileNameWithoutExtension(fileName) + "_new.xml");
            return sb.ToString();
        }
 
    }
}
