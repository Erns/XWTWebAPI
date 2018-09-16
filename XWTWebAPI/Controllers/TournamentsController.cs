using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XWTWebAPI.Controllers
{
    public class TournamentsController : ApiController
    {
        // GET api/values
        public string Get()
        {
            return "";
        }

        // GET api/values/5 (READ)
        public string Get(int id)
        {

            return "";
        }

        // POST api/values (CREATE)
        public string Post([FromBody]string value)
        {
            return "";
        }

        // PUT api/values/5 (UPDATE)
        public string Put(int id, [FromBody]string value)
        {
            return "";
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
        }

    }
}
