﻿using coffeecard.Helpers.MobilePay.RequestMessage;
using coffeecard.Helpers.MobilePay.ResponseMessage;
using Jose;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using coffeecard.Helpers.MobilePay.ErrorMessage;

namespace coffeecard.Helpers.MobilePay
{
    public class MobilePayApiHttpClient : IDisposable
    {
        private const string MobilePayBaseEndpoint = "https://api.mobeco.dk/appswitch/api/v1/";
        private const string CertificateName = "www.analogio.dk.pfx";

        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly X509Certificate2 _certificate;

        public MobilePayApiHttpClient(HttpClient client, IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _certificate = LoadCertificate(environment);

            _client = client;
        }

        public async Task<T> SendRequest<T>(IMobilePayAPIRequestMessage requestMessage) where T: IMobilePayAPIResponse
        {
            var requestUri = new Uri(MobilePayBaseEndpoint + requestMessage.GetEndPointUri());

            var request = new HttpRequestMessage()
            {
                Headers =
                {
                    { "AuthenticationSignature", GenerateAuthenticationSignature(requestUri.ToString(), requestMessage.GetRequestBody()) },
                    { "Ocp-Apim-Subscription-Key", _configuration["MPSubscriptionKey"] }
                },
                Method = requestMessage.GetHttpMethod(),
                RequestUri = requestUri,
                Content = new StringContent(requestMessage.GetRequestBody(), Encoding.UTF8, "application/json")
            };

            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                await HandleHttpErrorAsync(response);
            }

            return await response.Content.ReadAsAsync<T>();
        }

        private static async Task HandleHttpErrorAsync(HttpResponseMessage response)
        {
            Log.Error($"HTTP Response failed with statusCode = {response.StatusCode} and message = {response.Content.ReadAsStringAsync().Result}");

            switch (response.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    {
                        var errorMessage = await response.Content.ReadAsAsync<InternalServerErrorMessage>();
                        throw new MobilePayException(errorMessage, HttpStatusCode.InternalServerError);
                    }
                case HttpStatusCode.RequestTimeout:
                    {
                        throw new MobilePayException(new DefaultErrorMessage()
                        {
                            Reason = MobilePayErrorReason.Other
                        }, HttpStatusCode.RequestTimeout);
                    }
                default:
                    {
                        var errorMessage = await response.Content.ReadAsAsync<DefaultErrorMessage>();
                        throw new MobilePayException(errorMessage, response.StatusCode);
                    }
            }
        }

        private X509Certificate2 LoadCertificate(IHostingEnvironment environment)
        {
            var provider = environment.ContentRootFileProvider;
            var contents = provider.GetDirectoryContents(string.Empty);
            var certPath = contents.FirstOrDefault(file => file.Name.Equals(CertificateName)).PhysicalPath;

            return new X509Certificate2(certPath, _configuration["CertificatePassword"], X509KeyStorageFlags.MachineKeySet);
        }

        private string GenerateAuthenticationSignature(string requestUri, string requestBody)
        {
            var combinedRequest = requestUri + requestBody;

#pragma warning disable CA5350 // SHA1 is used per Mobile Pay documentation but considered weak. Ignore compiler warning
            var sha1 = new SHA1Managed();
#pragma warning restore CA5350
            var hash = Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(combinedRequest)));

            using (RSA rsa = _certificate.GetRSAPrivateKey())
            {
                var signature = JWT.Encode(hash, rsa, JwsAlgorithm.RS256);
                return signature;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
