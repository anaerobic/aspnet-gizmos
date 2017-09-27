using AspNetGizmos.Models;
using System.Collections.Generic;

namespace AspNetGizmos.Tests
{
  internal class GizmoComparer : IEqualityComparer<Gizmo>
  {
    public bool Equals(Gizmo x, Gizmo y)
    {
      if (x == null || y == null)
      {
        return false;
      }

      return x.Id == y.Id
        && x.Name == y.Name
        && x.Rank == y.Rank;
    }

    public int GetHashCode(Gizmo obj)
    {
      var address = obj.Name.GetHashCode() ^ obj.Rank;

      return address.GetHashCode();
    }
  }
}