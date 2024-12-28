using ADS.Code.Models;
using ADSCrossPlatform.Code.Models;

public class AddressHistoryViewModel
{
    private readonly DataManager _dataManager;

    public string ArticleForHistory { get; set; } = string.Empty;

    public AddressHistoryViewModel(DataManager dataManager)
    {
        _dataManager = dataManager;
    }

    public async Task<IEnumerable<HistoryFormated>> GetHistoryByArticle(string article)
    {
        return await _dataManager.GetHistoryByArticleAsync(article) ?? new List<HistoryFormated>();
    }
}

