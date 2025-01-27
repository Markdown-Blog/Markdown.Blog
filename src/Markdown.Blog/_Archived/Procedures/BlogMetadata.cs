//using System;
//using System.Collections.Generic;
//using Markdown.Blog.Shared;
//using Newtonsoft.Json;

//namespace Markdown.Blog.Procedures
//{
//    /// <summary>
//    /// Provides functionality for handling blog metadata operations
//    /// </summary>
//    public static class BlogMetadataProcessor
//    {
//        /// <summary>
//        /// Deserializes a JSON string into a list of BlogMetadata objects
//        /// </summary>
//        /// <param name="jsonContent">The JSON string to deserialize</param>
//        /// <returns>A list of BlogMetadata objects</returns>
//        /// <exception cref="InvalidOperationException">Thrown when deserialization fails</exception>
//        public static List<BlogMetadata> DeserializeMetadata(string jsonContent)
//        {
//            try
//            {
//                return JsonConvert.DeserializeObject<List<BlogMetadata>>(jsonContent)!;
//            }
//            catch (JsonException ex)
//            {
//                throw new InvalidOperationException("Failed to deserialize blog metadata JSON content", ex);
//            }
//        }
//    }
//}
