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

            switch (message.Text?.ToLower())
            {
                case "клавиатура":
                    await EnableKeyboard(chatId);
                    break;

                case "абоба":
                case "сам абоба":
                    await _botClient.SendTextMessageAsync(chatId, "сам абоба", cancellationToken: _ct);
                    break;

                case "/start":
                    await ToMainMenu(chatId);
                    break;

                /*
                case "📅 расписание 📅":
                case "расписание":
                    InlineKeyboardMarkup ik_etomto = new(new[]
                    {
                        InlineKeyboardButton.WithCallbackData("ЭТО", "Отделение_ЭТО"),
                        InlineKeyboardButton.WithCallbackData("МТО", "Отделение_МТО"),
                    });
                    await _botClient.SendTextMessageAsync(chatId, "Выберите отделение.", replyMarkup: ik_etomto, cancellationToken: _ct);
                    break;
                */

                case "🏠 на главную 🏠":
                case "на главную":
                    await ToMainMenu(chatId);
                    break;

                default:
                    await _botClient.SendTextMessageAsync(chatId, "Неизвестная команда.", cancellationToken: _ct);
                    break;
            }
        }

        public static async Task ToMainMenu(long chatId, bool asEdit = false, int? msgId = null)
        {
            if (_botClient == null) return;
            string menuHeader = "________🏠 Меню 🏠________\n\n__⬇️ Выберите вкладку ⬇️__";
            InlineKeyboardMarkup ik_main = new(new[]
                    {
                        new[] { InlineKeyboardButton.WithCallbackData("Приемная комиссия", "приемная_комиссия") },
                        new[] { InlineKeyboardButton.WithCallbackData("Подача заявления", "подача_заявления") },
                        new[] { InlineKeyboardButton.WithCallbackData("Проходные баллы", "проходные_баллы") },
                        new[] { InlineKeyboardButton.WithCallbackData("Адреса на карте", "показать_карту") },
                        new[] { InlineKeyboardButton.WithUrl("Специальности", "http://eetk.ru/20-2/22-2/"),
                                InlineKeyboardButton.WithUrl("Приказы о зачислении", "http://eetk.ru/20-2/9985-2/") 
                        },
                        /*new[] { InlineKeyboardButton.WithCallbackData("Расписание", "Главная_Расписание") },
                        new[] { InlineKeyboardButton.WithCallbackData("О боте", "Главная_ОБоте") },*/
                    });
            if (asEdit)
            {
                if (msgId == null)
                {
                    Console.WriteLine("null msgId");
                    return;
                }
                else await _botClient.EditMessageTextAsync(chatId, (int)msgId, menuHeader, replyMarkup: ik_main, cancellationToken: _ct);
            }
            else await _botClient.SendTextMessageAsync(chatId, menuHeader, replyMarkup: ik_main, cancellationToken: _ct);
        }

        static async Task EnableKeyboard(long chatId)
        {
            if (_botClient == null) return;
            Console.WriteLine("Enabling reply keyboard");
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[] { new KeyboardButton[] { "🏠 На главную 🏠", /*"📅 Расписание 📅"*/ } }) { ResizeKeyboard = true };
            await _botClient.SendTextMessageAsync(chatId, "Включаю клавиатуру...", replyMarkup: replyKeyboardMarkup, disableNotification: true, cancellationToken: _ct);
            //await _botClient.DeleteMessageAsync(chatId, kbmsg.MessageId, _ct);
        }
    }
}
