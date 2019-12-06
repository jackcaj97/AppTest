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
            string predict = "";

            foreach (Prediction p in predictions)
            {
                predict = predict + p.ToString() + "\n";
            }

            return string.Format(
                "Resutlt Id: {0}\nProject: {1}\nIteration: {2}\nCreated: {3}\n" + predict,
                id, project, iteration, created);
        }
    }
}
