using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhenomenologicalStudy.API.Data;
using PhenomenologicalStudy.API.Models;
using PhenomenologicalStudy.Web.Models.ViewModels.Home;
using System.Threading.Tasks;

namespace PhenomenologicalStudy.Web.Controllers
{
  [Authorize("Admin")]
  public class HomeController : Controller
  {
    private readonly PhenomenologicalStudyContext _db;
    public HomeController(PhenomenologicalStudyContext db)
    {
      _db = db;
    }
    public async Task<IActionResult> Index()
    {
      IndexViewModel reflectionsAndChildren = new()
      {
        Reflections = await _db.Reflections.ToListAsync(),
        Children = await _db.Children.ToListAsync()
      };
      return View(reflectionsAndChildren);
    }
  }
}
