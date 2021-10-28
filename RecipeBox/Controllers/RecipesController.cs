using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using RecipeBox.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecipeBox.Controllers {
  [Authorize]
  public class RecipesController: Controller {
    private readonly RecipeBoxContext _db;
    private readonly UserManager <ApplicationUser> _userManager;

    public RecipesController(UserManager <ApplicationUser> userManager, RecipeBoxContext db) {
      _userManager = userManager;
      _db = db;
    }
    [AllowAnonymous]
    public ActionResult Index(string searchString) {
      IQueryable<Recipe> model = _db.Recipes.OrderByDescending(rate => rate.Rating);
      if(!string.IsNullOrEmpty(searchString))
      {
        model = model.Where(rate => rate.Ingredients.Contains(searchString)); 
      }
      return View(model.ToList());
    }

    public ActionResult Create() {
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View();
    }

    [HttpPost]
    public async Task <ActionResult> Create(Recipe Recipe, int CategoryId) {
      var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var currentUser = await _userManager.FindByIdAsync(userId);
      Recipe.User = currentUser;
      _db.Recipes.Add(Recipe);
      _db.SaveChanges();
      if (CategoryId != 0) {
        _db.CategoryRecipe.Add(new CategoryRecipe() {
          CategoryId = CategoryId, RecipeId = Recipe.RecipeId
        });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    [AllowAnonymous]
    public ActionResult Details(int id) {
      var thisRecipe = _db.Recipes
        .Include(Recipe => Recipe.JoinEntities)
        .ThenInclude(join => join.Category)
        .FirstOrDefault(Recipe => Recipe.RecipeId == id);
      return View(thisRecipe);
    }

    public ActionResult Edit(int id) {
      var thisRecipe = _db.Recipes.FirstOrDefault(Recipe => Recipe.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult Edit(Recipe Recipe, int CategoryId) {
      if (CategoryId != 0) {
        _db.CategoryRecipe.Add(new CategoryRecipe() {
          CategoryId = CategoryId, RecipeId = Recipe.RecipeId
        });
      }
      _db.Entry(Recipe).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult AddCategory(int id) {
      var thisRecipe = _db.Recipes.FirstOrDefault(Recipe => Recipe.RecipeId == id);
      ViewBag.CategoryId = new SelectList(_db.Categories, "CategoryId", "Name");
      return View(thisRecipe);
    }

    [HttpPost]
    public ActionResult AddCategory(Recipe Recipe, int CategoryId) {
      if (CategoryId != 0) {
        _db.CategoryRecipe.Add(new CategoryRecipe() {
          CategoryId = CategoryId, RecipeId = Recipe.RecipeId
        });
      }
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    public ActionResult Delete(int id) {
      var thisRecipe = _db.Recipes.FirstOrDefault(Recipe => Recipe.RecipeId == id);
      return View(thisRecipe);
    }

    [HttpPost, ActionName("Delete")]
    public ActionResult DeleteConfirmed(int id) {
      var thisRecipe = _db.Recipes.FirstOrDefault(Recipe => Recipe.RecipeId == id);
      _db.Recipes.Remove(thisRecipe);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult DeleteCategory(int joinId) {
      var joinEntry = _db.CategoryRecipe.FirstOrDefault(entry => entry.CategoryRecipeId == joinId);
      _db.CategoryRecipe.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}