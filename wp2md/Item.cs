using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp2md
{
    class Item
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Author { get; set; }

        // Not to be confused with a .net guid
        public string Guid { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string Excerpt { get; set; }
        public int Id { get; set; }
        public DateTime PostDate { get; set; }
        public DateTime PostDateGmt { get; set; }
        public string CommentStatus { get; set; }
        public string PingStatus { get; set; }
        public string PostName { get; set; }
        public string PostParent { get; set; }
        public string PostType { get; set; }
        public List<string> Categories { get; set; }
        public List<Tuple<string, string>> PostMeta { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
