using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSSReader
{
	public class RSSParser
	{
		public IList<Item> Parse(string PortalName, string RssLink, string Category = "")
		{
			var itemList = new List<Item>();
			var result = GetFeed(RssLink);
			try
			{
				var list = XDocument.Load(result.Result).Root.Element("channel").Elements("item");
				if (!string.IsNullOrEmpty(Category))
				{
						list = list.Where(x => x.Element("category").Value == Category);		
				}
				foreach (var item in list)
				{
					var news = new Item
					{
						Portal = PortalName,
						Title = item.Element("title").Value,
						Link = item.Element("link").Value,
						Description = ExtractText(item.Element("description").Value),
						PublishTime = ParseDate(item.Element("pubDate").Value)
					};
					itemList.Add(news);
				}
			}
			catch(ArgumentException) { }
			
			return itemList;
		}
		private async Task<Stream> GetFeed(string url)
		{
			try
			{
				var client = new HttpClient();
				var stream = await client.GetStreamAsync(url);
				return stream;
			}
			catch (HttpRequestException)
			{
				return null;
			}
		}
		private  DateTime ParseDate(string date)
		{
			DateTime result;
			if (DateTime.TryParse(date, out result))
				return result;
			else
				return DateTime.MinValue;
		}
		private string ExtractText(string html)
		{
			Regex rx = new Regex("<.*?>");
			return rx.Replace(html, "").Trim();
		}
	}
}
