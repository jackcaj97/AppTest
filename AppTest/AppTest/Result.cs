using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace AppTest
{
    class Result
    {
        public string id { get; set; }
        public string project { get; set; }
        public string iteration { get; set; }
        public string created { get; set; }
        public List<Prediction> predictions { get; set; }


        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Resutlt Id: {0}\nProject: {1}\nIteration: {2}\nCreated: {3}\n" + predictions.ToString(),
                id, project, iteration, created);
        }
    }
}
