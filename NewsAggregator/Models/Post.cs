using System;
using System.Text.RegularExpressions;

namespace NewsAggregator.Models {
    public class Post {
        public string Title { get; set; } = "Untitled";
        public string Link { get; set; } = "No link";
        public DateTime Date { get; set; } = new DateTime ();
        public string Author { get; set; } = "Unknown";
        private string _summary = "No summary";

        public string Summary {
            get => _summary;
            set {
                Match matchP = Regex.Match (value, @"<p>\s*(.+?)\s*</p>");
                _summary = matchP.Success ?
                    Regex.Replace (matchP.Groups[1].Value, @"<.+?>", String.Empty) :
                    Regex.Replace (value, @"<.+?>", String.Empty);
            }
        }

    }
}