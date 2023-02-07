using AngleSharp.Dom;
using NomadicBook.Models.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Utils
{
    public interface IReptile
    {
        public IDocument GetWebPage(string url);
        public string GetHref(IDocument document);
        public string GetTitle(IDocument document);
        public IHtmlCollection<IElement> GetBookData(string path, IDocument document);
        public string GetAuthor(IHtmlCollection<IElement> htmlNodes);
        public (string company, string date) GetPublishData(IHtmlCollection<IElement> htmlNodes);
        public string GetIntroduction(IHtmlCollection<IElement> htmlNodes);
        public IElement GetBookDetail(IDocument document);
        public (double bookLong, double bookWidth, double bookHigh) GetSize(IElement htmlNode);
        public (string categoryId, string categoryName)  GetCategory(IElement htmlNode);
        public List<(string, string, string)> GetAllCategory(string path, byte mainId);
        public List<ConvenienceStore> GetStore();
    }
}
