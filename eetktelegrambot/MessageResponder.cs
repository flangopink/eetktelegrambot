using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace eetktelegrambot
{
    internal class MessageResponder
    {
        static ITelegramBotClient? _botClient;
        static CancellationToken _ct;

        static bool keyboardActive = false;

        public static void Init(ITelegramBotClient botClient, CancellationToken ct)
        {
            _botClient = botClient;
            _ct = ct;
        }

        public static async Task Respond(Message message, long chatId)
        {
            if (_botClient == null) return;
            if (!keyboardActive)
            {
                await EnableKeyboard(chatId);
                keyboardActive = true;
            }

            switch (message.Text)
            {
                case "почини клавиатуру":
                    await EnableKeyboard(chatId);
                    break;

                case "абоба":
                case "сам абоба":
                case "сам абоба.":
                    await _botClient.SendTextMessageAsync(chatId, "сам абоба.", cancellationToken: _ct);
                    break;

                case "📅 Расписание 📅":
                case "Расписание":
                case "расписание":
                    InlineKeyboardMarkup ik_etomto = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("ЭТО", "Отделение_ЭТО"),
                        InlineKeyboardButton.WithCallbackData("МТО", "Отделение_МТО"),
                    });
                    await _botClient.SendTextMessageAsync(chatId, "Выберите отделение.", replyMarkup: ik_etomto, cancellationToken: _ct);
                    break;

                case "🏠 На главную 🏠":
                case "На главную":
                case "на главную":
                    InlineKeyboardMarkup ik_main = new(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Кнопка 1", "Главная_Кнопка1") },
                        new[] { InlineKeyboardButton.WithCallbackData("Кнопка 2", "Главная_Кнопка2") },
                        new[] { InlineKeyboardButton.WithCallbackData("Кнопка 3", "Главная_Кнопка3") },
                        new[] { InlineKeyboardButton.WithCallbackData("Расписание", "Главная_Расписание") },
                        new[] { InlineKeyboardButton.WithCallbackData("О боте", "Главная_ОБоте") },
                    });
                    await _botClient.SendTextMessageAsync(chatId, "🏠", replyMarkup: ik_main, cancellationToken: _ct);
                    break;

                default:
                    await _botClient.SendTextMessageAsync(chatId, "Неизвестная команда.", cancellationToken: _ct);
                    break;
            }
        }

        static async Task EnableKeyboard(long chatId)
        {
            if (_botClient == null) return;
            Console.WriteLine("Enabling reply keyboard");
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "🏠 На главную 🏠", "📅 Расписание 📅" } }) { ResizeKeyboard = true };
            var kbmsg = await _botClient.SendTextMessageAsync(chatId, "Включаю клавиатуру...", replyMarkup: replyKeyboardMarkup, disableNotification: true, cancellationToken: _ct);
            //await _botClient.DeleteMessageAsync(chatId, kbmsg.MessageId, _ct);
        }
    }
}
