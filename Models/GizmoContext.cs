using Microsoft.EntityFrameworkCore;

namespace AspNetGizmos.Models
{
  public class GizmoContext : DbContext
  {
    public GizmoContext(DbContextOptions<GizmoContext> options)
        : base(options)
    {
    }

    public DbSet<Gizmo> Gizmos { get; set; }
  }
}
