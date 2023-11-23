using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using eetktelegrambot;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using Telegram.Bot.Requests;

Console.OutputEncoding = Encoding.UTF8; // вывод кириллицы

var botClient = new TelegramBotClient("6944624888:AAErnEw1jxtbjaaAzyqxs96BPhSa6mJzDJ8"); // токен

using CancellationTokenSource cts = new();
ReceiverOptions receiverOptions = new() { AllowedUpdates = Array.Empty<UpdateType>() };
botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, receiverOptions, cts.Token);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

// responding to messages
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
{
    var callbackQuery = update.CallbackQuery;

    // Handle the callback data
    if (callbackQuery != null && callbackQuery.Message != null)
    {
        var cbchatId = callbackQuery.Message.Chat.Id;
        var msgId = callbackQuery.Message.MessageId;
        Message msgWait;
        Console.WriteLine("Received callback query: " + callbackQuery.Data);
        switch (callbackQuery.Data) 
        {
            case "Отделение_ЭТО":
                InlineKeyboardMarkup ik_dnevzaoch_eto = new(new[]
                {
                    InlineKeyboardButton.WithCallbackData("Дневное отделение", "Дневное_отделение_ЭТО"),
                    InlineKeyboardButton.WithCallbackData("Заочное отделение", "Заочное_отделение_ЭТО"),
                });
                await botClient.EditMessageReplyMarkupAsync(cbchatId, msgId, replyMarkup: ik_dnevzaoch_eto, ct);
                break;
            case "Отделение_МТО":
                InlineKeyboardMarkup ik_dnevzaoch_mto = new(new[]
                {
                    InlineKeyboardButton.WithCallbackData("Дневное отделение", "Дневное_отделение_МТО"),
                    InlineKeyboardButton.WithCallbackData("Заочное отделение", "Заочное_отделение_МТО"),
                });
                await botClient.EditMessageReplyMarkupAsync(cbchatId, msgId, replyMarkup: ik_dnevzaoch_mto, ct);
                break;
            case "Дневное_отделение_ЭТО":
                await botClient.DeleteMessageAsync(cbchatId, msgId, ct);
                msgWait = await botClient.SendTextMessageAsync(cbchatId, "Загрузка расписания...", cancellationToken: ct);
                await ResponseHelpers.ListSchedules("80-2", "86-2", botClient, cbchatId, ct);
                await botClient.DeleteMessageAsync(cbchatId, msgWait.MessageId, ct);
                break;
            case "Заочное_отделение_ЭТО":
                await ResponseHelpers.ListSchedules("80-2", "90-2", botClient, cbchatId, ct);
                break;
            case "Дневное_отделение_МТО":
                await botClient.DeleteMessageAsync(cbchatId, msgId, ct);
                msgWait = await botClient.SendTextMessageAsync(cbchatId, "Загрузка расписания...", cancellationToken: ct);
                await ResponseHelpers.ListSchedules("82-2", "88-2", botClient, cbchatId, ct);
                await botClient.DeleteMessageAsync(cbchatId, msgWait.MessageId, ct);
                break;
            case "Заочное_отделение_МТО":
                await ResponseHelpers.ListSchedules("82-2", "92-2", botClient, cbchatId, ct);
                break;
            default:
                await botClient.SendTextMessageAsync(cbchatId, "Что-то пошло не так... Неверный запрос.", cancellationToken: ct);
                break;
         } 
    }

    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message) return;

    // Only process text messages
    if (message.Text is not { } messageText) return;

    var chatId = message.Chat.Id;

    Log(message);

    ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]{new KeyboardButton[] { "🏠 На главную 🏠", "📅 Расписание 📅" } }) { ResizeKeyboard = true };

    //Message sentMessage = await botClient.SendTextMessageAsync(chatId, "Choose a response", replyMarkup: replyKeyboardMarkup, cancellationToken: ct);

    switch (messageText)
    {
        case "абоба":
        case "сам абоба":
        case "сам абоба.":
            await botClient.SendTextMessageAsync(chatId, "сам абоба.", cancellationToken: ct);
            break;

        case "📅 Расписание 📅":
        case "Расписание":
        case "расписание":
            InlineKeyboardMarkup ik_etomto = new(new[]
            {
                InlineKeyboardButton.WithCallbackData("ЭТО", "Отделение_ЭТО"),
                InlineKeyboardButton.WithCallbackData("МТО", "Отделение_МТО"),
            });
            await botClient.SendTextMessageAsync(chatId, "Выберите отделение.", replyMarkup: ik_etomto, cancellationToken: ct);
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
            await botClient.SendTextMessageAsync(chatId, "🏠", replyMarkup: ik_main, cancellationToken: ct);
            break;

        default:
            await botClient.SendTextMessageAsync(chatId, "Неизвестная команда.", cancellationToken: ct);
            break;
    }

    //await ResponseHelpers.ListSchedules("82-2","88-2",botClient,chatId,ct);
}






void Log(Message message)
{
    if (message.From == null)
    {
        Console.WriteLine("Received a message from a null user.");
        return;
    }
    else Console.WriteLine($"[Message]({message.Date.ToLocalTime()}) | {message.From.FirstName} ({message.From.Username}): {message.Text} ");
}


// error handling
Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}", 
                                              _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}