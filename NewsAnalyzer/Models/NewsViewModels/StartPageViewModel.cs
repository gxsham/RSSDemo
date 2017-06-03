using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAnalyzer.Models.NewsViewModels
{
    public class StartPageViewModel
    {
		public string PortalName { get; set; }
		public int PositiveCount { get; set; }
		public int NegativeCount { get; set; }
    }
}
