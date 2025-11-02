using System.IO;
using System.IO.Compression;
using System.Text;

namespace Markdown.Blog.Infrastructure.Compression
{
    /// <summary>
    /// GZip compressor implementation for text content.
    /// </summary>
    public class GzipCompressor : ICompressor
    {
        public byte[] Compress(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            using var input = new MemoryStream(bytes);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
            {
                input.CopyTo(gzip);
            }
            return output.ToArray();
        }

        public string Decompress(byte[] binary)
        {
            using var input = new MemoryStream(binary);
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            return reader.ReadToEnd();
        }
    }
}