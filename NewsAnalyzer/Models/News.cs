using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAnalyzer.Models
{
	public class News
	{
		public int Id { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		public string Link { get; set; }
		public DateTime PublishDate { get; set; }
		[Range(0,100)]
		public int Sentiment { get; set; }
		public int PortalId { get; set; }
		public Portal Portal { get; set; }
    }
}
