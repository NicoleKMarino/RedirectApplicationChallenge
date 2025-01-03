using Moq;
using RedirectApplication.Models;

namespace RedirectApplication.Tests
{
    [TestClass]
    public class RedirectTests
    {
        public readonly Redirect _redirect;

        public RedirectTests()
        {
            var apiUrl = "http://localhost:8080/";
            var httpClientFactory = new Mock<IHttpClientFactory>();

            _redirect = new Redirect(apiUrl, httpClientFactory.Object);

            var redirects = new List<RedirectModel>()
            {
               new() {
                   RedirectUrl = "/campaignA",
                   TargetUrl = "/campaigns/targetcampaign",
                   RedirectType = 302,
                   UseRelative = false
               },
               new() {
                   RedirectUrl = "/campaignB",
                   TargetUrl = "/campaigns/targetcampaign/channelB",
                   RedirectType = 302,
                   UseRelative = false
               },
               new() {
                   RedirectUrl = "/product-directory",
                   TargetUrl = "/products",
                   RedirectType = 301,
                   UseRelative = true
               }
            };

            _redirect.Load(redirects);
        }

        [TestMethod]
        [DataRow("/campaignC", null, null)]
        [DataRow("/product-directory", "/products", 301)]
        [DataRow("/product-directoryandsomemore", null, null)]
        [DataRow("/product-directory/bits", "/products/bits", 301)]
        [DataRow("/product-directory/bits/masonary/diamond-tip", "/products/bits/masonary/diamond-tip", 301)]
        [DataRow("/campaignA", "/campaigns/targetcampaign", 302)]
        [DataRow("/campaignB", "/campaigns/targetcampaign/channelb", 302)]
        [DataRow("/campaignAlonger", null, null)]
        [DataRow("/none", null, null)]

        public void ReturnsCorrectResult(string requestUrl, string? expectedRedirectUrl, int? expectedStatusCode)
        {
            var redirect = _redirect.GetRedirectByUrl(requestUrl);

            Assert.AreEqual(expectedRedirectUrl, redirect?.RedirectUrl, true);
            Assert.AreEqual(expectedStatusCode, redirect?.StatusCode);
        }
    }
}