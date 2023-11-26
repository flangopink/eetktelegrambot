using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using eetktelegrambot;

Console.OutputEncoding = Encoding.UTF8; // вывод кириллицы

var botClient = new TelegramBotClient("6944624888:AAErnEw1jxtbjaaAzyqxs96BPhSa6mJzDJ8"); // токен

using CancellationTokenSource cts = new();
ReceiverOptions receiverOptions = new() { AllowedUpdates = Array.Empty<UpdateType>() };
botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, receiverOptions, cts.Token);

MessageResponder.Init(botClient, default);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

// responding to messages
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
{
    var callbackQuery = update.CallbackQuery;
    await CallbackResponder.Respond(update, botClient, ct);
    if (callbackQuery != null) LogQ(callbackQuery);

    if (update.Message is not { } message) return;
    if (message.Text is not { } messageText) return;

    var chatId = message.Chat.Id;

    Log(message);
    await MessageResponder.Respond(message, chatId);
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

void LogQ(CallbackQuery query)
{
    if (query.From == null)
    {
        Console.WriteLine("Received a callback from a null user.");
        return;
    }
    else Console.WriteLine($"[Callback]({query.Message?.Date.ToLocalTime()}) | {query.From.FirstName} ({query.From.Username}): {query.Data} ");
}

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