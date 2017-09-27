using AspNetGizmos.Controllers;
using AspNetGizmos.Data;
using AspNetGizmos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetGizmos.Tests
{
  public class GizmosControllerShould
  {
    readonly GizmoContext _ctx;

    public GizmosControllerShould()
    {
      var builder = new DbContextOptionsBuilder<GizmoContext>()
          .UseInMemoryDatabase("Gizmos");
      var context = new GizmoContext(builder.Options);

      if (!context.Gizmos.Any())
      {
        context.AddRange(GizmosData.Get());
        context.SaveChanges();
      }

      _ctx = context;
    }

    [Fact]
    public async Task GetOverall()
    {
      var sut = new GizmosController(_ctx);

      var actual = await sut.GetAll();

      Assert.Equal(_ctx.Gizmos.Count(), actual.Count());

      GizmosUtils.AssertRanks(actual);
    }
    
    [Theory]
    [InlineData(1, "Foo")]
    [InlineData(2, "Bar")]
    public void ReturnOne(long id, string name)
    {
      var sut = new GizmosController(_ctx);

      var actual = sut.GetById(id);

      Assert.Equal(typeof(ObjectResult), actual.GetType());

      var objectResult = actual as ObjectResult;

      var gizmo = objectResult.Value as Gizmo;

      Assert.Equal(typeof(Gizmo), gizmo.GetType());

      Assert.Equal(name, gizmo.Name);
    }

    [Fact]
    public void ReturnOneNotFound()
    {
      var sut = new GizmosController(_ctx);

      var actual = sut.GetById(0);

      Assert.Equal(typeof(NotFoundResult), actual.GetType());
    }

    [Fact]
    public async Task Create()
    {
      var sut = new GizmosController(_ctx);

      var maxId = _ctx.Gizmos.Max(r => r.Id);

      var maxRank = _ctx.Gizmos.Max(r => r.Rank);

      var gizmo = new Gizmo { Name = "Baz", Rank = maxRank + 1 };

      var actual = await sut.Create(gizmo);

      Assert.Equal(typeof(CreatedAtRouteResult), actual.GetType());

      var createdResult = actual as CreatedAtRouteResult;

      // endpoint returns gizmo
      Assert.Same(gizmo, createdResult.Value as Gizmo);

      // endpoint assigns next id to gizmo
      Assert.True(gizmo.Id > maxId);
      Assert.Equal(gizmo.Id, _ctx.Gizmos.Max(r => r.Id));

      // endpoint returns correct id in route values
      Assert.Equal(gizmo.Id, createdResult.RouteValues["id"]);
    }

    [Fact]
    public async Task CreateBadRequestNull()
    {
      var sut = new GizmosController(_ctx);

      var actual = await sut.Create(null);

      Assert.Equal(typeof(BadRequestResult), actual.GetType());
    }

    [Fact]
    public async Task Update()
    {
      var sut = new GizmosController(_ctx);

      var gizmo = _ctx.Gizmos.Last();

      gizmo.Name = "Bar";

      var actual = await sut.Update(gizmo.Id, gizmo);

      Assert.Equal(typeof(NoContentResult), actual.GetType());
      
      var result1 = _ctx.Gizmos.Single(x => x.Id == gizmo.Id);

      Assert.Equal("Bar", result1.Name);
    }

    [Fact]
    public async Task UpdateBadRequestNull()
    {
      var sut = new GizmosController(_ctx);

      var actual = await sut.Update(0, null);

      Assert.Equal(typeof(BadRequestResult), actual.GetType());
    }

    [Fact]
    public async Task UpdateBadRequestIdsNotMatch()
    {
      var sut = new GizmosController(_ctx);

      var actual = await sut.Update(1, new Gizmo { Id = 0 });

      Assert.Equal(typeof(BadRequestResult), actual.GetType());
    }

    [Fact]
    public async Task UpdateNotFound()
    {
      var sut = new GizmosController(_ctx);

      var actual = await sut.Update(0, new Gizmo { Id = 0 });

      Assert.Equal(typeof(NotFoundResult), actual.GetType());
    }

    [Fact]
    public async Task Delete()
    {
      var sut = new GizmosController(_ctx);

      var gizmo = _ctx.Gizmos.Last();

      var actual = await sut.Delete(gizmo.Id);

      Assert.Equal(typeof(NoContentResult), actual.GetType());

      Assert.Null(_ctx.Gizmos.SingleOrDefault(x => x.Id == gizmo.Id));
    }

    [Fact]
    public async Task DeleteNotFound()
    {
      var sut = new GizmosController(_ctx);

      var maxId = _ctx.Gizmos.Max(x => x.Id);

      var actual = await sut.Delete(maxId + 1);

      Assert.Equal(typeof(NotFoundResult), actual.GetType());
    }
  }
}
