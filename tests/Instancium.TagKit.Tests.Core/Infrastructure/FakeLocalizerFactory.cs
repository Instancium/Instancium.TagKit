using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public class FakeLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource) => new FakeLocalizer();
        public IStringLocalizer Create(string baseName, string location) => new FakeLocalizer();
    }

}
