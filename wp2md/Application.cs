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
                new Parser().Parse(document);
            }
            else
            {
                if (_options.Verbose)
                    _logger(string.Format("Source file '{0}' does not exit. Aborting...", _options.SourceFile));

                Environment.Exit(-2);
            }
        }

        private Task ConvertToMarkdown(List<Item> posts)
        {
            throw new NotImplementedException();
        }
    }

    public class Parser 
    {
        private XDocument _document;
        private IEnumerable<Item> _items;
        private static XNamespace excerpt = "http://wordpress.org/export/1.2/excerpt/";
        private static XNamespace content = "http://purl.org/rss/1.0/modules/content/";
        private static XNamespace dc = "http://purl.org/dc/elements/1.1/";
        private static XNamespace wp = "http://wordpress.org/export/1.2/";

        public void Parse(XDocument document)
        {
            _document = document;

            _items = (from item in _document.Root.Element("channel").Elements("item")
                         select 
                            new Item
                            {
                                Title = item.Element("title").Value,
                                PublicationDate = ParseDateTime(item.Element("pubDate").Value),
                                Author = item.Element(dc + "creator").Value,
                                Guid = item.Element("guid").Value,
                                Description = item.Element("description").Value,
                                Content = item.Element(content + "encoded").Value,
                                Id = Convert.ToInt32(item.Element(wp + "post_id").Value),
                                PostDate = ParseDateTime(item.Element(wp + "post_date").Value),
                                PostDateGmt = ParseDateTime(item.Element(wp + "post_date_gmt").Value),
                                CommentStatus = item.Element(wp + "comment_status").Value,
                                PingStatus = item.Element(wp + "ping_status").Value,
                                PostName = item.Element(wp + "post_name").Value,
                                PostParent = item.Element(wp + "post_parent").Value,
                                PostType = item.Element(wp + "post_type").Value,
                                Categories = item.Elements("category").Select(el => el.Value).ToList(),
                                PostMeta = (from meta in item.Elements(wp + "postmeta")
                                            select new Tuple<string, string>(meta.Element(wp + "meta_key").Value, meta.Element(wp + "meta_value").Value)
                                           ).ToList()
                            }).ToList();
        }

        private DateTime ParseDateTime(string date)
        {
            try
            {
                return DateTime.Parse(date);
            }
            catch(FormatException)
            {
                return DateTime.MinValue;
            }
        }
    }
}