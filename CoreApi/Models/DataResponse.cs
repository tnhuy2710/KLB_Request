using System.Collections;
using System.Globalization;
using Newtonsoft.Json;

namespace CoreApi.Models
{
    public class DataResponse
    {
        [JsonIgnore]
        public double StatusCode { get; set; }

        /// <summary>
        /// Status code as string will help you define all case when API response.
        /// </summary>
        [JsonProperty("StatusCode")]
        public string StatusCodeString => StatusCode.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// Response message from Backend. Sometime you don't need handle all case when api response, backend will show message to client.
        /// </summary>
        [JsonProperty("Message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Debug Message for Tracking Debug when has error 500. Please sent it to Backend developer.
        /// </summary>
        [JsonProperty("DebugMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string Debug { get; set; }

        /// <summary>
        /// It return count of item if Data reponse is List.
        /// </summary>
        [JsonProperty("ItemCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? ItemCount { get; set; }

        /// <summary>
        /// Total item page of item if Data response is list and support pagination.
        /// </summary>
        [JsonProperty("TotalPage", NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalPage { get; set; }

        private object _data;

        /// <summary>
        /// Data response is dynamic. It can be String, List, Int, ...
        /// </summary>
        [JsonProperty("Data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data
        {
            get => _data;
            set
            {
                _data = value;

                // Set item count if data is List
                var list = value as IList;
                if (list != null)
                    ItemCount = list.Count;
            }
        }
    }
}
