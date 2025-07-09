using System.Security.Cryptography;
using System.Text;

namespace Instancium.TagKit.Core.Rendering
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
