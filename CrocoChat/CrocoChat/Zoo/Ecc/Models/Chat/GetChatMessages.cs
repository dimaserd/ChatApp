﻿using System.ComponentModel;

namespace Zoo.Ecc.Models.Chat
{
    public class GetChatMessages
    {
        /// <summary>
        /// Сколько нужно взять сообщений
        /// </summary>
        [Description("Сколько нужно взять сообщений")]
        public int Count { get; set; }

        /// <summary>
        /// Будут подгружены те сообщения, которые меньше данной даты
        /// </summary>
        [Description("")]
        public long LessThantUtcTicks { get; set; }

        /// <summary>
        /// Идентификатор чата
        /// </summary>
        [Description("Идентификатор чата")]
        public int ChatId { get; set; }
    }
}