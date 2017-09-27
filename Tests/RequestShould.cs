using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace AspNetGizmos.Tests
{
  public abstract class RequestShould
  {
    protected readonly TestServer _server;
    protected readonly HttpClient _client;

    protected RequestShould()
    {
      _server = new TestServer(new WebHostBuilder()
          .UseStartup<Startup>());
      _client = _server.CreateClient();
    }
  }
}
