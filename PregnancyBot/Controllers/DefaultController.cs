using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PregnancyBot.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody]string body)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}