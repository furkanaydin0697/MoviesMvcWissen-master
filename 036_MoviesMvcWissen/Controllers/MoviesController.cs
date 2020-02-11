﻿using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models;
using _036_MoviesMvcWissen.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller
    {
        MoviesContext db = new MoviesContext();
        // GET: Movies
        //public ViewResult Index()
        public ViewResult Index(MoviesIndexViewModel moviesIndexViewModel)
        {
            //var model = db.Movies.ToList();
            var model = GetList();
            //ViewData["count"] = model.Count;
            ViewBag.count = model.Count;
            var years = new List<SelectListItem>();
            years.Add(new SelectListItem()
            {
                Value = "",
                Text = "---All---"
            });
            for (int i = DateTime.Now.Year; i >= 1950; i--)
            {
                years.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

            
            if (moviesIndexViewModel == null)
            {
                moviesIndexViewModel = new MoviesIndexViewModel();
            }
            if (String.IsNullOrWhiteSpace(moviesIndexViewModel.YearId))
            {
                moviesIndexViewModel.Movies = db.Movies.ToList();
            }
            else
            {
                moviesIndexViewModel.Movies = db.Movies.Where(e => e.ProductionYear == moviesIndexViewModel.YearId).ToList();
            }
            moviesIndexViewModel.Year = new SelectList(years, "Value", "Text", moviesIndexViewModel.YearId);

            //var moviesIndexViewModel = new MoviesIndexViewModel()
            //{
            //    Movies = model
            //};
            //return View(model);
            return View(moviesIndexViewModel);
        }

        [NonAction]
        public List<Movie> GetList(bool removeSession = true)
        {
            List<Movie> entities;
            if (removeSession)
            {
                Session.Remove("movies");
            }
            if (Session["movies"] == null || removeSession)
            {
                entities = db.Movies.ToList();
                Session["movies"] = entities;
            }
            else
            {
                entities = Session["movies"] as List<Movie>;
            }
            return entities;
        }

        public ActionResult GetMoviesFromSession()
        {
            var model = GetList(false);
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.Message = "Please enter movie information...";
            var directors = db.Directors.ToList().Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name + " " + e.Surname
            }).ToList();
            ViewData["directors"] = new MultiSelectList(directors, "Value", "Text");
            return View();
            //return new ViewResult();
        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name, int ProductionYear, string BoxOfficeReturn, List<int> Directors)
        {
            var entity = new Movie()
            {
                Id = 0,
                Name = Name,
                ProductionYear = ProductionYear.ToString(),
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture), 
                //MovieDirectors = new List<MovieDirector>()
            };
            entity.MovieDirectors = Directors.Select(e => new MovieDirector()
            {
                MovieId = entity.Id,
                DirectorId = e
            }).ToList();
            db.Movies.Add(entity);
            db.SaveChanges();
            Debug.WriteLine("Added Entity Id: " + entity.Id);
            TempData["Info"] = "Record successfully saved to database.";
            return RedirectToAction("Index");
            
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            }
            var model = db.Movies.Find(id.Value);
            //return View(model);
            List<SelectListItem> years = new List<SelectListItem>();
            SelectListItem year;
            for (int i = DateTime.Now.Year; i >= 1980 ; i--)
            {
                year = new SelectListItem() { Value = i.ToString(), Text = i.ToString() };
                years.Add(year);
            }
            ViewBag.Years = new SelectList(years, "Value", "Text", model.ProductionYear);
            var directors = db.Directors.Select(e => new DirectorModel(){
                Id = e.Id,
                FullName = e.Name + " " + e.Surname
            }).ToList();
            var directorsIds = model.MovieDirectors.Select(e => e.DirectorId).ToList();
            ViewBag.directors = new MultiSelectList(directors, "Id", "FullName", directorsIds);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "Id, Name, ProductionYear")] Movie movie, string BoxOfficeReturn, List<int> directorIds)
        {
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            entity.BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture);
            entity.MovieDirectors = new List<MovieDirector>();
            var movieDirectors = db.MovieDirectors.Where(e => e.MovieId == movie.Id).ToList();
            foreach (var movieDirector in movieDirectors)
            {
                db.MovieDirectors.Remove(movieDirector);
            }

            foreach (var directorId in directorIds)
            {
                var movieDirector = new MovieDirector()
                {
                    MovieId = movie.Id,
                    DirectorId = directorId
                };
                entity.MovieDirectors.Add(movieDirector);
            }
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Info"] = "Record successfully updated in database";
            return RedirectToRoute(new { controller = "Movies", action = "Index" });
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            }
            var model = db.Movies.FirstOrDefault(e => e.Id == id.Value);
            return View(model);
        }

        [ActionName("Delete")]
        [HttpPost]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            }
            var entity = db.Movies.Find(id);
            db.Movies.Remove(entity);
            db.SaveChanges();
            TempData["Info"] = "Record successfully deleted in database";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            }
            var model = db.Movies.Find(id.Value);
            return View(model);
        }

        public ActionResult Welcome()
        {
            var result = "Welcome to Movies MVC";
            return PartialView("_Welcome", result);
        }
    }
}