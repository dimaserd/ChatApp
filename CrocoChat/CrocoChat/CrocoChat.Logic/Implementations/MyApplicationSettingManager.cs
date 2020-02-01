using System;
using System.Linq;
using System.Threading.Tasks;
using Croco.Core.Abstractions;
using Croco.Core.Abstractions.Settings;
using Croco.Core.Model.Entities.Configuration;
using Croco.Core.Utils;
using Croco.WebApplication.Abstractions;

namespace CrocoChat.Logic.Implementations
{
    /// <summary>
    /// Класс для работы с настройками пользователя через куки
    /// </summary>
    public class MyApplicationSettingManager : IUserSettingManager
    {
        private readonly ICookieManager _cookieManager;
        private readonly ICrocoAmbientContext _ambientContext;

        /// <summary>
        /// Создает экземпляр класса <see cref="MyApplicationSettingManager"/>
        /// </summary>
        /// <param name="cookieManager"></param>
        /// <param name="ambientContext"></param>
        public MyApplicationSettingManager(ICookieManager cookieManager, ICrocoAmbientContext ambientContext)
        {
            _cookieManager = cookieManager;
            _ambientContext = ambientContext;
        }

        #region Константы
        private const string UserConfigurationPrefix = "userConfiguration_";
        #endregion


        #region Вспомогательные функции

        private void SaveUserSettingToDatabase<T>(T model) where T : class, IUserConfiguration<T>, new()
        {
            var repository = _ambientContext.RepositoryFactory.GetRepository<UserConfiguration>();

            var userId = _ambientContext.RequestContext.UserPrincipal.UserId;

            var typeName = typeof(T).Name;

            var conf = repository.Query().FirstOrDefault(x => x.UserId == userId);

            var json = Tool.JsonConverter.Serialize(model);

            if (conf != null)
            {
                conf.ConfigurationJson = json;

                repository.UpdateHandled(conf);
            }
            else
            {
                conf = new UserConfiguration
                {
                    ConfigurationType = typeName,
                    ConfigurationJson = json,
                    UserId = userId
                };

                repository.CreateHandled(conf);
            }

            _ambientContext.RepositoryFactory.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private void WriteCookie<T>(T value)
        {
            var key = GetUserConfigurationCookieName<T>();

            _cookieManager.Append(key, value);
        }

        private static string GetUserConfigurationCookieName<T>()
        {
            return $"{UserConfigurationPrefix}{typeof(T).Name}";
        }

        private T GetUserSettingFromCookie<T>() where T : class, IUserConfiguration<T>, new()
        {
            var key = GetUserConfigurationCookieName<T>();

            T value;

            if (!_cookieManager.ContainsKey(key))
            {
                value = new T().GetDefault();
                _cookieManager.Append(key, value);

                return value;
            }

            value = _cookieManager.GetValue<T>(key);

            return value;
        }

        /// <summary>
        /// Получить настройку пользователя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> GetUserSettingAsync<T>() where T : class, IUserConfiguration<T>, new()
        {
            return Task.FromResult(GetUserSettingFromCookie<T>());
        }

        /// <summary>
        /// Обновить настройку пользователя
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public Task UpdateUserSettingAsync<T>(T model) where T : class, IUserConfiguration<T>, new()
        {
            if (model == null)
            {
                throw new NullReferenceException(nameof(model));
            }

            if (!model.ValidationFunc())
            {
                return Task.CompletedTask;
            }

            SaveUserSettingToDatabase(model);
            WriteCookie(model);

            return Task.CompletedTask;
        }

        #endregion
    }
}