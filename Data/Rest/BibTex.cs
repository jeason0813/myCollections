using System.Linq;
using System.Xml.Linq;
using myCollections.Utils;

namespace myCollections.Data.Rest
{
    internal class BibTex
    {

        public string author { get; set; }
        public string title { get; set; }
        public string pages { get; set; }
        public string publisher { get; set; }
        public string isbn { get; set; }
        public string editor { get; set; }
        public string booktitle { get; set; }
        public string year { get; set; }
        public string address { get; set; }
        public string volume { get; set; }

        public static BibTex XmlToBibTex(XElement objRest)
        {
            BibTex objItem=null;
            const string ns = "{http://bibtexml.sf.net/}";

            //if (objRest.ToString().IndexOf(ns) == -1)
            //    ns = string.Empty;

            if (objRest != null)
            {
                objItem  = new BibTex();

                objItem.address = Util.GetElementValue(objRest, ns +"address");
                objItem.author = Util.GetElementValue(objRest, ns + "author");
                objItem.booktitle = Util.GetElementValue(objRest, ns + "booktitle");
                objItem.editor = Util.GetElementValue(objRest, ns + "editor");
                objItem.isbn = Util.GetElementValue(objRest, ns + "isbn");
                objItem.pages = Util.GetElementValue(objRest, ns + "pages");

                if (string.IsNullOrWhiteSpace(objItem.pages) == false)
                    if (objItem.pages.Contains("-"))
                        objItem.pages = objItem.pages.Split('-').Last();

                objItem.publisher = Util.GetElementValue(objRest, ns + "publisher");
                objItem.title = Util.GetElementValue(objRest, ns + "title");
                objItem.volume = Util.GetElementValue(objRest, ns + "volume");
                objItem.year = Util.GetElementValue(objRest, ns + "year");

            }

            return objItem;
        }
    }

}