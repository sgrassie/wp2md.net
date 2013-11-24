using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Dynamic;

namespace wp2md
{
    struct PostType
    {
        public const string Post = "post";
        public const string Page = "page"; 
    }
    class Application
    {
        private Options _options;
        private Action<string> _logger;

        public Application(Action<string> logger)
        {
            _logger = logger;
        }

        public void Execute(Options options)
        {
            _options = options;

            if (_options.Verbose)
                _logger("Processing source file...");

            ProcessSourceFile();
        }

        private void ProcessSourceFile()
        {
            if(File.Exists(_options.SourceFile))
            {
                var document = XDocument.Load(_options.SourceFile, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
                var posts = GetPosts(document);
                var pages = GetPages(document);
            }
            else
            {
                if (_options.Verbose)
                    _logger(string.Format("Source file '{0}' does not exit. Aborting...", _options.SourceFile));

                Environment.Exit(-2);
            }
        }

        private List<Item> GetPosts(XDocument document)
        {
            if (_options.Verbose) _logger("Loading posts...");
            var posts = document.Root()
                        .GetItemByType(PostType.Post)
                        .WithoutStatus("draft")
                        .SelectItems()
                        .ToList();

            if (_options.Verbose) _logger(string.Format("Found {0} posts.", posts.Count));
            return posts;
        }

        private List<Item> GetPages(XDocument document)
        {
            if (_options.Verbose) _logger("Loading pages...");
            var pages = document.Root().GetItemByType(PostType.Page).WithoutStatus("draft").SelectItems().ToList();

            if (_options.Verbose) _logger(string.Format("Found {0} pages.", pages.Count));
            return pages;
        }
    }

    static class Extensions
    {
        private static XNamespace excerpt = "http://wordpress.org/export/1.2/excerpt/";
        private static XNamespace content = "http://purl.org/rss/1.0/modules/content/";
        private static XNamespace dc = "http://purl.org/dc/elements/1.1/";
        private static XNamespace wp = "http://wordpress.org/export/1.2/";

        public static IQueryable<XElement> Root(this XDocument document)
        {
            return document.Root.Element("channel").Elements("item").AsQueryable();
        }

        public static IQueryable<XElement> GetItemByType(this IQueryable<XElement> queryable, string postType)
        {
            return queryable.Where(item => item.Element(wp + "post_type").Value == postType);
        }
        public static IQueryable<XElement> WithoutStatus(this IQueryable<XElement> queryable, string status)
        {
            return queryable.Where(item => item.Element(wp + "status").Value != status);
        }

        public static IQueryable<Item> SelectItems(this IQueryable<XElement> items)
        {
            return items.Select(item =>
                new Item
                {
                    Title = item.Element("title").Value,
                    PublicationDate = DateTime.Parse(item.Element("pubDate").Value),
                    Author = item.Element(dc + "creator").Value,
                    Guid = item.Element("guid").Value,
                    Description = item.Element("description").Value,
                    Content = item.Element(content + "encoded").Value,
                    Id = Convert.ToInt32(item.Element(wp + "post_id").Value),
                    PostDate = DateTime.Parse(item.Element(wp + "post_date").Value),
                    PostDateGmt = DateTime.Parse(item.Element(wp + "post_date_gmt").Value),
                    CommentStatus = item.Element(wp + "comment_status").Value,
                    PingStatus = item.Element(wp + "ping_status").Value,
                    PostName = item.Element(wp + "post_name").Value,
                    PostParent = item.Element(wp + "post_parent").Value,
                    PostType = item.Element(wp + "post_type").Value,
                    Categories = item.Elements("category").Select(el => el.Value).ToList(),
                    PostMeta = (from meta in item.Elements(wp + "postmeta")
                                select new Tuple<string, string>(meta.Element(wp + "meta_key").Value, meta.Element(wp + "meta_value").Value)
                               ).ToList()
                });
        }
    }
}