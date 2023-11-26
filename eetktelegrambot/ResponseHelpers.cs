using HtmlAgilityPack;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;

namespace eetktelegrambot
{
    internal class ResponseHelpers
    {
        public static string FindText(string tag)
        {
            List<string> lines = new();

            using (var reader = new StreamReader("responses.txt"))
            {
                if (reader == null) return "[файл не найден]";

                var startFound = false;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null) continue;

                    if (!startFound) startFound = line.Contains(tag);
                    else
                    {
                        if (line == null) continue;
                        var isEnd = line.Contains("/" + tag);
                        if (!isEnd) lines.Add(line.Replace("\\t", "\t"));
                        else break;
                    }
                }
            }
            return string.Join('\n',lines) ?? "[нет текста]";
        }

        #region - Расписание -

        public static async Task LoadScheduleList(string otdelenie, string dnev_zaoch, long chatId, int msgId, ITelegramBotClient botClient, CancellationToken ct)
        {
            await botClient.DeleteMessageAsync(chatId, msgId, ct);
            var msgWait = await botClient.SendTextMessageAsync(chatId, "Загрузка расписания...", cancellationToken: ct);
            await ListSchedules(otdelenie, dnev_zaoch, chatId, botClient, ct);
            await botClient.DeleteMessageAsync(chatId, msgWait.MessageId, ct);
        }

        // Расписание
        public static async Task ListSchedules(string otdelenie, string dnev_zaoch, long chatId, ITelegramBotClient botClient, CancellationToken ct)
        {
            try
            {
                await botClient.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing, cancellationToken: ct);
                HtmlWeb hw = new();
                HtmlDocument doc = hw.Load($"http://eetk.ru/78-2/{otdelenie}/{dnev_zaoch}/"); //мто дневное
                string header = "";
                bool gotHeader = false;

                var nodes = doc.DocumentNode.SelectNodes("//strong");
                if (nodes == null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Ошибка поиска расписания.", cancellationToken: ct);
                    return;
                }

                foreach (HtmlNode node in nodes)
                {
                    if (gotHeader) break;
                    if (node.InnerText.StartsWith("Расписание"))
                    {
                        header += node.InnerText;
                        if (node.NextSibling.Name == "strong")
                        {
                            header += node.NextSibling.InnerText.Replace("&nbsp;", "");
                        }
                        gotHeader = true;
                    }
                }

                if (string.IsNullOrEmpty(header))
                {
                    await botClient.SendTextMessageAsync(chatId, "Ошибка поиска расписания.", cancellationToken: ct);
                    return;
                }

                // Расписание
                List<InlineKeyboardButton> buttons = new();

                var table = doc.DocumentNode.SelectNodes("//table")?.First();
                if (table == null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Похоже, расписания ещё нет.", cancellationToken: ct);
                    return;
                }

                foreach (HtmlNode node in table.SelectNodes(".//a[@href]"))
                {
                    HtmlAttribute att = node.Attributes["href"];
                    if (!att.Value.Contains("cloud")) continue;
                    buttons.Add(InlineKeyboardButton.WithUrl(node.InnerText.Trim(), att.Value));
                }
                InlineKeyboardMarkup inlineKeyboard = new(buttons);
                await botClient.SendTextMessageAsync(chatId, header, replyMarkup: inlineKeyboard, cancellationToken: ct);



                // Изменения в расписании
                string link = otdelenie == "80-2" ? "http://eetk.ru/78-2/2953-2/3112-2/" : "http://eetk.ru/78-2/2953-2/3115-2/";
                HtmlDocument izmdoc = hw.Load(link); //изменения мто

                var izm = izmdoc.DocumentNode.SelectNodes("//article")?.First();

                if (izm == null)
                {
                    await botClient.SendTextMessageAsync(chatId, "Изменений в расписании нет.", cancellationToken: ct);
                    return;
                }

                foreach (HtmlNode span in izm.Descendants("span"))
                {
                    if (span.InnerText.Contains("Изменения") && (span.InnerText.Contains("МТО") || span.InnerText.Contains("ЭТО")))
                    {
                        await ScheduleChangesMessage(span, botClient, chatId, ct);
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Изменения в расписании
        public static async Task ScheduleChangesMessage(HtmlNode span, ITelegramBotClient botClient, long chatId, CancellationToken ct)
        {
            try
            {
                List<HtmlNode> ps = new();
                HtmlNode nextNode;
                nextNode = span.ParentNode.NextSibling.NextSibling; // wtf
                while (nextNode?.Name == "p")
                {
                    ps.Add(nextNode);
                    nextNode = nextNode.NextSibling.NextSibling; // wtf
                }

                List<InlineKeyboardButton[]> buttons = new();
                foreach (HtmlNode p in ps)
                {
                    var node = p.SelectSingleNode(".//a[@href]");
                    HtmlAttribute att = node.Attributes["href"];
                    buttons.Add(new[] { InlineKeyboardButton.WithUrl(node.InnerText.Trim().Replace("( ", "(").Replace(" )", ")").Replace(" студентов ", ""), att.Value) });
                }
                InlineKeyboardMarkup inlineKeyboard = new(buttons);
                await botClient.SendTextMessageAsync(chatId, span.InnerText.Trim(), replyMarkup: inlineKeyboard, cancellationToken: ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion
    }
}
