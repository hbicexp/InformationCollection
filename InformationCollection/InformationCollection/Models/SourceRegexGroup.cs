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
    
    public partial class SourceRegexGroup
    {
        public SourceRegexGroup()
        {
            this.SourceRegexes = new HashSet<SourceRegex>();
        }
    
        public int SourceRegexGroupId { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Regex { get; set; }
        public bool Enabled { get; set; }
        public bool Decode { get; set; }
    
        public virtual ICollection<SourceRegex> SourceRegexes { get; set; }
    }
}