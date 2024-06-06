using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADSCrossPlatform.Code.Models
{
    public class AddressViewModel(DataManager dataManager) : INotifyPropertyChanged
    {
        private AddressModel? _selectedAddressModel;
        private DataManager _dataManager = dataManager;
        private string _article;

        public ObservableCollection<AddressModel> Addresses        
        {
            get
            {
                //return _dataManager.GetAllData();
                return _dataManager.GetDataByArticle(Article);
            }
            set { }
        }

        public string Article { get => _article; set => _article = value; }

        internal AddressModel? SelectedAddressModel
        {
            get => _selectedAddressModel;
            set
            {
                _selectedAddressModel = value;
                OnPropertyChanged("SelectedModel");
            }
        }

        public AddressDBModel addressDBModel
        {
            get
            {
                return _dataManager.addressDBModel;
            }
        }

        public void UpdateArticle(string article)
        {
            Article = article;
            OnPropertyChanged("Addresses");
        }

        public string[] GetAllInfo(string article, bool isFindByArticle)
        {
            return _dataManager.GetAllInfo(article, isFindByArticle);
        }

        public async Task<bool> CreateRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.CreateRecord(addressDBModel);
            OnPropertyChanged("Addresses");
            return result;
        }

        public async Task<bool> EditRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.EditRecord(addressDBModel);
            OnPropertyChanged("Addresses");
            return result;
        }

        public async Task<bool> DeleteRecord(AddressDBModel addressDBModel)
        {
            bool result = await _dataManager.DeleteRecord(addressDBModel);
            OnPropertyChanged("Addresses");
            return result;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
