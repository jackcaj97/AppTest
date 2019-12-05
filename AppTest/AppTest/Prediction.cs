using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace AppTest
{
    class Prediction
    {
        public float probability { get; set; }
        public string tagId { get; set; }
        public string tagName { get; set; }

        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Probability: {0}\nTagId: {1}\nTagName: {2}",
                probability, tagId, tagName);
        }
    }
}
