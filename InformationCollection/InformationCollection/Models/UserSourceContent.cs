using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimiSoft.InformationCollection.Models
{
    public class UserSourceContent
    {
        public int SourceId { get; set; }
        public string Content { get; set; }
        public int ContentType { get; set; }
        public string Url { get; set; }
        public System.DateTime AddTime { get; set; }
        public int SourceContentId { get; set; }
        public string Source { get; set; }
        public bool IsFavor { get; set; }
    }
}
