﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary
{
    public class UsefulWebApis
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task<dynamic> MciOtpSendActivationCode(string serviceCode, string mobileNumber, string price)
        {
            dynamic result = new ExpandoObject();
            result.Status = "Error";
            try
            {
                var accessKey = Security.GetSha256Hash("OtpCharge" + serviceCode + mobileNumber);
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                {
                   { "AccessKey", accessKey },
                   { "MobileNumber", mobileNumber },
                   { "Price", price },
                   { "ServiceCode", serviceCode }
                };

                    var content = new FormUrlEncodedContent(values);
                    var url = "http://79.175.164.51:8093/api/App/OtpCharge";
                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
                    result = jsonResponse;
                }
            }
            catch (Exception e)
            {
                result.Status = "Error";
                logs.Error("Exception in MciOtpSendActivationCode: ", e);
            }
            return result;
        }
        public static async Task<dynamic> MciOtpSendConfirmCode(string serviceCode, string mobileNumber, string confirmCode)
        {
            dynamic result = new ExpandoObject();
            result.Status = "Error";
            try
            {
                var accessKey = Security.GetSha256Hash("OtpConfirm" + serviceCode + mobileNumber);
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>
                {
                   { "AccessKey", accessKey },
                   { "MobileNumber", mobileNumber },
                   { "ConfirmCode", confirmCode },
                   { "ServiceCode", serviceCode }
                };

                    var content = new FormUrlEncodedContent(values);
                    var url = "http://79.175.164.51:8093/api/App/OtpConfirm";
                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
                    result = jsonResponse;
                }
            }
            catch (Exception e)
            {
                result.Stauts = "Error";
                logs.Error("Exception in MciOtpSendConfirmCode: ", e);
            }
            return result;
        }

        public static async Task<T> NotificationBotApi<T>(string methodName, Dictionary<string, string> parameters) where T : class
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(parameters);
                    var url = "http://79.175.164.51:8093/api/Bot/" + methodName;
                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                        return null;
                    var responseString = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseString,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in BotApis: ", e);
            }
            return null;
        }
    }
}