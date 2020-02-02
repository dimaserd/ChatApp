using Hangfire.Dashboard;

namespace CrocoChat.Simple.Configuration.Hangfire
{
    public class FreeDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            //Разрешаем вообще всем пользователям доступ к дашборду
            return true;
        }
    }
}