using Microsoft.AspNetCore.Mvc.Rendering;
using PhenomenologicalStudy.API.Models.ManyToMany;
using System.Collections.Generic;

namespace PhenomenologicalStudy.Web.Models.ViewModels
{
  public class ReflectionChildViewModel
  {
    public IEnumerable<SelectListItem> Reflections { get; set; }
    public ReflectionChild Child { get; set; }
  }
}
