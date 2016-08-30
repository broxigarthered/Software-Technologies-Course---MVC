using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using MVCBlog.Extensions;
using MVCBlog.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace MVCBlog.Controllers
{
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            //string message = TempData["message"] as string;
            //ViewBag.message = message;

            //todo set the message for deleting etc
            return View(db.Posts.Include(p => p.Author).ToList());
        }

        // GET: Posts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //// POST: Posts/Comment/5
        //public ActionResult PostComment(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Post post = db.Posts.Find(id);
        //    if (post == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    // TODO: Get comment from area
        //    // Form.get("textarea")
        //    // Comment currentComment = new Comment (postId, commentStr)
        //    // Post.
        //    return View("Post.html");

        //}

        // GET: Posts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Body,Date")] Post post)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                if (user != null)
                {
                    post.Author = db.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
                }

                db.Posts.Add(post);
                this.AddNotification("Post Created.", NotificationType.INFO);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        //[Authorize(Roles = "Administrators")]
        public ActionResult Edit(int? id)
        {
            var userStore = new UserStore<ApplicationUser>(db);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>(db);
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            // if the author of the post is the one currently logged or the admin is logged - post editing is enabled
            if (user.Id == post.Author_Id || userManager.IsInRole(user.Id, "Administrators"))
            {
                var authors = db.Users.ToList();
                ViewBag.Authors = authors;

                return View(post);
            }

            // if not, we create temp data message which then will be printed on the post index
           // TempData["message"] = "You're not allowed to edit this post";
            this.AddNotification("You cannot edit that post.", NotificationType.INFO);
        
            return RedirectToAction("Index");
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        // [Authorize(Roles = "Administrators")]
        public ActionResult Edit([Bind(Include = "ID,Title,Body,Date,Author_ID")] Post post)
        {
            ApplicationDbContext userscontext = new ApplicationDbContext();
            var userStore = new UserStore<ApplicationUser>(userscontext);
            var userManager = new UserManager<ApplicationUser>(userStore);

            var roleStore = new RoleStore<IdentityRole>(userscontext);
            var roleManager = new RoleManager<IdentityRole>(roleStore);

            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            if (user.Id == post.Author_Id || userManager.IsInRole(user.Id, "Administrators"))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(post).State = EntityState.Modified;
                    this.AddNotification("Post Edited successfully.", NotificationType.INFO);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                var authors = db.Users.ToList();
                ViewBag.Authors = authors;
                return View(post);

            }
            return Redirect("Index");
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrators")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrators")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
