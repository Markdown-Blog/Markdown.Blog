using System;
using System.Collections.Generic;
using System.Text.Json;
using Markdown.Blog.Shared.Models;

namespace Markdown.Blog.Infrastructure.Utilities
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
                return JsonSerializer.Deserialize<BlogMetadata>(json) 
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
                return JsonSerializer.Deserialize<List<BlogMetadata>>(json) 
                    ?? new List<BlogMetadata>();
            }
            catch (JsonException)
            {
                return new List<BlogMetadata>();
            }
        }

        /// <summary>
        /// Deserializes a JSON string into a generic type
        /// </summary>
        public static T? DeserializeFromJson<T>(string json) where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Serializes an object to JSON
        /// </summary>
        public static string SerializeToJson<T>(T obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (JsonException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Serializes a BlogMetadata object to JSON
        /// </summary>
        public static string ToJson(BlogMetadata metadata)
        {
            return SerializeToJson(metadata);
        }

        /// <summary>
        /// Serializes a list of BlogMetadata objects to JSON
        /// </summary>
        public static string ToJson(List<BlogMetadata> metadataList)
        {
            return SerializeToJson(metadataList);
        }
    }
}