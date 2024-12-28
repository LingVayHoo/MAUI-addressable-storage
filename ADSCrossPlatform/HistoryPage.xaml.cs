using ADS.Code.Models;
using System.Collections.ObjectModel;

namespace ADSCrossPlatform;

public partial class HistoryPage : ContentPage
{
    private readonly AddressHistoryViewModel _viewModel;

    public HistoryPage(AddressHistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        ArticleEntry.Text = _viewModel.ArticleForHistory;
        LoadHistory(ArticleEntry.Text).ConfigureAwait(false);
    }

    private async Task LoadHistory(string article)
    {
        // Загрузка данных из API
        var historyFormated = await _viewModel.GetHistoryByArticle(article);
        HistoryListView.ItemsSource = historyFormated;
    }

}