using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models;
using _036_MoviesMvcWissen.Models.ViewModels;

namespace _036_MoviesMvcWissen.Controllers
{
    public class DirectorsController : Controller
    {
        private MoviesContext db = new MoviesContext();

        // GET: Directors
        public ActionResult Index()
        {
            var model = new DirecstorIndexViewModel()
            {
                Directors = db.Directors.ToList()
            };
            //return View(db.Directors.ToList());
            return View(model);
        }

        // GET: Directors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }
        [HttpGet]
        // GET: Directors/Create
        public ActionResult Create()
        {
            ViewBag.Message = "Please enter movie information...";
            var movies = db.Movies.ToList().Select(e => new SelectListItem()
            {
                Text = e.Name,
                Value = e.Id.ToString()
            }).ToList();
            ViewData["movies"] = new MultiSelectList(movies, "Value", "Text");
            return View();
        }

        // POST: Directors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Surname,Retired")] Director director)
        [ActionName("Create")]
        //public ActionResult CreateNew()   //1
        public ActionResult CreateNew(FormCollection formCollection, List<int> Movies)    //2
        {
            var director = new Director()
            {
                //Name = Request.Form["Name"],  //1
                //Surname = Request.Form["Surname"]    //1
                Name = formCollection["Name"],      //2
                Surname = formCollection["Surname"],     //2
            };

            /*var retired = Request.Form["Retired"];*/      //1
            var retired = formCollection["Retired"];
            var movieIds = formCollection["movieIds"].Split(',');
            director.Retired = true;
            if (retired.Equals("false"))
            {
                director.Retired = false;
            }
            if (String.IsNullOrWhiteSpace(director.Name))
            {
                ModelState.AddModelError("Name", "Director Name is required!");
            }
            if (String.IsNullOrWhiteSpace(director.Surname))
            {
                ModelState.AddModelError("Surname", "Director Surname is required!");
            }
            if (director.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Director Name must be maximum 100 characters!");
            }
            if (director.Surname.Length > 100)
            {
                ModelState.AddModelError("Name", "Director Surname must be maximum 100 characters!");
            }
            if (ModelState.IsValid)
            {
                db.Directors.Add(director);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                director.MovieDirectors = Movies.Select(e => new MovieDirector()
                {
                    DirectorId = director.Id,
                    MovieId = Convert.ToInt32(e),

                }).ToList();
                db.Directors.Add(director);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(director);
        }

        [HttpGet]
        // GET: Directors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            }

            var model = db.Directors.Find(id.Value);

            var movies = db.Movies.Select(e => new MovieModel()
            {
                Id = e.Id,
                MovieName = e.Name
            }).ToList();

            var movieIds = model.MovieDirectors.Select(e => e.MovieId).ToList();
            ViewBag.movies = new MultiSelectList(movies, "Id", "MovieName", movieIds);
            return View(model);
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Director director = db.Directors.Find(id);
            //if (director == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(director);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Retired")] Director director, List<int> movieIds)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(director).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            var entity = db.Directors.SingleOrDefault(e => e.Id == director.Id);
            var dbDirector = db.Directors.Find(director.Id);
            entity.Name = director.Name;
            entity.Surname = director.Surname;
            entity.Retired = director.Retired;
            entity.MovieDirectors = new List<MovieDirector>();
            var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id);
            foreach (var movieDirector in movieDirectors)
            {
                db.MovieDirectors.Remove(movieDirector);
            }

            foreach (var movieId in movieIds)
            {
                var movieDirector = new MovieDirector()
                {
                    MovieId = movieId,
                    DirectorId = director.Id
                };
                entity.MovieDirectors.Add(movieDirector);
            }
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Info"] = "Record successfully updated in database";
            return RedirectToRoute(new { controller = "Directors", action = "Index" });
            //return View(director);
        }

        // GET: Directors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Director director = db.Directors.Find(id);
            db.Directors.Remove(director);
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
