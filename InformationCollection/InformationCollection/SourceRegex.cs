using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimiSoft.InformationCollection
{
    public class SourceRegex
    {
        [Display(Name = "组")]
        public string SourceRegexGroup { get; set; }
        [Display(Name = "编号")]
        public int SourceRegexId { get; set; }
        [Display(Name = "表达式")]
        public string Regex { get; set; }
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "匹配")]
        public bool IsMatched { get; set; }
        [Display(Name = "类型")]
        public string RegexType { get; set; }
        [Display(Name = "组编号")]
        public int SourceRegexGroupId { get; set; }
    }
}
