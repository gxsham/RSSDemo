using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAnalyzer.Models.NewsViewModels
{
    public class CompareViewModel
    {
		public int Id { get; set; }
		public News News { get; set; }
		public bool Value { get; set; }
    }
}
