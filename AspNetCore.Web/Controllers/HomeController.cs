using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore.Web.Models;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Net.Http;
using System.Net;

namespace AspNetCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _cache;
        public HomeController(ILogger<HomeController> logger, IMemoryCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <returns></returns>
        [RequestSizeLimit(1_073_741_824)]
        public IActionResult Upload()
        {
            var url = "http://localhost:9001/Api/Default/Upload";

            var data = new MultipartFormDataContent();
            if (Request.HasFormContentType)
            {
                var request = Request.Form.Files;
                foreach (var item in request)
                {
                    data.Add(new StreamContent(item.OpenReadStream()), item.Name, item.FileName);
                }

                foreach (var item in Request.Form)
                {
                    data.Add(new StringContent(item.Value), item.Key);
                }
            }
            string jsonString = string.Empty;
            using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip }))
            {
                var taskResponse = client.PostAsync(url, data);
                taskResponse.Wait();
                if (taskResponse.IsCompletedSuccessfully)
                {
                    var taskStream = taskResponse.Result.Content.ReadAsStreamAsync();
                    taskStream.Wait();
                    using (var reader = new StreamReader(taskStream.Result))
                    {
                        jsonString = reader.ReadToEnd();
                    }
                }
            }
            return new JsonResult(jsonString);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(1_073_741_824)]
        public async Task<IActionResult> Save()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var form = await Request.ReadFormAsync();
            int saveCount = 0;
            long totalCount = form.Files.Sum(x => x.Length);
            foreach (var item in form.Files)
            {
                var fileSavePath = Environment.CurrentDirectory + "\\Files\\" + item.Name;
                using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        using (BinaryReader br = new BinaryReader(item.OpenReadStream()))
                        {
                            var customReadLength = item.Length;
                            byte[] buffer = new byte[customReadLength];
                            int readCount = 0;
                            while ((readCount = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, readCount);
                                saveCount += readCount;
                                var progress = (saveCount * 1.0 / totalCount).ToString("0.00");
                                _cache.Set<string>("UploadSpeed", progress, DateTimeOffset.Now.AddMinutes(60));
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }
            sw.Stop();
            return new JsonResult($"Read {string.Join(Environment.NewLine, Request.Form.Files.Select(x => x.FileName))} Success !耗时：{sw.ElapsedMilliseconds}");
        }

        /// <summary>
        /// 读取进度
        /// </summary>
        /// <returns></returns>
        public IActionResult UploadProgress()
        {
            var progress = _cache.Get<string>("UploadSpeed");
            return Json(progress);
        }
    }
}
