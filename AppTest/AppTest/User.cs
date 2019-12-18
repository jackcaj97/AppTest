using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace AppTest
{
    class User
    {
        public string id { get; set; }
        public string email { get; set; }
        public bool verified_email { get; set; }
        public string picture { get; set; }

        [JsonProperty("body")]
        public string Content { get; set; }

        public override string ToString()
        {
            return string.Format(
                "Id: {0}\nEmail: {1}\nVerified: {2}\nPicture: {3}",
                id, email, verified_email, picture);
        }
    }
}
