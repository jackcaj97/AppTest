using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace AppTest
{
    class Post
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Post Id: {0}\nTitle: {1}\nBody: {2}",
                Id, Title, Content);
        }
    }
}
