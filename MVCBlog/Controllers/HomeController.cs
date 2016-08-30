using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCBlog.Models;

namespace MVCBlog.Controllers
{
    public class HomeController : Controller
    {

        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int page = 0)
        {
            const int PagePostsCount = 3; 

            var count = this.db.Posts.Count();

            var posts = this.db.Posts.Include(p => p.Author)
                .OrderByDescending(p => p.Date)
                .Skip(page * PagePostsCount)
                .Take(PagePostsCount).ToList();

            this.ViewBag.MaxPage = (count / PagePostsCount) - (count % PagePostsCount == 0 ? 1 : 0);

            this.ViewBag.Page = page;

            var postsForSideBars = db.Posts.Include(p => p.Author)
                .OrderByDescending(p => p.Date)
                .Take(5);
            ViewBag.SidebarPosts = postsForSideBars.ToList();

            return View(posts);
        }

    }
}