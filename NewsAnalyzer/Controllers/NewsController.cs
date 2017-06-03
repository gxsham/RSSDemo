using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsAnalyzer.Data;
using NewsAnalyzer.Models;
using Microsoft.AspNetCore.Authorization;
using RSSReader;
using System.Net.Http;
using NewsAnalyzer.Models.NewsViewModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NewsAnalyzer.Controllers
{
	[Authorize]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NewsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: News
		[AllowAnonymous]
        public async Task<IActionResult> Index(int page)
        {
            var newsList = _context.News.OrderByDescending(x=>x.PublishDate).Skip(page*30).Take(30).Include(x=>x.Portal);
            return View(await newsList.ToListAsync());
        }
		
		[AllowAnonymous]
		public ActionResult StartingPage()
		{
			var portalList = _context.Portals;
			var result = new List<StartPageViewModel>();
			foreach (var item in portalList)
			{
				var resultingElement = new StartPageViewModel();
				resultingElement.PortalName = item.Name;
				resultingElement.NegativeCount = _context.News.Count(x=>x.Portal == item && x.Sentiment == 0);
				resultingElement.PositiveCount = _context.News.Count(x => x.Portal == item && x.Sentiment == 1);
				if(resultingElement.PositiveCount!=0 || resultingElement.NegativeCount!=0)
					result.Add(resultingElement);
			}
			return View(result);
		}
        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Portal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: News/Create
        public IActionResult Create()
        {
            ViewData["PortalId"] = new SelectList(_context.Portals, "Id", "Name");
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Link,PublishDate,Sentiment,PortalId")] News news)
        {
            if (ModelState.IsValid)
            {
                _context.Add(news);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["PortalId"] = new SelectList(_context.Portals, "Id", "Name", news.PortalId);
            return View(news);
        }

        // GET: News/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.SingleOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            ViewData["PortalId"] = new SelectList(_context.Portals, "Id", "Name", news.PortalId);
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Link,PublishDate,Sentiment,PortalId")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["PortalId"] = new SelectList(_context.Portals, "Id", "Name", news.PortalId);
            return View(news);
        }

        // GET: News/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Portal)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.SingleOrDefaultAsync(m => m.Id == id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }

		public async Task<IActionResult> GetUpdates()
		{
			RSSParser rss = new RSSParser();
			Random rnd = new Random();
			var client = new HttpClient();
			var list = await _context.Portals.ToListAsync();
			foreach (var item in list)
			{
				var result = rss.Parse(item.Name, item.RSSLink, item.Category);
				//var lastNews =  await _context.News.Where(x => x.Portal == item).OrderByDescending(x=>x.PublishDate).Take(1).FirstOrDefaultAsync();
				//if(lastNews!=null)
				//{
				//	result.Where(x => DateTime.Compare(x.PublishTime, lastNews.PublishDate) > 1).ToList();
					var titleJson =  Newtonsoft.Json.JsonConvert.SerializeObject(result.Select(x => x.Title));
					try
					{
						var response = await client.GetStringAsync($"http://13.79.234.96/evaluate/?sentence={titleJson}");
						var token = Newtonsoft.Json.JsonConvert.DeserializeObject<List<double>>(response);
						foreach (var element in result)
						{
							_context.News.Add(new News
							{
								Title = element.Title,
								Link = element.Link,
								PublishDate = element.PublishTime,
								Sentiment = (int)token[result.IndexOf(element)],
								Portal = item
							});
						}

						await _context.SaveChangesAsync();
					} catch { }
				}
			//}
			return RedirectToAction("Index");

	}
		[AllowAnonymous]
		[HttpGet]
		public  ViewResult Compare(int page)
		{
			var newsList =  _context.News.OrderByDescending(x => x.PublishDate).Skip(page*30).Take(30).Include(x => x.Portal);
			var list = new List<CompareViewModel>();
			foreach (var item in newsList)
			{
				list.Add(new CompareViewModel { News = item, Value = false});
			}
			return View(list);
		}
	}
}
