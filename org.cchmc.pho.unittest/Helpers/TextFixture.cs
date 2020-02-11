using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace org.cchmc.pho.unittest.Helpers
{
    public class TestFixture<TStartup> : IDisposable where TStartup : class
    {
        public readonly TestServer Server;
        private readonly HttpClient _client;

        public TestFixture()
        {
            var contentRootPath = Directory.GetCurrentDirectory();
            var builder = new WebHostBuilder()
                .UseContentRoot(contentRootPath + $"\\..\\..\\..\\..\\org.cchmc.pho.api\\")
                .UseStartup<TStartup>();

            Server = new TestServer(builder);
            _client = new HttpClient();
        }


        public void Dispose()
        {
            _client.Dispose();
            Server.Dispose();
        }
    }
}
