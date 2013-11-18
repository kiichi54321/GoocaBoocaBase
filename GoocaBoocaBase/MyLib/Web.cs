using System;
using System.Web;
using System.Collections.Generic;
using System.Text;

namespace MyLib
{
	/// <summary>
	/// Web �̊T�v�̐����ł��B
	/// </summary>
	public class Web
	{
		public Web()
		{

		}
		/// <summary>
		/// �O�[�O���őΏی���������A�u���E�U�ŕ\�������܂��B
		/// </summary>
		/// <param name="word"></param>
		public static void SerchGoogle(string word)
		{
			string name = System.Web.HttpUtility.UrlEncode(word); 
			string url = "http://www.google.co.jp/search?hl=ja&c2coff=1&q="+name+"&lr=lang_ja";
			System.Diagnostics.Process process = System.Diagnostics.Process.Start(url); 

		}

		/// <summary>
		/// URL���u���E�U�ɕ\��������BURL�`�F�b�N�����Ȃ��Ƃ܂�����Ȃ��E�E�E
		/// </summary>
		/// <param name="url"></param>
		public static void OpenUrl(string url)
		{
			System.Diagnostics.Process process = System.Diagnostics.Process.Start(url); 
		}

		/// <summary>
        /// HTML�̃^�O��S���폜�B�܂������Q�Ɓi&lt;�Ȃǁj���u�����܂�
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string HtmlTagAllDelete(string html)
		{
			bool tagStart = false;

			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

			foreach(char c in html.ToCharArray())
			{
				if(tagStart == true)
				{
					if(c.Equals('>'))
					{
						tagStart = false;
						//tag�̏I�����ɂ̓X�y�[�X��������
						strBuilder.Append(' ');
					}				
				}
				else 
				{
					if(c.Equals('<'))
					{
						tagStart = true;
					}
					else
					{
						strBuilder.Append(c);
					}
				}
			}
			strBuilder.Replace("&nbsp;"," ");
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            strBuilder.Replace("&amp;", "&");
            strBuilder.Replace("&quot;", "\"");
			return strBuilder.ToString();

            
		}

        /// <summary>
        /// HTML�̃^�O��S���폜�B�܂������Q�Ɓi&lt;�Ȃǁj���u�����܂�
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlTagAllDelete(string html,string replacedTxt)
        {
            bool tagStart = false;

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

            foreach (char c in html.ToCharArray())
            {
                if (tagStart == true)
                {
                    if (c.Equals('>'))
                    {
                        tagStart = false;
                        //tag�̏I�����ɂ̓X�y�[�X��������
                        strBuilder.Append(replacedTxt);
                    }
                }
                else
                {
                    if (c.Equals('<'))
                    {
                        tagStart = true;
                    }
                    else
                    {
                        strBuilder.Append(c);
                    }
                }
            }
            strBuilder.Replace("&nbsp;", " ");
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            strBuilder.Replace("&amp;", "&");
            strBuilder.Replace("&quot;", "\"");
            return strBuilder.ToString();


        }

        /// <summary>
        /// ���ʁ���������HTML�G���R�[�h����
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SimpleHtmlEncode(string txt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(txt);
            strBuilder.Replace("<","&lt;");
            strBuilder.Replace(">","&gt;");
            return strBuilder.ToString();
        }

        /// <summary>
        /// ���ʁ���������HTML�f�R�[�h����
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string SimpleHtmlDecode(string txt)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(txt);
            strBuilder.Replace("&lt;", "<");
            strBuilder.Replace("&gt;", ">");
            return strBuilder.ToString();
        }

