﻿using ADSCrossPlatform.Code.Models;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;


namespace ADSCrossPlatform
{
    public partial class MainPage : ContentPage
    {
        //int count = 0;
        //private DataManager _dataManager;
        //private AddressViewModel _addressViewModel;

        public MainPage()
        {
            InitializeComponent();            
            //header.Text = "Управление складскими/торговыми местами";
            //_dataManager = new(new WebApiModel());
            //_addressViewModel = new();
            //BindingContext = _addressViewModel;
            
            //windowPicker.SelectedIndex = 1;

        }

        //private void Button_Clicked(object sender, EventArgs e)
        //{
            

        //    if (Search(ArticleField.Text, true)) return;
        //    if (Search(ArticleField.Text, false)) return;

        //    //Попытка поиска с измененными данными по артикулу и имени
        //    ArticleField.Text = ArticleField.Text.Replace(".", null);
        //    if (Search(ArticleField.Text, true)) return;

        //    ArticleField.Text = ArticleField.Text.Replace(".", null);
        //    if (Search(ArticleField.Text, false)) return;
        //}

        //private bool Search(string art, bool isFindByArticle)
        //{
        //    _addressViewModel.UpdateArticle(art);
        //    ArtNameField.Text = _addressViewModel.GetAllInfo(art, isFindByArticle)[0];
        //    ArtQtyField.Text = _addressViewModel.GetAllInfo(art, isFindByArticle)[1];

        //    return !(ArtNameField.Text == string.Empty);
        //}

        //private void DataListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{

        //}

        //private void DataListView_ItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    if (_addressViewModel.SelectedAddressModel != null &&
        //        _addressViewModel.SelectedAddressModel == e.Item as AddressModel)
        //    {
        //        ToDetailsPage(sender, e);
                
        //    }
        //    else _addressViewModel.SelectedAddressModel = e.Item as AddressModel;
        //}

        //private async void ToDetailsPage(object? sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new Details
        //        (_addressViewModel,
        //        Convert(_addressViewModel.SelectedAddressModel),
        //        false));
        //}

        //private async void ToNewPage(object? sender, EventArgs e)
        //{
        //    var newRecord = _addressViewModel.addressDBModel;
        //    newRecord.Article = ArticleField.Text;
        //    await Navigation.PushAsync(new Details
        //       (_addressViewModel, newRecord,
        //       true));
        //}

        //private AddressDBModel Convert(AddressModel addressModel)
        //{
        //    return new AddressDBModel
        //    {
        //        Id = addressModel.Id,
        //        Article = addressModel.Article,
        //        Zone = addressModel.Zone,
        //        Row = addressModel.Row,
        //        Place = addressModel.Place,
        //        Level = addressModel.Level,
        //        Qty = addressModel.Qty
        //    };
        //}

        //private void Create_Clicked(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrEmpty(ArticleField.Text)) return;
        //    ToNewPage(sender, e);

        //}

        //private async void PickerSelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (windowPicker.SelectedItem.ToString() == "31") await Shell.Current.GoToAsync("Window_310");
        //    //await Navigation.PushAsync(new Window_310());

        //}

        ////private void OnCounterClicked(object sender, EventArgs e)
        ////{
        ////    //count++;

        ////    //if (count == 1)
        ////    //    CounterBtn.Text = $"Clicked {count} time";
        ////    //else
        ////    //    CounterBtn.Text = $"Clicked {count} times";

        ////    //SemanticScreenReader.Announce(CounterBtn.Text);
        ////}
    }

}
