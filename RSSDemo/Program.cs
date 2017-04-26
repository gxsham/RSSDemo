using RSSReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSSDemo
{
	public class Program
	{
		static void Main(string[] args)
		{
			RSSParser parser = new RSSParser();
			var list = parser.Parse("zdrs", "http://www.interlic.md/rss/politics-news/");
			Console.ReadKey();
		}
	}
}