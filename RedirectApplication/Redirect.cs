using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedirectApplication.Models;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Net.Http;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Web;
using Microsoft.AspNetCore.Http;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using System.Text.RegularExpressions;
using System.Security.Policy;
using Microsoft.Extensions.Logging;
using RedirectApplication.Services;

namespace RedirectApplication
{
    public class Redirect
    {
        public string _apiUrl;
        public IHttpClientFactory _httpClientFactory;
        public Dictionary<string, RedirectModel> _redirectModel;

        public Redirect(string apiUrl, IHttpClientFactory httpClientFactory)
        {
            _apiUrl = apiUrl;
            _httpClientFactory = httpClientFactory;
        }


        public string SanitizeUrl(string url)
        {
            return Regex.Replace(url, @"(^\/.*)?(\/$|\/(\?.*))", "").ToLower(); ;
        }

        public Response GetRedirectByUrl(string requestUrl)
        {
            requestUrl = SanitizeUrl(requestUrl);
            RedirectModel redirect = new RedirectModel();

            if (_redirectModel.ContainsKey(requestUrl))
            {
                redirect = _redirectModel[requestUrl];
                return new Response(redirect.TargetUrl, redirect.RedirectType);
            }
            else
            {
                return GetRelativePathRedirect(requestUrl);
            }
        }

        public Response GetRelativePathRedirect(string requestUrl)
        {
            RedirectModel redirect = new RedirectModel();

            List<string> urlSeperates = requestUrl.Split('/').ToList();

            //var seperates = new Uri(redirect.TargetUrl).Segments;

            for (var i = urlSeperates.Count; i > 0; i--)
            {

                var newUrl = String.Join("/", urlSeperates.Take(i));

                if (_redirectModel.ContainsKey(newUrl))
                {
                    redirect = _redirectModel[newUrl];

                    if (redirect.UseRelative)
                    {
                        var redirectUrl = $"{redirect.TargetUrl}/{String.Join("/", urlSeperates.Skip(i))}";

                        return new Response(redirectUrl, redirect.RedirectType);
                    }
                }
            }

            return null;
        }

        public async Task Refresh()
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(_apiUrl);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var redirects = JsonSerializer.Deserialize<List<RedirectModel>>(json, new JsonSerializerOptions());

            await Load(redirects);
        }

        public Task Load(IEnumerable<RedirectModel> redirects)
        {
            if (redirects == null)
            {
                redirects = new List<RedirectModel>();
            }

            _redirectModel = redirects.ToDictionary(response => SanitizeUrl(response.RedirectUrl), response =>
            {
                response.RedirectUrl = SanitizeUrl(response.RedirectUrl);
                response.TargetUrl = SanitizeUrl(response.TargetUrl);

                return response;
            });

            return Task.CompletedTask;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestUrl = context.Request.Path + context.Request.QueryString;
            var redirectResponse = GetRedirectByUrl(requestUrl);

            if (redirectResponse == null)
            {
                await next(context);

                return;
            }

            context.Response.Redirect(redirectResponse.RedirectUrl, redirectResponse.StatusCode == 301);
        }

    }
}

