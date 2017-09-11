using System;
using System.Collections.Generic;
using System.Linq;
using ItunesSearch.Models;
using System.Web;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Threading;

namespace ItunesSearch.SearchAPI
{
    public class ItunesAPI
    {
        public static List<GroupedResult> Search(string Term)
        {
            try
            {
               //Making Web Request
                ItunesSearchResult res = new ItunesSearchResult();
                string BaseURL = "https://itunes.apple.com/search?entity=movie,album,shortFilm,ebook,allArtist,podcast,software,musicVideo,mix,audiobook,tvSeason,allTrack&term=";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseURL + Term);
                request.Method = "GET";
                request.Headers.Add("cache-control", "no-cache");
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                request.ContentType = "application/json";
                
                //Getting Response from Web Request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string content = string.Empty;
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                        //Serializing the Response to Object.
                        res =JsonConvert.DeserializeObject<ItunesSearchResult>(content);
                    }
                }
                //To Capitalize the group key.
                TextInfo textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;
                return res.results.GroupBy(t => t.kind==null? (t.collectionType == null? t.wrapperType:t.collectionType) : t.kind).Select(t => new GroupedResult() { Type = textInfo.ToTitleCase(t.Key.ToString())+"s", results = t.ToList(), TotalCount = res.resultCount }).ToList();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}