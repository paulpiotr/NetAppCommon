#region using

using System;
using System.Diagnostics;
using System.Net;

#endregion

#region namespace

namespace NetAppCommon.Extensions.WebClient
{
    public class GZipWebClient : System.Net.WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            Debug.Assert(request != null, nameof(request) + " != null");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}

#endregion
