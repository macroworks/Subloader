﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SubtitleSuppliers.OpenSubtitles
{
    public class OpenSubtitles : ISubtitleSupplier
    {
        private readonly string baseRestUrl = "https://rest.opensubtitles.org/search";
        private readonly string userAgentId = "SubLoad v1";

        public async Task<IList<ISubtitleResultItem>> SearchAsync(string path)
        {
            using (HttpClient client = new HttpClient { Timeout = new TimeSpan(0, 0, 0, 0, -1) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("X-User-Agent", userAgentId);

                var responseHash = await client.PostAsync(this.FormHashSearchUrl(path), null);
                string responseHashBody = await responseHash.Content.ReadAsStringAsync(); // this is json string
                var resultHash = JsonConvert.DeserializeObject<IList<OSItem>>(responseHashBody).ToList();

                // return ConvertList(resultHash);

                var responseQuery = await client.PostAsync(this.FormQuerySearchUrl(path), null);
                string responseQueryBody = await responseQuery.Content.ReadAsStringAsync(); // this is json string
                var resultQuery = JsonConvert.DeserializeObject<IList<OSItem>>(responseQueryBody).ToList();

                return ConvertList(resultHash.Concat(resultQuery).Distinct());
            }
        }

        private List<ISubtitleResultItem> ConvertList(IEnumerable<OSItem> list)
        {
            List<ISubtitleResultItem> result = new List<ISubtitleResultItem>();
            foreach(var x in list)
            {
                result.Add(x);
            }
            return result;
        }

        private string FormHashSearchUrl(string path)
        {
            var moviehash = Hasher.ToHexadecimal(Hasher.ComputeMovieHash(path));
            FileInfo file = new FileInfo(path);
            var movieByteSize = file.Length;

            return baseRestUrl
                + $"/moviebytesize-{movieByteSize}"
                + $"/moviehash-{moviehash}";
        }

        private string FormQuerySearchUrl(string path)
        {
            FileInfo file = new FileInfo(path);
            string nameString = file.Name.Replace('.', ' ');

            return baseRestUrl
                + $"/query-{HttpUtility.UrlEncode(nameString)}";
        }
    }
}