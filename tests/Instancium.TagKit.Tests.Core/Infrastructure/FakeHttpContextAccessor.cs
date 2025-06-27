using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }

        public FakeHttpContextAccessor(string langCode)
        {
            var ctx = new DefaultHttpContext();
            ctx.Request.Headers["X-Language-Code"] = langCode;
            HttpContext = ctx;
        }
    }

}
