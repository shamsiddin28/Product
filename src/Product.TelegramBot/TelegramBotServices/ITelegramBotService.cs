using Product.Service.ViewModels.ProductViewModels;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Product.TelegramBot.TelegramBotServices
{
    public interface ITelegramBotService
    {
        Task<List<ProductViewModel>> GetProductsAsync();
        Task<Stream> DownloadVideoStreamAsync(string videoPartPath);
        Task<List<(long Id, int SortNumber)>> GetPropertiesForAllProductsAsync();
        Task<(string videoData, string videoFileName, long sortNumber)> GetVideoDataOfProduct(long id);

        Task StartBotAsync();
        Task<InlineKeyboardMarkup> GetInlineKeyboard();
        Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);
    }
}
