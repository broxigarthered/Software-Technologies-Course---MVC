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

        public ActionResult Index()
        {
            var posts = db.Posts.Include(p => p.Author)
                .OrderByDescending(p => p.Date)
                .Take(3);

            var postsForSideBars = db.Posts.Include(p => p.Author)
                .OrderByDescending(p => p.Date)
                .Take(5);
            ViewBag.SidebarPosts = postsForSideBars.ToList();

            return View(posts.ToList());
        }

    }
}