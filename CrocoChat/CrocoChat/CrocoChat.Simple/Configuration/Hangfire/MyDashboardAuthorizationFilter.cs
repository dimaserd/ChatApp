using Hangfire.Dashboard;

namespace CrocoChat.Simple.Configuration.Hangfire
{
    public class MyDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //Разрешаем всем пользователям с данными правами доступ к дашборду
            return true;
        }
    }
}