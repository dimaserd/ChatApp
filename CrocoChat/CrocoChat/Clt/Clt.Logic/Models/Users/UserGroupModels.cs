namespace Clt.Logic.Models.Users
{
    /// <summary>
    /// Модель добавления или удаления одного пользователя из группы
    /// </summary>
    public class UserInGroupAddOrDelete
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Если добавить в группу то значение равно true, если удалить значить false
        /// </summary>
        public bool AddOrDelete { get; set; }
    }
}