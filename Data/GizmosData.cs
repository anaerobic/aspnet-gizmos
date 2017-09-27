using AspNetGizmos.Models;
using System.Collections.Generic;

namespace AspNetGizmos.Data
{
  public static class GizmosData
  {
    public static IEnumerable<Gizmo> Get()
    {
      return new List<Gizmo>
      {
        new Gizmo { Name = "Foo", Rank = 0 },
        new Gizmo { Name = "Bar", Rank = 1 },
      };
    }
  }
}