        public static List<string> GetTagContentList(string txt, string start, string end)
        {
            List<string> list = new List<string>();
            if (txt == null)
            {
                return list;
            }

            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start,idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(end, s+1);

                if (s < 0 || e < 0)
                {
                    break;
                }
                list.Add(txt.Substring(s + start.Length, e - s - start.Length));
                idx = e+1;
            }
            return list;
        }

        /// <summary>
        /// �^�O�ɋ��܂ꂽ���̂����X�g�����܂��Btag��True�ɂ����Ƃ��A�^�O���g���܂߂܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static List<string> GetTagContentList(string txt, string start, string end,bool tag)
        {
            List<string> list = new List<string>();
            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start, idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(end, s);

                if (s < 0 || e < 0)
                {
                    break;
                }
                if (tag)
                {
                    list.Add(txt.Substring(s, (e+end.Length) - s));
                }
                else
                {
                    list.Add(txt.Substring(s + start.Length, e - s - start.Length));
                }
                    idx = e;
            }
            return list;
        }


//        #region GetTag
//        private enum TagType
//        {
//            Start,End
//        }

//        /// <summary>
//        /// txt�̂���Atag�ŋ��܂ꂽ�v�f�����q�֌W���܂߂Ď���Ă���B
//        /// </summary>
//        /// <param name="txt"></param>
//        /// <param name="tag"></param>
//        /// <returns></returns>
//        public static ICollection<TagClass> GetTag(string txt, string tag)
//        {
//            List<MyLib.Collections.Pair<int, TagType>> list = new List<MyLib.Collections.Pair<int, TagType>>();
////            string startTag = "<" + tag + " " ;
//            string endTag = "</"+tag+">";
//            var startList = StringIndexList(txt, new string[]{ "<"+tag+" ","<"+tag+">"});
//            var endList = StringIndexList(txt, endTag);
//            Dictionary<int, TagClass> dic = new Dictionary<int, TagClass>();
//            foreach (var item in startList)
//            {
//                list.Add(new MyLib.Collections.Pair<int, TagType>(item, TagType.Start));
//                dic.Add(item, new TagClass(tag, item, txt));
//            }
//            foreach (var item in endList)
//            {
//                list.Add(new MyLib.Collections.Pair<int, TagType>(item,TagType.End));
//            }
//            list.Sort();

//            while (list.Count > 0)
//            {
//                List<MyLib.Collections.Pair<int, TagType>> tmpList = new List<MyLib.Collections.Pair<int, TagType>>();

//                for (int i = 0; i < list.Count - 1; i++)
//                {
//                    if (list[i].Value == TagType.Start && list[i + 1].Value == TagType.End)
//                    {
//                        var tagClass = dic[list[i].Key];
//                        tagClass.End = list[i + 1].Key;
//                        tmpList.Add(list[i]);
//                        tmpList.Add(list[i + 1]);

//                        if (i > 0 && list[i - 1].Value == TagType.Start)
//                        {
//                            var tagParent = dic[list[i - 1].Key];
//                            tagParent.Children.Add(tagClass);
//                            tagClass.Parent = tagParent;
//                        }
//                    }
//                }

//                foreach (var item in tmpList)
//                {
//                    list.Remove(item);
//                }

//                int s = 0;
//                int e = 0;
//                foreach (var item in list)
//                {
//                    if (item.Value == TagType.Start) s++;
//                    else if (item.Value == TagType.End) e++;
//                }
//                if (s == 0 || e == 0)
//                {
//                    if (e == 0)
//                    {
//                        //�J�n�^�O�΂���̎�
//                        for (int i = 0; i < list.Count-1; i++)
//                        {
//                            dic[list[i].Key].End = dic[list[i + 1].Key].Start - 1;
//                        }
//                    }
//                    break;
//                }
//            }

//            List<TagClass> result = new List<TagClass>();
//            foreach (var item in dic.Values)
//            {
//                if (item.Parent == null)
//                {
//                    result.Add(item);
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// ���͂ɑ΂��ĒP���T���Č��������ꏊ�̃��X�g�ł��B
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="search"></param>
//        /// <returns></returns>
//        public static List<int> StringIndexList(string text, string word)
//        {
//            List<int> list = new List<int>();
//            int idx = 0;
//            int s = 0;
//            int e = 0;

