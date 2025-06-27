using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instancium.TagKit.Tests.Core.Infrastructure
{
    public static class SnapshotTestUtils
    {
        private static readonly string SnapshotsPath = Path.Combine(AppContext.BaseDirectory, "snapshots");

        public static void AssertMatchesSnapshot(string fileName, string actualContent)
        {
            string path = Path.Combine(SnapshotsPath, fileName);
            if (!File.Exists(path))
            {
                // Авто-создание первого снапшота
                Directory.CreateDirectory(SnapshotsPath);
                File.WriteAllText(path, actualContent);
                throw new InvalidOperationException($"Snapshot created at {path}. Please verify and re-run the test.");
            }

            string expected = File.ReadAllText(path);
            Assert.Equal(NormalizeLineEndings(expected), NormalizeLineEndings(actualContent));
        }

        private static string NormalizeLineEndings(string text) =>
            text.Replace("\r\n", "\n").Trim();
    }

}
