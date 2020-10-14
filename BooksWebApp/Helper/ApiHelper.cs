using BooksApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BooksApi.Models.TMS;
using System.Linq;
using BooksApi.Models.Paging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Helper
{
    #region API for login
    public class ApiHelper
    {
        private static string _BaseUrl = String.Empty;

        #region Constructor
        public static HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            _BaseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BOOKSAPI")["BaseUrl"];
            client.BaseAddress = new Uri(_BaseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
        #endregion

        #region Login to get token from api
        public static async Task<string> GetUserToken(string userName, string password)
        {
            string result = null;

            using (var client = GetClient())
            {
                string apiURL = $"{StaticVar.ApiUrlUsers}/Login";
                Authenticate login = new Authenticate();
                {
                    login.Code = userName;
                    login.Password = password;
                };
                HttpResponseMessage response = await client.PostAsJsonAsync(apiURL, login).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<string>().ConfigureAwait(false);
                }
                else
                {
                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync().ConfigureAwait(true)}");
                }
            }
            return result;
        }
        #endregion
    }
    #endregion

    #region API for data
    public class ApiHelper<T>
    {
        private static string _BaseUrl = String.Empty;

        #region Constructor
        public static HttpClient GetClient()
        {
            _BaseUrl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BOOKSAPI")["BaseUrl"];
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(_BaseUrl);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // if use Session to save token, use this !!
            //string userToken = AppContext.Current.Session.GetString(StaticVar.SessionUserToken);

            // if use cookies save ClaimsUser, use this !!
            string userToken = AppContext.Current.User.Claims.Where(c => c.Type == StaticVar.SessionUserToken).FirstOrDefault()?.Value;

            if (!String.IsNullOrEmpty(userToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
            }

            return client;
        }
        #endregion

        #region GET ASYNC, only use to get data from api

        public static async Task<dynamic> RunGetAsync(string _ApiUrl)
        {
            dynamic apiResponse = null;
            using (var client = GetClient())
            {

                HttpResponseMessage response = await client.GetAsync(_ApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadAsAsync<T>();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {

                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (response.Headers.Contains("Token-Expired"))
                    {
                        throw new Exception(StaticVar.ExpiredToken);
                    }
                    else
                    {
                        throw new Exception("Lỗi: Không có quyền truy cập !");
                    }
                }
                else
                {
                    throw new Exception(response.StatusCode.ToString() + response.Content.ToString());
                }
            }
            return apiResponse;
        }

        #endregion

        #region POST ASYNC, use to create ! Return new item if success Insert and NUll if error !

        public static async Task<dynamic> RunPostAsync(string _ApiUrl, object inputData = null)
        {
            dynamic apiResponse = null;

            using (var client = GetClient())
            {

                HttpResponseMessage response = await client.PostAsJsonAsync(_ApiUrl, inputData);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadAsAsync<T>();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {

                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (response.Headers.Contains("Token-Expired"))
                    {
                        throw new Exception(StaticVar.ExpiredToken);
                    }
                    else
                    {
                        throw new Exception("Lỗi: Không có quyền truy cập !");
                    }
                }
                else
                {
                    return $"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                }
            }
            return apiResponse;
        }


        #endregion

        #region PUT ASYNC, use to update! Return OK if success updated and NULL if error !

        public static async Task<dynamic> RunPutAsync(string _ApiUrl, object inputData = null)
        {
            dynamic apiResponse = null;

            using (var client = GetClient())
            {

                HttpResponseMessage response = await client.PutAsJsonAsync(_ApiUrl, inputData);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadAsAsync<T>();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {

                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {

                    if (response.Headers.Contains("Token-Expired"))
                    {
                        throw new Exception(StaticVar.ExpiredToken);
                    }
                    else
                    {
                        return $"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
                else
                {
                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            return apiResponse;
        }

        #endregion

        #region DELETE ASYNC, use to delete ! Return OK if success deleted and NULL if error !
        public static async Task<dynamic> RunDeleteAsync(string _ApiUrl)
        {
            dynamic apiResponse = null;

            using (var client = GetClient())
            {

                HttpResponseMessage response = await client.DeleteAsync(_ApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = await response.Content.ReadAsAsync<T>();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception($"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    //throw new Exception("Lỗi BadRequest");
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (response.Headers.Contains("Token-Expired"))
                    {
                        throw new Exception(StaticVar.ExpiredToken);
                    }
                    else
                    {
                        return $"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                    }
                }
                else
                {
                    return $"{(int)response.StatusCode} - {await response.Content.ReadAsStringAsync()}";
                }
            }
            return apiResponse;
        }
        #endregion

        #region ISSET Code ?
        public static async Task<bool> CheckIssetCode(string _url)
        {
            try
            {
                return await RunGetAsync(_url);
            }
            catch(Exception)
            {
                return false;
            }
        }
        #endregion

        #region Datapaging Table
        public static DatatablesPaging InitDatatable(IFormCollection request)
        {
            DatatablesPaging database = new DatatablesPaging
            {
                Draw = request["draw"],
                Start = request["start"],
                Length = request["length"],
                SearchValue = request["search[value]"]
            };
            var column_Index = request["order[0][column]"];
            database.SortColumn = request["columns[" + column_Index + "][data]"];
            database.SortColumnDirection = request["order[0][dir]"];

            database.SearchArray = new Dictionary<string, string>();

            if ((request.Keys.Count - 7) > 0)
            {
                for (int k = 0; k < (request.Keys.Count - 7) / 6; k++)
                {
                    if (!string.IsNullOrEmpty(request["columns[" + k + "][search][value]"]))
                    {
                        database.SearchArray.Add(request["columns[" + k + "][data]"], request["columns[" + k + "][search][value]"]);

                    }
                }
            }

            //database.SearchArray.Add(request["columns[0][data]"], request["columns[0][search][value]"]);

            return database;
        }
        #endregion
    }
    #endregion


}
