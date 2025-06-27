using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public class FakeLocalizer : IStringLocalizer
    {
        public LocalizedString this[string name] => new(name, $"[{name}]", true);
        public LocalizedString this[string name, params object[] arguments] => this[name];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            Enumerable.Empty<LocalizedString>();

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }

}
