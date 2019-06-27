using System;
using System.IO;
using System.Net;
using System.Text;
public partial class Query{
    public static string Get(string uri){
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        //request.ContentType = "application/json";
        //request.UserAgent = "Mozilla";
        request.Headers["Authorization"]="Basic aW50ZXJmYWNlV2ViOlRoM0wwdTE1VjF2MTNy";
            try{
                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream)){
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e){
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse) response;
                    Console.WriteLine("Error code: {0} when trying to GET {1}", httpResponse.StatusCode, uri);
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data)){
                        return "Error";
                    }
                }
            }
            catch (Exception e) {  
                Console.WriteLine("Error: {0} when trying to GET {1}", e.InnerException.Message, uri);
                return "Error";
            }
    }
    public static string Post(string uri, string data){
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        request.ContentLength = dataBytes.Length;
        request.ContentType = "application/json";
        request.Method = "POST";
        //request.UserAgent = "Mozilla";
        request.Headers["Authorization"]="Basic aW50ZXJmYWNlV2ViOlRoM0wwdTE1VjF2MTNy";

        try{
                using(Stream requestBody = request.GetRequestStream()){
                requestBody.Write(dataBytes, 0, dataBytes.Length);
                }

                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream)){
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e){
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse) response;
                    Console.WriteLine("Error code: {0} when trying to POST {1}", httpResponse.StatusCode, uri);
                    using (Stream stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream)){
                        return "Error";
                    }
                }
            }
            catch (Exception e) {  
                Console.WriteLine("Error: {0} when trying to POST {1}", e.InnerException.Message, uri);
                return "Error";
            }
    }

    public static string Put(string uri, string data){
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        request.ContentLength = dataBytes.Length;
        request.ContentType = "application/json";
        request.Method = "PUT";
        //request.UserAgent = "Mozilla";
        request.Headers["Authorization"]="Basic aW50ZXJmYWNlV2ViOlRoM0wwdTE1VjF2MTNy";

        try{
                using(Stream requestBody = request.GetRequestStream()){
                requestBody.Write(dataBytes, 0, dataBytes.Length);
                }

                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream)){
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e){
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse) response;
                    Console.WriteLine("Error code: {0} when trying to PUT {1}", httpResponse.StatusCode, uri);
                    using (Stream stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream)){
                        return "Error";
                    }
                }
            }
            catch (Exception e) {  
                Console.WriteLine("Error: {0} when trying to PUT {1}", e.InnerException.Message, uri);
                return "Error";
            }
    }
    public static string Delete(string uri){

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
        request.ContentType = "application/json";
        request.Method = "DELETE";
        //request.UserAgent = "Mozilla";
        request.Headers["Authorization"]="Basic aW50ZXJmYWNlV2ViOlRoM0wwdTE1VjF2MTNy";

        try{
                using(Stream requestBody = request.GetRequestStream()){
                }

                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream)){
                    return reader.ReadToEnd();
                }
            }
            catch (WebException e){
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse) response;
                    Console.WriteLine("Error code: {0} when trying to DELETE {1}", httpResponse.StatusCode, uri);
                    using (Stream stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream)){
                        return "Error";
                    }
                }
            }
            catch (Exception e) {  
                Console.WriteLine("Error: {0} when trying to DELETE {1}", e.InnerException.Message, uri);
                return "Error";
            }
    }
}