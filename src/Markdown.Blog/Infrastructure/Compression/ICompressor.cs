namespace Markdown.Blog.Infrastructure.Compression
{
    /// <summary>
    /// Compression adapter interface.
    /// </summary>
    public interface ICompressor
    {
        /// <summary>
        /// Compress text content to binary payload.
        /// </summary>
        byte[] Compress(string content);

        /// <summary>
        /// Decompress binary payload to text content.
        /// </summary>
        string Decompress(byte[] binary);
    }
}