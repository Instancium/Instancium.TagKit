using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Core.Core
{
    public class ResourceHash
    {
        public static string FromContent(string content)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToHexString(hash[..6]).ToLower();
        }
    }
}
