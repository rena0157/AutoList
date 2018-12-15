using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;

// Import Core from AutoList
using AutoList.Core;
using System.Linq;

namespace AutoList.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AutoListApi : ControllerBase
    {

        /// <summary>
        /// Read the body from an HttpPost
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <returns>The body of the request as a string</returns>
        private static string ReadBody(HttpContext context)
        {
            // Return string
            string bodyString;
            var request = context.Request;

            // So we can rewind the StreamReader after we are done
            request.EnableRewind();

            using (var reader
                = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyString = reader.ReadToEnd();
            }

            // Rewind the Stream Reader
            request.Body.Position = 0;
            return bodyString;
        }


        [HttpPost]
        public string SayHello()
        {
            var name = ReadBody(HttpContext);
            return $"Hello: {name}";
        }

        [HttpPost]
        public double GetTotalLength() => AutoListParser
            .GetDouble(ReadBody(HttpContext), AutoListPatterns.LinesLengthPattern)
            .Sum();

        [HttpPost]
        public double GetTotalArea() => AutoListParser
            .GetDouble(ReadBody(HttpContext), AutoListPatterns.HatchAreaPattern)
            .Sum();

        [HttpPost]
        public string GetBlocksJson() => AutoListParser
            .GetBlocks(ReadBody(HttpContext), ExportOptions.Json);

        [HttpPost]
        public string GetBlocksCsv() => AutoListParser
            .GetBlocks(ReadBody(HttpContext), ExportOptions.Csv);
    }
}
