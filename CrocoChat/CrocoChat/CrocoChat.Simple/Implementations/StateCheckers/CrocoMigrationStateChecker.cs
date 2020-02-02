using Croco.Core.Abstractions.Application;
using Croco.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CrocoChat.Simple.Implementations.StateCheckers
{
    /// <summary>
    /// Проверяет базу данных на примененность последней версии миграций
    /// </summary>
    public class CrocoMigrationStateChecker
    {
        public static void CheckApplicationState(ICrocoApplication app)
        {
            var appContext = app.GetDatabaseContext(SystemCrocoExtensions.GetRequestContext());

            var context = appContext.GetDbContext();

            var contextType = context.GetType();
            var modelProjectName = contextType.Assembly.ManifestModule.Name.Replace(".dll", "");
            var dbContextClassName = contextType.Name;


            var lastAppliedMigration = context.Database.GetAppliedMigrations().LastOrDefault();
            var lastDefinedMigration = context.Database.GetMigrations().LastOrDefault();

            var command = $"Update-Database -Context {dbContextClassName}";

            var instructions = "Зайдите в Средства->Диспетчер пакетов NuGet->" +
                    $"Консоль диспетчера пакетов, далее выберите проект по умолчанию {modelProjectName} и введите команду {command}";

            if (lastAppliedMigration == null)
            {
                throw new Exception($"К вашей базе данных не применены миграции. {instructions}");
            }

            if (lastDefinedMigration != lastAppliedMigration)
            {
                throw new Exception($"К вашей базе данных не применена последняя миграция. {instructions}");
            }
        }
    }
}