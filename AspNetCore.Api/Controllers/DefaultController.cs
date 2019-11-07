using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public static UserInfo UserInfo = new UserInfo
        {
            Id = Guid.NewGuid(),
            Age = 23,
            Name = "Jon",
            Sex = true
        };

        [HttpGet]
        public IActionResult Test()
        {
            return new JsonResult("OK");
        }

        /// <summary>
        /// API GET
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<UserInfo> FormCall([FromForm]UserInfo user)
        {
            return new JsonResult($"FromForm Response {JsonSerializer.Serialize(user)}");
        }

        /// <summary>
        /// API GET
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<UserInfo> BodyCall([FromBody]UserInfo user)
        {
            return new JsonResult($"FromBody Response {JsonSerializer.Serialize(user)}");
        }

        /// <summary>
        /// API POST
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<UserInfo> Upload()
        {
            var files = Request.Form.Files;
            return new JsonResult($"Read {string.Join(Environment.NewLine,files.Select(x=>x.FileName))} Success !");
        }
    }
}