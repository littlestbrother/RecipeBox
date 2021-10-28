using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;

namespace RecipeBox.Controllers {
  public class CategoriesController: Controller {
    private readonly RecipeBoxContext _db;

    public CategoriesController(RecipeBoxContext db) {
      _db = db;
    }

    public ActionResult Index() {
      List <Category> model = _db.Categories.ToList();
      return View(model);
    }

    public ActionResult Create() {
      return View();
    }

    [HttpPost]
    public ActionResult Create(Category Category) {
      _db.Categories.Add(Category);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Details(int id) {
      var thisCategory = _db.Categories
        .Include(Category => Category.JoinEntities)
        .ThenInclude(join => join.Recipe)
        .FirstOrDefault(Category => Category.CategoryId == id);
      return View(thisCategory);
    }
    public ActionResult Edit(int id) {
      var thisCategory = _db.Categories.FirstOrDefault(Category => Category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost]
    public ActionResult Edit(Category Category) {
      _db.Entry(Category).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id) {
      var thisCategory = _db.Categories.FirstOrDefault(Category => Category.CategoryId == id);
      return View(thisCategory);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id) {
      var thisCategory = _db.Categories.FirstOrDefault(Category => Category.CategoryId == id);
      _db.Categories.Remove(thisCategory);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}