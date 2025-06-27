using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public class FakeOptions<T> : IOptions<T> where T : class
    {
        public T Value { get; }
        public FakeOptions(T value) => Value = value;
    }

}
