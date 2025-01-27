using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Markdown.Blog.Shared
{
    /// <summary>
    /// Handles deserialization of blog metadata
    /// </summary>
    public static class BlogMetadataDeserializer
    {
        /// <summary>
        /// Deserializes a JSON string into a BlogMetadata object
        /// </summary>
        public static BlogMetadata FromJson(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<BlogMetadata>(json) 
                    ?? new BlogMetadata();
            }
            catch (JsonException)
            {
                return new BlogMetadata();
            }
        }

        /// <summary>
        /// Deserializes a JSON string into a list of BlogMetadata objects
        /// </summary>
        public static List<BlogMetadata> FromJsonArray(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<BlogMetadata>>(json) 
                    ?? new List<BlogMetadata>();
            }
            catch (JsonException)
            {
                return new List<BlogMetadata>();
            }
        }

        /// <summary>
        /// Serializes a BlogMetadata object to JSON
        /// </summary>
        public static string ToJson(BlogMetadata metadata)
        {
            return JsonConvert.SerializeObject(metadata, Formatting.Indented);
        }

        /// <summary>
        /// Serializes a list of BlogMetadata objects to JSON
        /// </summary>
        public static string ToJson(List<BlogMetadata> metadataList)
        {
            return JsonConvert.SerializeObject(metadataList, Formatting.Indented);
        }
    }
} 