//using System;
//using System.Collections.Generic;
//using Markdown.Blog.Client;
//using Newtonsoft.Json;

//namespace Markdown.Blog.Procedures
//{
//    /// <summary>
//    /// Provides functionality for handling blog metadata operations
//    /// </summary>
//    public static class BlogDataProcessor
//    {
//        public static List<BlogData> GetBlogDataListFromIndexJson(Division division, string jsonContent)
//        {
//            var blogMetadataList =   BlogMetadataProcessor.DeserializeMetadata(jsonContent);
//            var blogDataList = new List<BlogData>();
//            foreach (var blogMetadata in blogMetadataList)
//            {
//                blogDataList.Add(new BlogData(division, blogMetadata));
//            }
//            return blogDataList;
//        }
//    }
//}
