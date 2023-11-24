using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace eetktelegrambot
{
    internal class CallbackResponder
    {
        public static async Task Respond(Update update, ITelegramBotClient botClient, CancellationToken ct)
        {
            var callbackQuery = update.CallbackQuery;
            // Handle the callback data
            if (callbackQuery != null && callbackQuery.Message != null)
            {
                var cbchatId = callbackQuery.Message.Chat.Id;
                var msgId = callbackQuery.Message.MessageId;
                Console.WriteLine("Received callback query: " + callbackQuery.Data);
                switch (callbackQuery.Data)
                {
                    case "Главная_Расписание":
                        InlineKeyboardMarkup ik_schedule = new(new[]
                        {
                            InlineKeyboardButton.WithCallbackData("ЭТО", "Отделение_ЭТО"),
                            InlineKeyboardButton.WithCallbackData("МТО", "Отделение_МТО"),
                        });
                        await botClient.SendTextMessageAsync(cbchatId, "Выберите отделение.", replyMarkup: ik_schedule, cancellationToken: ct);
                        break;

                    case "Меню_ЭТОМТО_Назад":
                        InlineKeyboardMarkup ik_schedule_edit = new(new[]
                        {
                            InlineKeyboardButton.WithCallbackData("ЭТО", "Отделение_ЭТО"),
                            InlineKeyboardButton.WithCallbackData("МТО", "Отделение_МТО"),
                        });
                        await botClient.EditMessageReplyMarkupAsync(cbchatId, msgId, replyMarkup: ik_schedule_edit, cancellationToken: ct);
                        break;

                    case "Отделение_ЭТО":
                        InlineKeyboardMarkup ik_dnevzaoch_eto = new(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Дневное отделение", "Дневное_отделение_ЭТО"),
                                InlineKeyboardButton.WithCallbackData("Заочное отделение", "Заочное_отделение_ЭТО") 
                            },
                            new[]{ InlineKeyboardButton.WithCallbackData("← Назад", "Меню_ЭТОМТО_Назад") }
                        });
                        await botClient.EditMessageReplyMarkupAsync(cbchatId, msgId, replyMarkup: ik_dnevzaoch_eto, ct);
                        break;

                    case "Отделение_МТО":
                        InlineKeyboardMarkup ik_dnevzaoch_mto = new(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Дневное отделение", "Дневное_отделение_МТО"),
                                InlineKeyboardButton.WithCallbackData("Заочное отделение", "Заочное_отделение_МТО")
                            },
                            new[]{ InlineKeyboardButton.WithCallbackData("← Назад", "Меню_ЭТОМТО_Назад") }
                        });
                        await botClient.EditMessageReplyMarkupAsync(cbchatId, msgId, replyMarkup: ik_dnevzaoch_mto, ct);
                        break;

                    case "Дневное_отделение_ЭТО":
                        await ResponseHelpers.LoadScheduleList("80-2", "86-2", cbchatId, msgId, botClient, ct);
                        break;

                    case "Заочное_отделение_ЭТО":
                        await ResponseHelpers.LoadScheduleList("80-2", "90-2", cbchatId, msgId, botClient, ct);
                        break;

                    case "Дневное_отделение_МТО":
                        await ResponseHelpers.LoadScheduleList("82-2", "88-2", cbchatId, msgId, botClient, ct);
                        break;

                    case "Заочное_отделение_МТО":
                        await ResponseHelpers.LoadScheduleList("82-2", "92-2", cbchatId, msgId, botClient, ct);
                        break;

                    default:
                        await botClient.SendTextMessageAsync(cbchatId, "Что-то пошло не так... Неверный запрос.", cancellationToken: ct);
                        break;
                }
            }
        }
    }
}
