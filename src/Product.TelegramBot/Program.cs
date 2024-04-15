using Product.TelegramBot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private readonly static string token = "7174140036:AAGHuKCi0Bysxi0xUEyAj2R2DV1na_nNnYo";
    private static TelegramBotClient botClient;

    static async Task Main(string[] args)
    {

        botClient = new TelegramBotClient(token);

        var controlProduct = new ControlProduct();

        var properties = await controlProduct.GetPropertiesForAllProductsAsync();

        var me = await botClient.GetMeAsync();

        //Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");


        using CancellationTokenSource cts = new();

        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        // Send cancellation request to stop bot
        cts.Cancel();



        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                var firstName = update.Message.Chat.FirstName;
                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                if (messageText == "/start")
                {
                    await SendInlineKeyboardButton(chatId, cancellationToken);
                    Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "You said: " + messageText,
                        cancellationToken: cancellationToken);
                }
            }

            if (update.CallbackQuery != null)
            {
                if (properties.Any(p => p.Id.ToString() == update.CallbackQuery.Data))
                {
                    (string videoData, string videoFileName) = await controlProduct.GetVideoDatasOfProduct(long.Parse(update.CallbackQuery.Data));
                    Stream videoStream = await controlProduct.DownloadVideoStreamAsync(videoData);

                    Message message = await botClient.SendVideoAsync(
                    chatId: update.CallbackQuery.Message.Chat.Id,
                    video: InputFile.FromStream(videoStream, $"{videoFileName}"),
                    supportsStreaming: true,
                    cancellationToken: cancellationToken);
                }
            }
        }

        async Task SendInlineKeyboardButton(long chatId, CancellationToken cancellationToken)
        {
            // Sample list of items
            List<(long id, int sortNumber)> items = new List<(long id, int sortNumber)>();

            foreach (var (id, sortNumber) in properties)
            {
                items.Add((id, sortNumber));
                //Console.WriteLine($"Product ID: {id}, Sort Number: {sortNumber}");
            }
            // Create a list to hold rows of buttons
            List<List<InlineKeyboardButton>> rows = new List<List<InlineKeyboardButton>>();

            // Define the maximum number of buttons per row
            int buttonsPerRow = 2;

            // Iterate over the items and create buttons dynamically
            for (int i = 0; i < items.Count; i += buttonsPerRow)
            {
                var rowItems = items.Skip(i).Take(buttonsPerRow);
                List<InlineKeyboardButton> row = rowItems
                    .Select(item => InlineKeyboardButton.WithCallbackData(text: item.sortNumber.ToString(), callbackData: item.id.ToString())).ToList();
                rows.Add(row);
            }

            // Create the inline keyboard markup
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(rows);

            // Send the message with the inline keyboard markup
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "A list of Product's SortNumbers available in the database\nChoose which one?",
                replyMarkup: inlineKeyboard,
                cancellationToken: cancellationToken);
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        Console.ReadKey();


    }

}
