using System;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace RabbitFarmLocal.messaging
{
   
    public class MyTelegram
    {
        private static readonly MyTelegram _instance = new MyTelegram();
        static ITelegramBotClient botClient;

       private  MyTelegram()
        {
            botClient = new TelegramBotClient("1636269565:AAEpoyhFvHYw0Z9ezl7NVKaINrDMILauLPo");
        }
        public static MyTelegram GetTelegram()
        {
            return _instance;
        }
        
       public static void SendMessageToBot(string message)
        {
             botClient.SendTextMessageAsync(
                  chatId: 1528799054,
                  text: "!!!Антон, у вас просрочены следующие задачи: \n\r"+message
                );
            botClient.SendTextMessageAsync(
                 chatId: 845900431,
                 text: "!!!Танюшечка, у вас, моя дорогая, просрочены следующие задачи: \n\r" + message
               );
        }

    }
}
