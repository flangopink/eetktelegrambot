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
                switch (callbackQuery.Data)
                {
                    case "приемная_комиссия":
                        InlineKeyboardMarkup ik_priemnaya = new(new[]
                        {
                            new[] {InlineKeyboardButton.WithCallbackData("Показать на карте", "показать_карту") },
                            new[] {InlineKeyboardButton.WithCallbackData("← Назад", "давай_назад_замена") },
                        });
                        await botClient.EditMessageTextAsync(cbchatId, msgId, ResponseHelpers.FindText("приемная_комиссия"), replyMarkup: ik_priemnaya, cancellationToken: ct);
                        break;

                    case "подача_заявления":
                        InlineKeyboardMarkup ik_podacha = new(new[]
                        {
                            new[] {InlineKeyboardButton.WithUrl("Специальности", "http://eetk.ru/20-2/22-2/") },
                            new[] {InlineKeyboardButton.WithUrl("Правила приема", "https://cloud.mail.ru/public/EnEu/kZ6s4KuLJ") },
                            new[] {InlineKeyboardButton.WithCallbackData("← Назад", "давай_назад") },
                        });
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("подача_заявления"), cancellationToken: ct);
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("подача_заявления_доки"), cancellationToken: ct);
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("подача_заявления_сроки"), replyMarkup: ik_podacha, cancellationToken: ct);
                        break;

                    case "проходные_баллы":
                        InlineKeyboardMarkup ik_prohodnye = new(new[]
                        {
                            new[] {InlineKeyboardButton.WithUrl("Специальности", "http://eetk.ru/20-2/22-2/") },
                            new[] {InlineKeyboardButton.WithCallbackData("← Назад", "давай_назад_замена") },
                        });
                        await botClient.EditMessageTextAsync(cbchatId, msgId, ResponseHelpers.FindText("проходные_баллы"), replyMarkup: ik_prohodnye, cancellationToken: ct);
                        break;

                    case "показать_карту":
                        InlineKeyboardMarkup ik_map = new(new[]
                        {
                            new[] {InlineKeyboardButton.WithCallbackData("← Назад", "давай_назад") },
                        });
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("карта_декабристов"), cancellationToken: ct);
                        await botClient.SendLocationAsync(cbchatId, 56.822558, 60.602406, cancellationToken: ct);
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("карта_ясная"), cancellationToken: ct);
                        await botClient.SendLocationAsync(cbchatId, 56.815173, 60.585653, cancellationToken: ct);
                        await botClient.SendTextMessageAsync(cbchatId, ResponseHelpers.FindText("карта_космонавтов"), cancellationToken: ct);
                        await botClient.SendLocationAsync(cbchatId, 56.890477, 60.614650, replyMarkup: ik_map, cancellationToken: ct);
                        break;

                    case "давай_назад":
                        await MessageResponder.ToMainMenu(cbchatId);
                        break;

                    case "давай_назад_замена":
                        await MessageResponder.ToMainMenu(cbchatId, true, msgId);
                        break;
                    /*
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
                    */

                    default:
                        await botClient.SendTextMessageAsync(cbchatId, "Что-то пошло не так... Неверный запрос.", cancellationToken: ct);
                        break;
                }
            }
        }
    }
}