//            while (idx > -1)
//            {
//                s = text.IndexOf(word, idx);
//                if (s > -1)
//                {
//                    list.Add(s);
//                }
//                else
//                {
//                    break;
//                }
//                idx = s + 1;
//            }

//            return list;
//        }

//        /// <summary>
//        /// ���͂ɑ΂��ĒP���T���Č��������ꏊ�̃��X�g�ł��B
//        /// </summary>
//        /// <param name="text"></param>
//        /// <param name="search"></param>
//        /// <returns></returns>
//        public static List<int> StringIndexList(string text, string[] words)
//        {
//            List<int> list = new List<int>();

//            foreach (var item in words)
//            {
//                list.AddRange(StringIndexList(text, item));
//            }

//            return list;
//        }

//        public class TagClass
//        {
//            public int Start { get; set; }
//            public int End { get; set; }
//            public string StartTag { get; set; }
//            public string EndTag { get; set; }
//            private string baseText;
//            private string parameter = string.Empty;

//            public string Parameter
//            {
//                get { return parameter; }
//                set { parameter = value; }
//            }
//            public TagClass(string tag, int start, string text)
//            {
//                this.Start = start;
//                //                this.StartTag = startTag;
//                this.EndTag = "</" + tag + ">";
//                this.baseText = text;
//                if (baseText != null)
//                {
//                    int s = baseText.IndexOf('>', start);
//                    this.StartTag = baseText.Substring(start, s - start);
//                }
//            }
//            public TagClass Parent { get; set; }
//            private List<TagClass> children = new List<TagClass>();

//            public List<TagClass> Children
//            {
//                get { return children; }
//                set { children = value; }
//            }

//            public string Inner
//            {
//                get
//                {
//                    if (baseText != null)
//                    {
//                        if (this.End > 0)
//                        {
//                            return baseText.Substring(Start + StartTag.Length, End - (Start + StartTag.Length));
//                        }
//                        else
//                        {
//                            return baseText.Substring(Start + StartTag.Length);
//                        }
//                    }
//                    else
//                    {
//                        return null;
//                    }
//                }
//            }

//            public string Outer
//            {
//                get
//                {
//                    if (baseText != null)
//                    {
//                        if (this.End > 0)
//                        {
//                            return baseText.Substring(Start, End + EndTag.Length - (Start));
//                        }
//                        else
//                        {
//                            return baseText.Substring(Start + StartTag.Length);
//                        }
//                    }
//                    else
//                    {
//                        return null;
//                    }
//                }
//            }

//        }

