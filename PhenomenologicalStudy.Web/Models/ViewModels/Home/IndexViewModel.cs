using System.Collections.Generic;

namespace PhenomenologicalStudy.Web.Models.ViewModels.Home
{
  public class IndexViewModel
  {
    public IEnumerable<PhenomenologicalStudy.API.Models.Reflection> Reflections { get; set; }
    public IEnumerable<PhenomenologicalStudy.API.Models.Child> Children { get; set; }
  }
}
