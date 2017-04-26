using System;
using System.Collections.Generic;
using System.Text;

namespace RSSReader
{
    public class Item
    {
		public string Portal { get; set; }
		public string Link { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime PublishTime { get; set; }

		public Item()
		{
			Portal = "";
			Link = "";
			Title = "";
			Description = "";
			PublishTime = DateTime.Today;
		}

    }
}
