//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TimiSoft.InformationCollection.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SourceRegex
    {
        public int SourceRegexId { get; set; }
        public string Regex { get; set; }
        public string Name { get; set; }
        public bool IsMatched { get; set; }
        public string RegexType { get; set; }
        public int SourceRegexGroupId { get; set; }
    
        public virtual SourceRegexGroup SourceRegexGroup { get; set; }
    }
}
