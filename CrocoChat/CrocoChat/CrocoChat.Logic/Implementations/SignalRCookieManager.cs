using System;
using System.Collections.Generic;
using Croco.Core.Utils;
using Croco.WebApplication.Abstractions;
using Microsoft.AspNetCore.Http;

namespace CrocoChat.Logic.Implementations
{
    public class SignalRCookieManager : ICookieManager
    {
        public SignalRCookieManager(IRequestCookieCollection cookies)
        {
            Cookies = cookies;
        }

        public IEnumerable<string> Keys => Cookies.Keys;

        public IRequestCookieCollection Cookies { get; }

        static void ThrowException()
        {
            throw new ApplicationException("Внутри запроса SignalR куки доступны только для чтения");
        }

        public void Append(string name, string value)
        {
            ThrowException();
        }

        public void Append<T>(string name, T value)
        {
            ThrowException();
        }

        public bool ContainsKey(string key)
        {
            return Cookies.Keys.Contains(key);
        }

        public string GetValue(string key)
        {
            Cookies.TryGetValue(key, out var value);

            return value;
        }

        public T GetValue<T>(string key) where T : class
        {
            var value = GetValue(key);

            return Tool.JsonConverter.Deserialize<T>(value);
        }

        public void Remove(string key)
        {
            ThrowException();
        }
    }
}