using Kang.KangException;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kang.Util
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpUtil
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="ContentType">application/json</param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static String PostResponse(string url, string postData,String ContentType, string statusCode)
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(postData))
            {
                throw new KangHttpException("===>KangHttpException:PostData不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            string result = string.Empty;
            //设置Http的正文
            HttpContent httpContent = new StringContent(postData);
            //设置Http的内容标头
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                //异步Post
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                //输出Http响应状态码
                statusCode = response.StatusCode.ToString();
                //确保Http响应成功
                if (response.IsSuccessStatusCode)
                {
                    //异步读取json
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        /// <summary>
        /// JSON、POST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string PostResponse(string url, string postData, string statusCode)
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(postData))
            {
                throw new KangHttpException("===>KangHttpException:PostData不能为空！");
            }
            string result = string.Empty;
            //设置Http的正文
            HttpContent httpContent = new StringContent(postData);
            //设置Http的内容标头
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                //异步Post
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                //输出Http响应状态码
                statusCode = response.StatusCode.ToString();
                //确保Http响应成功
                if (response.IsSuccessStatusCode)
                {
                    //异步读取json
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        /// <summary>
        /// 泛型：Post请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static T PostResponse<T>(string url, string postData, String ContentType) where T : class, new()
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(postData))
            {
                throw new KangHttpException("===>KangHttpException:PostData不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            T result = default(T);

            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ContentType);
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    //Newtonsoft.Json
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }

        /// <summary>
        /// 泛型：Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static T GetResponse<T>(string url, String ContentType) where T : class, new()
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            T result = default(T);

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="ContentType"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string GetResponse(string url, String ContentType, out string statusCode)
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            string result = string.Empty;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                statusCode = response.StatusCode.ToString();

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="putData"></param>
        /// <param name="ContentType"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static string PutResponse(string url, string putData, String ContentType, out string statusCode)
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(putData))
            {
                throw new KangHttpException("===>KangHttpException:PostData不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            string result = string.Empty;
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;
                statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }

        /// <summary>
        /// 泛型：Put请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="putData"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static T PutResponse<T>(string url, string putData, String ContentType) where T : class, new()
        {
            if (StringUtil.isBlank(url))
            {
                throw new KangHttpException("===>KangHttpException:Url不能为空！");
            }
            if (StringUtil.isBlank(putData))
            {
                throw new KangHttpException("===>KangHttpException:PostData不能为空！");
            }
            if (StringUtil.isBlank(ContentType))
            {
                throw new KangHttpException("===>KangHttpException:ContentType不能为空！");
            }
            T result = default(T);
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    string s = t.Result;
                    string json = JsonConvert.DeserializeObject(s).ToString();
                    result = JsonConvert.DeserializeObject<T>(json);
                }
            }
            return result;
        }
    }
}
