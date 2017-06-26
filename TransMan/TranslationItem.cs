using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransMan
{
    public class TranslationItem
    {
        public enum OperationTypeEnum
        {
            Insert, Update, Delete
        }

        public string Keyword { get; set; }
        public string English { get; set; }
        public string German { get; set; }
        public OperationTypeEnum Operation { get; set; }
        public TranslationItem UpdatedItem { get; set; }

        public TranslationItem() { }

        public TranslationItem(string keyword, string english, string german, OperationTypeEnum operation)
        {
            Keyword = keyword;
            English = english;
            German = german;
            Operation = operation;
        }
    }
}
