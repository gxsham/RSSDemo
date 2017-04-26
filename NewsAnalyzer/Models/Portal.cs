using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAnalyzer.Models
{
	
    public class Portal
    {
		public int Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string RSSLink { get; set;}
		public string Category { get; set; }
		public ICollection<News> News { get; set; }
		
    }
}
