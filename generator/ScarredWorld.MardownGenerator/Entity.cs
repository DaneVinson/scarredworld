using System;
using System.Collections.Generic;
using System.Text;

namespace ScarredWorld.MardownGenerator
{
    public class Entity
    {
        public Entity(
            string key,
            string name = null,
            string nickname = null,
            string fullName = null,
            string alignment = null,
            string markdownName = null)
        {
            Alignment = alignment;
            FullName = fullName;
            Key = key;
            MarkdownName = markdownName;
            Name = name;
            Nickname = nickname;
        }

        public string Alignment { get; set; }
        public string FullName
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_fullName))
                {
                    if (String.IsNullOrWhiteSpace(Nickname)) { return Name; }
                    else { return $"{Name} ({Nickname})"; }
                }
                return _fullName;
            }
            set { _fullName = value; }
        }
        public string Key { get; set; }
        public string MarkdownLink => $"[{Name}](.\\{MarkdownName})";
        public string MarkdownName
        {
            get { return String.IsNullOrWhiteSpace(_markdownName) ? $"{Key}.md" : _markdownName; }
            set { _markdownName = value; }
        }
        public string Name { get; set; }
        public string Nickname { get; set; }

        private string _fullName;
        private string _markdownName;
    }
}
