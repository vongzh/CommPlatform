using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Model
{
    public class Message
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public string Redirect { get; set; }
        public string Qs { get; set; }
        public int Duration { get; set; } = 0;
        public string PopupWinType { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
                {
                    IgnoreSerializableAttribute = true
                }
            });
        }
    }
}
