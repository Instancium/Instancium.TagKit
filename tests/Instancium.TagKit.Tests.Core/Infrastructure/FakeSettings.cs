using Instancium.TagKit.Core.Config;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public static class FakeAppSettings
    {
        public static IOptions<AppSettings> Instance => new FakeOptions<AppSettings>(new AppSettings
        {
            BaseUrl = "/"
        });
    }

}
