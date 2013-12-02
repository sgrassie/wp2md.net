using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace wp2md
{
    public interface IVisitor
    {
        void Visit(Parser visitable);
    }

    public class PostVisitor : IVisitor
    {
        private Options _options;

        public void Visit(Parser visitable)
        {
            _options = visitable.Options;
            var posts = visitable.Items.Where(x => x.PostType == PostType.Post);
            
            foreach(var post in posts)
            {
                WriteMarkdownFile(post);
            }
        }

        /// <summary>
        /// Actually write the markdown file.
        /// </summary>
        /// <param name="item">The post to write.</param>
        /// TODO: Probably move this to a base class.
        private void WriteMarkdownFile(Item item)
        {
            using(var stream = new FileStream(BuildFileNameForItem(item), FileMode.Create, FileAccess.Write))
            using(var writer = new StreamWriter(stream, Encoding.Default))
            {
                WriteHeader(writer, item);
                WriteContent(writer, item);
            }
        }

        private string BuildFileNameForItem(Item item)
        {
            var title = !string.IsNullOrEmpty(item.PostName) ? item.PostName : item.Title.ToLower()
                .Replace(" ", "-").Replace(":", "").Replace("!", "").Replace(",", "");

            // year-month-day-title
            var template = "{0}-{1}-{2}-{3}";
            var formatted = string.Format(template, item.PostDate.Year, item.PostDate.Month, item.PostDate.Day, title);
            return Path.ChangeExtension(Path.Combine(_options.OutputPath, formatted), ".md");
        }

        /// <summary>
        /// Write the header information.
        /// </summary>
        /// <remarks>
        /// See <a href="https://github.com/Sandra/Sandra.Snow/wiki/Markdown-File-Header">Markdown-File-Header</a> spec for Sandra.Snow
        /// </remarks>
        /// <param name="writer"></param>
        /// <param name="item"></param>
        private void WriteHeader(StreamWriter writer, Item item)
        {
            writer.WriteLine("---"); // Start header

            writer.WriteLine("title: {0}", item.Title);
            writer.WriteLine("layout: {0}", item.PostType);

            string[] categories = item.Categories.ToArray();
            var joined = string.Join(",", categories);
            writer.WriteLine("category: {0}", joined);

            writer.WriteLine("metadescription: {0}", item.PostName);

            writer.WriteLine("---"); // End header
        }

        private void WriteContent(StreamWriter writer, Item item)
        {
            writer.WriteLine(item.Content);
        }
    }
}
