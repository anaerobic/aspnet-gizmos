using AspNetGizmos.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AspNetGizmos.Tests
{
  public class GizmosRequestShould : RequestShould
  {
    [Fact]
    public async Task ReturnOverall()
    {
      var gizmos = await GetAll();

      GizmosUtils.AssertRanks(gizmos);
    }

    [Fact]
    public async Task ReturnOne()
    {
      var gizmos = await GetAll();

      foreach (var gizmo in gizmos)
      {
        var actual = await GetById(gizmo.Id);

        Assert.Equal(gizmo, actual, new GizmoComparer ());
      }
    }

    [Fact]
    public async Task Create()
    {
      var gizmo = new Gizmo
      {
        Name = "Bat",
        Rank = -1
      };

      var response = await _client.PostAsync("/api/Gizmos/",
        new StringContent(JsonConvert.SerializeObject(gizmo),
        Encoding.UTF8, "application/json"));
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      
      var actual = JsonConvert.DeserializeObject<Gizmo>(responseString);
      
      Assert.Equal(gizmo.Name, actual.Name);
      Assert.Equal(gizmo.Rank, actual.Rank);

      Assert.True(actual.Id > 0);
    }

    [Fact]
    public async Task Update()
    {
      var gizmos = await GetAll();

      foreach (var gizmo in gizmos)
      {
        gizmo.Name += " " + DateTime.Now.Ticks;
        gizmo.Rank += 1000;

        var response = await _client.PutAsync($"/api/Gizmos/{gizmo.Id}",
          new StringContent(JsonConvert.SerializeObject(gizmo),
          Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(string.Empty, responseString);

        var updatedResult = await GetById(gizmo.Id);
        
        Assert.Equal(gizmo.Id, updatedResult.Id);
        Assert.Equal(gizmo.Name, updatedResult.Name);
        Assert.Equal(gizmo.Rank, updatedResult.Rank);
      }
    }

    [Fact]
    public async Task Delete()
    {
      var gizmos = await GetAll();

      foreach (var gizmo in gizmos)
      {
        var response = await _client.DeleteAsync($"/api/Gizmos/{gizmo.Id}");
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.Equal(string.Empty, responseString);

        var deleted = await GetById(gizmo.Id, false);

        Assert.Null(deleted);
      }
    }

    private async Task<Gizmo> GetById(long id, bool ensureSuccess = true)
    {
      var response = await _client.GetAsync($"/api/Gizmos/{id}");
      if (ensureSuccess) response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<Gizmo>(responseString);
    }

    private async Task<IEnumerable<Gizmo>> GetAll()
    {
      var response = await _client.GetAsync("/api/Gizmos/");
      response.EnsureSuccessStatusCode();

      var responseString = await response.Content.ReadAsStringAsync();
      
      return JsonConvert.DeserializeObject<IEnumerable<Gizmo>>(responseString);
    }
  }
}
