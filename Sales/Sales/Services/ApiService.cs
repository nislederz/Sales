namespace Sales.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Common.Models;
    using Plugin.Connectivity;
    using Helpers;

    public class ApiService
    {
        public async Task<Responce> CheckConnection() {
            if (!CrossConnectivity.Current.IsConnected) {
                return new Responce
                {
                    IsSuccess = false,
                    Message = Languages.TurnOnInternet,
                };
            }
            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable) {
                return new Responce
                {
                    IsSuccess = false,
                    Message = Languages.NoInternet,
                };
            }
            return new Responce
            {
                IsSuccess = true,
            };
        }

        public async Task<Responce> GetList<T>(string urlBase, string prefix, string controller) {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}";
                var response = await client.GetAsync(url);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode) {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var list = JsonConvert.DeserializeObject<List<T>>(answer);
                return new Responce
                {
                    IsSuccess = true,
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Responce
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        
    }
}
