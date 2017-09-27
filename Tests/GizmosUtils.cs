using AspNetGizmos.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AspNetGizmos.Tests
{
  public static class GizmosUtils
  {
    public static void AssertRanks(IEnumerable<Gizmo> gizmos)
    {
      Assert.True(gizmos.Any());

      for (int i = 0; i < gizmos.Count() - 1; i++)
      {
        Assert.True(gizmos.ElementAt(i).Rank <= gizmos.ElementAt(i + 1).Rank);
      }
    }
  }
}
