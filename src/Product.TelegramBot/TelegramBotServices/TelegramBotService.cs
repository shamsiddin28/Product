using Product.Service.Interfaces.Products;
using Product.Service.ViewModels.ProductViewModels;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Product.TelegramBot.TelegramBotServices
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly TelegramBotClient _botClient;
        private readonly IProductService _productService;

        public TelegramBotService(string botToken, IProductService productService)
        {
            _botClient = new TelegramBotClient(botToken);
            _productService = productService;
        }

        public async Task<Stream> DownloadVideoStreamAsync(string videoPartPath)
        {
            try
            {
                // Call the DownloadAsync method to download the video file as a byte array
                byte[] videoBytes = await _productService.DownloadAsync(videoPartPath);

                // Convert the byte array into a memory stream
                MemoryStream stream = new MemoryStream(videoBytes);

                // Set the position of the stream to the beginning
                stream.Seek(0, SeekOrigin.Begin);
                Console.WriteLine(stream.ToString() + "\nDownloaded from service DownloadAsync ...");
                return stream;
            }
            catch (FileNotFoundException)
            {
                throw; // Rethrow the exception if the file is not found
            }
        }

        public async Task<List<ProductViewModel>> GetProductsAsync()
        {
            try
            {
                var results = await _productService.RetrieveAllProductsAsync();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<(long Id, int SortNumber)>> GetPropertiesForAllProductsAsync()
        {
            try
            {
                var products = await _productService.RetrieveAllProductsAsync();
                return products
                        .Select(product => (product.Id, product.SortNumber))
                        .ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<(string videoData, string videoFileName, long sortNumber)> GetVideoDataOfProduct(long id)
        {
            var product = await _productService.RetrieveByIdAsync(id);
            if (product != null)
            {
                string videoFileName = Path.GetFileName(product!.VideoData);

                return (product.VideoData, videoFileName, product.SortNumber);
            }
            else { return ("", "", 0); }
        }

        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var sentMessage = new Message();
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var chatId = update.Message.Chat.Id;
                var messageText = update.Message.Text;
                var firstName = update.Message.Chat.FirstName;
                Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
                if (messageText == "/start")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Hello {firstName} !\nWelcome to Product.",
                        cancellationToken: cancellationToken);

                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: $"Choose a SortNumber:",
                        replyMarkup: await GetInlineKeyboard(),
                        cancellationToken: cancellationToken);
                }
            }

            if (update.CallbackQuery != null)
            {
                var properties = await GetPropertiesForAllProductsAsync();
                if (properties.Any(p => p.Id.ToString() == update.CallbackQuery.Data))
                {
                    sentMessage = await botClient.SendTextMessageAsync(
                         chatId: update.CallbackQuery.Message!.Chat.Id,
                         text: "sending video...",
                         cancellationToken: cancellationToken
                     );

                    (string videoData, string videoFileName, long sortNumber) = await GetVideoDataOfProduct(long.Parse(update.CallbackQuery.Data!));
                    Stream videoStream = await DownloadVideoStreamAsync(videoData);

                    Message message = await botClient.SendVideoAsync(
                         chatId: update.CallbackQuery.Message!.Chat.Id,
                         video: InputFile.FromStream(videoStream, $"{videoFileName}"),
                         replyToMessageId: update.CallbackQuery.Message.MessageId,
                         caption: "sortnumber: " + sortNumber.ToString(),
                         supportsStreaming: true,
                         cancellationToken: cancellationToken);
                    Console.WriteLine(message.Chat.Id);
                    // Delete the SMS message
                    await botClient.DeleteMessageAsync(
                        chatId: update.CallbackQuery.Message!.Chat.Id,
                        messageId: sentMessage.MessageId,
                        cancellationToken: cancellationToken
                    );
                }
            }
        }

        public async Task<InlineKeyboardMarkup> GetInlineKeyboard()
        {
            // Sample list of items
            List<(long id, int sortNumber)> items = await GetPropertiesForAllProductsAsync();

            // Define the maximum number of buttons per row
            int buttonsPerRow = 2;

            // Create the inline keyboard rows using LINQ
            var rows = items
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / buttonsPerRow)
                .Select(group => group.Select(x =>
                    InlineKeyboardButton.WithCallbackData(text: x.item.sortNumber.ToString(), callbackData: x.item.id.ToString())))
                .ToList();

            // Create the inline keyboard markup
            InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(rows);

            return inlineKeyboard;
        }

        public async Task StartBotAsync()
        {
            var me = await _botClient.GetMeAsync();
            Console.WriteLine($"Bot started: {me.Username}");

            using CancellationTokenSource cts = new();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };


            _botClient.StartReceiving(updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token);

            Console.WriteLine("Press any key to exit");
            Console.Read();

            cts.Cancel();
        }
    }
}
