using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace AppTest
{
    public class CollectionImage
    {
        public string imageUrl { get; set; }
        public string tag1 { get; set; }
        public string tag2 { get; set; }

        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            
            return string.Format(
                "Result:\n imageUrl: {0}\nTag1: {1}\ntag2: {2}\n", imageUrl, tag1, tag2);
        }
    }
}
