using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;

namespace TimiSoft.InformationCollection.Models
{
    /// <summary>
    /// User Source class
    /// </summary>
    public class SourceView
    {
        [Display(Name = "编号")]
        public int SourceId { get; set; }
        
        [Required]
        [Display(Name = "任务名称")]
        public string SourceName { get; set; }

        [Required]
        [Display(Name = "任务地址")]
        public string Url { get; set; }

        [Display(Name = "添加时间")]
        public DateTime CreateTime { get; set; }

        [Required]
        [Display(Name = "频率")]
        public int Interval { get; set; }
    }
}