using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using org.cchmc.pho.core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace org.cchmc.pho.api.Middleware
{
    /* Got this class from this blog post: https://itnext.io/log-requests-and-responses-in-asp-net-core-3-a1bebd49c996
     * GitHub source here: https://github.com/elanderson/ASP.NET-Core-Basics-Refresh/commit/c1b24de0d44dfc45d379b91d721842656c4ba3d8
     * Made some modifications:
     *  - don't log anything if the path contains text from the custom options
     *  - add the username to the logged info
     * Although we _could_ log the header info, it will contain our token and let's not.
     */
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly List<string> _pathsNotToLog;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RecyclableMemoryStreamManager> logger, IOptions<CustomOptions> customOptions)
        {
            _next = next;
            _logger = logger;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _pathsNotToLog = customOptions.Value.DoNotLogMetaDataForPaths;
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            if (_pathsNotToLog != null && _pathsNotToLog.Any(p => context.Request.Path.Value.ToLower().Contains(p.ToLower())))
                return;

            // Need to buffer the request, otherwise reading it destroys it before it gets to the controller. 
            context.Request.EnableBuffering();

            string userName = "n/a"; // set to n/a so we don't log null or empty string for anonymous routes
            if (context.User != null && context.User.HasClaim(x => x.Type == ClaimTypes.Name))
                userName = context.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                                   $"UserName:{userName} " + 
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} " +
                                   $"Request Body: {ReadStreamInChunks(requestStream)}");

            // reset the position to 0 so the request is leaving this method in the same state it came in
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            if (_pathsNotToLog != null && _pathsNotToLog.Any(p => context.Request.Path.Value.ToLower().Contains(p.ToLower())))
            {
                await _next(context);
                return;
            }

            await _next(context);

            string userName = "n/a"; // set to n/a so we don't log null or empty string for anonymous routes
            if (context.User != null && context.User.HasClaim(x => x.Type == ClaimTypes.Name))
                userName = context.User.FindFirst(x => x.Type == ClaimTypes.Name).Value;

            _logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                                   $"UserName:{userName} " +
                                   $"StatusCode:{context.Response.StatusCode} " +
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} ");
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}
