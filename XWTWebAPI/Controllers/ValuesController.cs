using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace XWTWebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 (READ)
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values (CREATE)
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5 (UPDATE)
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
        }
    }
}
