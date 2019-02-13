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
    using System.Text;
    using System.Net.Http.Headers;

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
        public async Task<TokenResponse> GetToken(string urlBase, string username, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var response = await client.PostAsync("Token",
                    new StringContent(string.Format(
                        "grant_type=password&username={0}&password={1}",
                        username, password), Encoding.UTF8, "application/x-www-form-urlencoded"));
                var resultJSON = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(resultJSON);
                return result;
            }
            catch
            {
                return null;
            }
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
        public async Task<Responce> GetList<T>(string urlBase, string prefix, string controller, string tokenType,string accessToken)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}";
                var response = await client.GetAsync(url);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
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
        public async Task<Responce> Post<T>(string urlBase, string prefix, string controller, T model) {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request,Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}";
                var response = await client.PostAsync(url,content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Responce
                {
                    IsSuccess = true,
                    Result = obj,
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
        public async Task<Responce> Post<T>(string urlBase, string prefix, string controller, T model, string tokenType, string accessToken)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}";
                var response = await client.PostAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Responce
                {
                    IsSuccess = true,
                    Result = obj,
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
        public async Task<Responce> Put<T>(string urlBase, string prefix, string controller, T model, int id)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}/{id}";
                var response = await client.PutAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Responce
                {
                    IsSuccess = true,
                    Result = obj,
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
        public async Task<Responce> Put<T>(string urlBase, string prefix, string controller, T model, int id, string tokenType, string accessToken)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}/{id}";
                var response = await client.PutAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                var obj = JsonConvert.DeserializeObject<T>(answer);
                return new Responce
                {
                    IsSuccess = true,
                    Result = obj,
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
        public async Task<Responce> Delete<T>(string urlBase, string prefix, string controller,int id)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = $"{prefix}{controller}/{id}";
                var response = await client.DeleteAsync(url);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }
                
                return new Responce
                {
                    IsSuccess = true,
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
        public async Task<Responce> Delete<T>(string urlBase, string prefix, string controller, int id, string tokenType, string accessToken)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}/{id}";
                var response = await client.DeleteAsync(url);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce
                    {
                        IsSuccess = false,
                        Message = answer,
                    };
                }

                return new Responce
                {
                    IsSuccess = true,
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
        public async Task<Responce> GetUser(string urlBase, string prefix, string controller, string email, string tokenType, string accessToken)
        {
            try
            {
                var getUserRequest = new GetUserRequest { Email = email, };
                var request = JsonConvert.SerializeObject(getUserRequest);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient(); client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = $"{prefix}{controller}";
                var response = await client.PostAsync(url, content);
                var answer = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new Responce { IsSuccess = false, Message = answer, };
                }
                var user = JsonConvert.DeserializeObject<MyUserASP>(answer);
                return new Responce { IsSuccess = true, Result = user, };
            }
            catch (Exception ex)
            {
                return new Responce { IsSuccess = false, Message = ex.Message, };
            }
        }



    }
}