//        #endregion 

        public static List<string> GetTagContentList(string txt, string start)
        {
            List<string> list = new List<string>();
            int idx = 0;
            int s = 0;
            int e = 0;
            while (idx > -1)
            {
                s = txt.IndexOf(start, idx);
                if (s < 0)
                {
                    break;
                }
                e = txt.IndexOf(start, s+1);

                if (s < 0 || e < 0)
                {
                    
                    break;
                }
                list.Add(txt.Substring(s , e - s));
                idx = e;
            }
            if (s > -1)
            {
                list.Add(txt.Substring(s, txt.Length - s));
            }
            return list;
        }


        /// <summary>
        /// HTML����IMG�^�O�𒊏o���ALink�I�u�W�F�N�g�ŕԂ��B
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ICollection<Link> GetImageLink(string Html, string url)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<IMG[^>]*?SRC\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(Html))
            {
                Link link = new Link();
                //link.Label = 
                string label =  GetTagContent(match.Value,"alt=\"","\"");
                if (label.Length == 0)
                {
                    label = GetTagContent(match.Value, "alt='", "'");
                    if (label.Length == 0)
                    {
                        label = GetTagContent(match.Value, "alt=", " ");
                    }
                }
                link.Label = label;
                link.Url = ChangeAbsoluteUriForUrl( match.Groups[2].Value,url);
                link.Tag = match.Value;
                linkList.Add(link);
            }
            return linkList;
        }




        /// <summary>
        /// �^�O�ɋ��܂ꂽ������𒊏o����B�^�O���̂��̂͏����܂��B
        /// ���܂ꂽ�����񂪑��݂��Ȃ��Ƃ���Null��Ԃ��܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static string GetTagContent(string txt, string startTag, string endTag)
        {
            if (txt == null || txt.Length == 0)
            {
                return null;
            } 
            
                string content = null;
            int s = txt.IndexOf(startTag);
            if (s < 0)
            {
                return null;
            }
            int e = txt.IndexOf(endTag, s+startTag.Length);

            if (s < 0 || e < 0)
            {
                return null;
            }
            else
            {
                content = txt.Substring(s + startTag.Length, e - s - startTag.Length);
            }
            return content;
        }

        /// <summary>
        /// �^�O�ɋ��܂ꂽ������𒊏o����Btag��Ture�̂Ƃ��A�^�O�������܂݂܂��B
        /// ���܂ꂽ�����񂪑��݂��Ȃ��Ƃ���Null��Ԃ��܂��B
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="startTag"></param>
        /// <param name="endTag"></param>
        /// <returns></returns>
        public static string GetTagContent(string txt, string startTag, string endTag,bool tag)
        {
            if (txt == null || txt.Length == 0)
            {
                return null;
            }
            string content = null;
            int s = txt.IndexOf(startTag);
            int e = txt.IndexOf(endTag, s + startTag.Length);

            if (s < 0 || e < 0)
            {
                return null;
            }
            else
            {
                if (tag)
                {
                    content = txt.Substring(s , e - s+endTag.Length);
                }
                else
                {
                    content = txt.Substring(s + startTag.Length, e - s - startTag.Length);
                }
            }
            return content;
        }


        public struct Link
        {
            private string url;

            public string Url
            {
                get { return url; }
                set { url = value; }
            }
            private string label;

            public string Label
            {
                get { return label; }
                set { label = value; }
            }
            private string tag;

            public string Tag
            {
                get { return tag; }
                set { tag = value; }
            }

        }
        /// <summary>
        /// HTML�t�@�C�����烊���N�𔲂��o���܂��B�Ԃ��̂�URL��Label�̃Z�b�g�̃R���N�V�����ł��B
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static ICollection<Link> GetLinkForHTML(string HTML)
        {
//            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=\s*""([^""]+)""[^>]*?>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            List<Link> linkList = new List<Link>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                Link link = new Link();
                link.Label = match.Groups[4].Value;
                link.Url = match.Groups[2].Value;
                link.Tag = match.Value;
                linkList.Add(link);
            }
            return linkList;
        }

        public static ICollection<Link> GetLinkForHTML(string HTML, string url)
        {
            Uri baseUri = new Uri(url);
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            List<Link> linkList = new List<Link>();

            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                Link link = new Link();
                link.Label = match.Groups[4].Value;
                link.Url = match.Groups[2].Value;
                link.Tag = match.Value;

                //HTTP�������Ă���Ƃ�
                if (regexHttp.IsMatch(link.Url) ==false)
                {
                    Uri uri = new Uri(baseUri, link.Url);
                    link.Url = uri.AbsoluteUri;
                }
                linkList.Add(link);
            }
           
            return linkList;
            
        }

        


        /// <summary>
        /// ���߂Ɍ������������N��Ԃ��B������Ȃ������Ƃ��͋�̃����N��Ԃ��B
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Link GetLinkFirstForHTML(string html, string url)
        {
            List<Link> list = new List<Link>(GetLinkForHTML(html, url));
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new Link();
            }
        }

        /// <summary>
        /// ���߂Ɍ������������N��Ԃ��B������Ȃ������Ƃ��͋�̃����N��Ԃ��B
        /// </summary>
        /// <param name="html"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Link GetLinkFirstForHTML(string html)
        {
            List<Link> list = new List<Link>(GetLinkForHTML(html));
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return new Link();
            }
        }

        /// <summary>
        /// html�̒�����ړI�̃��x���ɉ�����URL���Q�b�g����B�Ȃ��Ƃ��͋�̕�����B
        /// </summary>
        /// <param name="html">�Ώۂ�html</param>
        /// <param name="label">�~�������x��</param>
        /// <returns></returns>
        public static string GetUrlForHTML(string html, string label)
        {
            string url = "";
            foreach (MyLib.Web.Link link in MyLib.Web.GetLinkForHTML(html))
            {
                if (link.Label.Equals(label))
                {
                    url = link.Url;
                    break;
                }
            }
            return url;
        }

        /// <summary>
        /// html�̒�����ړI�̃��x���ɉ�����URL���Q�b�g����B�Ȃ��Ƃ��͋�̕�����B
        /// </summary>
        /// <param name="html">�Ώۂ�html</param>
        /// <param name="label">�~�������x��</param>
        /// <returns></returns>
        public static string GetUrlForHTML(string html, string label,string url)
        {
            string url1 = "";
            foreach (MyLib.Web.Link link in MyLib.Web.GetLinkForHTML(html,url))
            {
                if (link.Label.Equals(label))
                {
                    url1 = link.Url;
                    break;
                }
            }
            return url1;
        }

        /// <summary>
        /// HTML���̑��΃p�X���΃p�X��
        /// </summary>
        /// <param name="HTML"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ChangeAbsoluteUri(string HTML, string url)
        {
            Uri baseUri = new Uri(url);
            System.Text.RegularExpressions.Regex regexHref = new System.Text.RegularExpressions.Regex(@"HREF\s*=(\s*|\s*[""])([^""\s]+)([""]|\s)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexSrc = new System.Text.RegularExpressions.Regex(@"SRC\s*=(\s*|\s*[""])([^""\s]+)([""]|\s)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Dictionary<string, string> repleceDic = new Dictionary<string, string>();
            foreach (System.Text.RegularExpressions.Match match in regexHref.Matches(HTML))
            {
                string href = match.Groups[2].Value;
                if (regexHttp.IsMatch(href) == false)
                {
                    Uri uri = new Uri(baseUri, href);
                    if (repleceDic.ContainsKey(href) == false)
                    {
                        repleceDic.Add(href, uri.AbsoluteUri);
                    }
                }
            }
            foreach (System.Text.RegularExpressions.Match match in regexSrc.Matches(HTML))
            {
                string href = match.Groups[2].Value;
                if (regexHttp.IsMatch(href) == false)
                {
                    Uri uri = new Uri(baseUri, href);
                    if (repleceDic.ContainsKey(href) == false)
                    {
                        repleceDic.Add(href, uri.AbsoluteUri);
                    }
                }
            }
            StringBuilder strBuilder = new StringBuilder(HTML);
            foreach(string key in repleceDic.Keys)
            {
                strBuilder.Replace("\""+key+"\"","\""+ repleceDic[key]+"\"");
                
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// URL���΂ɕϊ����܂��B
        /// </summary>
        /// <param name="url"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public static string ChangeAbsoluteUriForUrl(string url, string baseUrl)
        {
            Uri baseUri = new Uri(baseUrl);
            string url1 = url;
            System.Text.RegularExpressions.Regex regexHttp = new System.Text.RegularExpressions.Regex("^http", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (regexHttp.IsMatch(url) == false)
            {
                Uri uri = new Uri(baseUri, url);
                url1 = uri.AbsoluteUri;
            }
            return url1;
        }


        public static ICollection<Link> CreateLinkCollection(string[] lines)
        {
            List<Link> list = new List<Link>();
            foreach (string line in lines)
            {
                if (line.Length > 0)
                {
                    string[] data = line.Split('\t');
                    Link link = new Link();
                    if (data.Length > 1)
                    {
                        link.Url = data[0];
                        link.Label = data[1];

                    }
                    else
                    {
                        link.Url = data[0];
                        link.Label = System.IO.Path.GetFileNameWithoutExtension(data[0]);
                    }
                    list.Add(link);
                }
            }
            return list;
        }

        public static string GetTitleForHTML(string html)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<title>([^<]*)</title>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match match = regex.Match(html);
            if (match.Groups.Count > 0)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }

        //public struct Table
        //{
        //    public ICollection<ICollection<string>> contents;
        //}

        ///// <summary>
        ///// ����q��Ԃ̔����o���͂ł��Ȃ�����ǁAtable�^�O��ǂ݂Ƃ��ăR���N�V�����ŕێ����Ă���郁�\�b�h
        ///// </summary>
        ///// <param name="HTML"></param>
        ///// <returns></returns>
        //public static ICollection<Table> GetTableForHTML(string HTML)
        //{
        //    System.Text.RegularExpressions.Regex regex_table = new System.Text.RegularExpressions.Regex(@"<table[^>]*?>([\s\S]*?)<\/table>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //    System.Text.RegularExpressions.Regex regex_tr = new System.Text.RegularExpressions.Regex(@"<tr[^>]*?>([\s\S]*?)<\/tr>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        //    System.Text.RegularExpressions.Regex regex_td = new System.Text.RegularExpressions.Regex(@"<td[^>]*?>([\s\S]*?)<\/td>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            
        //    List<Table> tableList = new List<Table>();
        //    foreach (System.Text.RegularExpressions.Match match in regex_table.Matches(HTML))
        //    {
        //        string table_html = match.Groups[0].Value;
        //        List<List<string>> tr_list = new List<List<string>>();
        //        foreach (System.Text.RegularExpressions.Match match1 in regex_tr.Matches(table_html))
        //        {
        //            string tr_html = match1.Groups[0].Value;
        //            List<string> td_list = new List<string>();
        //            foreach (System.Text.RegularExpressions.Match match2 in regex_td.Matches(table_html))
        //            {
        //                td_list.Add(match2.Groups[0]);
        //            }
        //            tr_list.Add(td_list);
        //        }
        //        Table table = new Table();
        //        table.contents = tr_list;
        //        tableList.Add(table);

        //    }
        //    return tableList;

        //}





        public static string GetMailAddress(string txt)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"[a-z][0-9a-z_.\-]*@[0-9a-z\-]+(\.[0-9a-z\-]+)+");
            return regex.Match(txt).Value;
        }

        public static string DeleteTargetBlank(string html)
        {
            StringBuilder strBuilder = new StringBuilder(html);
            foreach (string link in GetLinkHtml(html))
            {
                if (link.Contains("target=\"_blank"))
                {
                    string link2 = link.Replace("target=\"_blank","");
                    strBuilder.Replace(link, link2);
                }
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// HTML�t�@�C������A���J�[�^�O�����������N�𔲂��o���܂��B
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static ICollection<string> GetLinkHtml(string HTML)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<A[^>]*?HREF\s*=(\s*|\s*[""])([^"">]+)([""][^>]*?|[^>]*?)>([\s\S]*?)<\/A>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            List<string> list = new List<string>();
            foreach (System.Text.RegularExpressions.Match match in regex.Matches(HTML))
            {
                list.Add(match.Value);
            }
            return list;
        }

        /// <summary>
        /// �������O�C���p��HTML�����܂��B
        /// </summary>
        /// <param name="loginUrl"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string CreateLoginHtml(string loginUrl, List<KeyValuePair<string, string>> list)
        {
            StringBuilder strbuilder = new StringBuilder();
            strbuilder.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
            strbuilder.AppendLine("<html lang=\"ja\">");
            strbuilder.AppendLine("<head>");
            strbuilder.AppendLine("<meta http-equiv=\"content-type\" content=\"text/html; charset=shift_jis\">");
            strbuilder.AppendLine("<meta http-equiv=\"content-script-type\" content=\"text/javascript\">");
            strbuilder.AppendLine("<title>�������O�C��</title>");
            strbuilder.AppendLine("</head>");
            strbuilder.AppendLine("<body onLoad=\"document.form.submit()\">");
            strbuilder.AppendLine("<form action=\""+loginUrl+"\" method=\"post\" name=\"form\">");
            strbuilder.AppendLine("<div>");

            foreach(KeyValuePair<string,string> pair in list)
            {
                strbuilder.AppendLine("<input type=\"hidden\" name=\""+pair.Key+ "\" value=\""+pair.Value+"\">");
            }

            strbuilder.AppendLine("</div></form></body></html>");
            return strbuilder.ToString();
        }


        /// <summary>
        /// �w�肵���^�O�͈̔͂𒲂ׂĂ��͈̔͂ɂ��镶�����Ԃ��B(�厸�s)
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public static string GetTagRange(string html, string tag, string tagString,int startIndex,out int endIndex)
        {
            int start = html.IndexOf(tagString,startIndex);
            endIndex = start;
            if(start <0)
            {
                return string.Empty;
            }
            int p = start+1;
            int tmp, end=0;
            int count = 0;
            while(count >=0)
            {
                string tagend = "</" + tag;
                tmp = html.IndexOf(tagend, p);
                //  tmp = html.IndexOf(  tag, p,StringComparison.CurrentCultureIgnoreCase);
                if (tmp > 0)
                {
                    count--;
                    end = tmp;
                    
                }
                else
                {
                    p = start;
                    break;
                }
                while (tmp > 0)
                {
                    tmp = html.IndexOf("<" + tag, p);
                    if (tmp > 0)
                    {
                        if (end > tmp)
                        {
                            count++;
                            p = tmp+1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                p = end+1;

            }
            if(start == p)
            {
                return string.Empty;
            }
            else
            {
                string endTag = "</" + tag + ">";
                endIndex = end+endTag.Length;

                return html.Substring(start, endIndex - start);
            }
        }
        /// <summary>
        /// �w�肵���^�O�͈̔͂𒲂ׂĂ��͈̔͂ɂ��镶�����Ԃ��B(�厸�s)
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public static string GetTagRange(string html, string tag, string tagString)
        {
            int i;
            return GetTagRange(html, tag, tagString, 0,out i);
        }

        /// <summary>
        /// �w�肵���^�O�Ɉ͂܂ꂽ������̃��X�g��Ԃ�(�厸�s)
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tag"></param>
        /// <param name="tagString"></param>
        /// <returns></returns>
        public static List<string> GetTagRangeList(string html, string tag, string tagString)
        {
            List<string> list = new List<string>();
            int startIndex = 0;
            int endIndex = 0;
            while(startIndex>-1)
            {
                string str = GetTagRange(html,tag,tagString,startIndex,out endIndex);
                if(str.Length >0)
                {
                    list.Add(str);
                    startIndex = endIndex;
                }
            }
            return list;
        }

        /// <summary>
        /// �X�N���v�g���폜����B
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string DeleteScript(string html)
        {
            StringBuilder strBuilder = new StringBuilder(html);
            List<string> list = MyLib.Web.GetTagContentList(html, "<script", "</script>", true);
            foreach (string s in list)
            {
                strBuilder.Replace(s, "");
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// URL�Ɋ܂ރp�����[�^��DIC�Ŏ擾���܂��BURL�f�R�[�h������܂��B
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetUrlParameter(string url)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            url = url.Replace("&amp;", "&");
            string[] data = url.Split('?');
            if (data.Length > 1)
            {
                string[] para = data[1].Split('&');
                foreach (var item in para)
                {
                    string[] keyval = item.Split('=');
                    if (keyval.Length > 1)
                    {
                        dic.Add(keyval[0],HttpUtility.UrlDecode( keyval[1]));
                    }
                }
            }
            return dic;
        }
	}
}
