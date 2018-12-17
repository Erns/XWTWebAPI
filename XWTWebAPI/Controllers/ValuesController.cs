using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using XWTWebAPI.Classes;

namespace XWTWebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values (READ)
        public IEnumerable<string> Get()
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return new string[] { "validation Fail" }; 
            }

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5 (READ)
        public string Get(int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                return "Validation fail";
            }

            return "value";
        }

        // POST api/values (CREATE)
        public void Post([FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                //return "Validation fail";
            }
        }

        // PUT api/values/5 (UPDATE)
        public void Put(int id, [FromBody]string value)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                //return "Validation fail";
            }
        }

        // DELETE api/values/5 (DELETE)
        public void Delete(int id)
        {
            if (!Utilities.IsValidated(Request.Headers))
            {
                //return "Validation fail";
            }
        }
    }
}
