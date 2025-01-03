﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADSCrossPlatform.Code.Models
{
    public class AddressModel : INotifyPropertyChanged
    {
        private AddressDBModel _addressDBModel;

        public Guid Id
        {
            get { return _addressDBModel.Id; }
            set
            {
                _addressDBModel.Id = value;
                OnPropertyChanged("Id");
            }
        }

        public string ProductID
        {
            get { return _addressDBModel.ProductID; }
            set
            {
                _addressDBModel.ProductID = value;
                OnPropertyChanged("ProductID");
            }
        }

        public string ProductName
        {
            get { return _addressDBModel.ProductName; }
            set
            {
                _addressDBModel.ProductName = value;
                OnPropertyChanged("ProductName");
            }
        }

        public string StoreID
        {
            get { return _addressDBModel.StoreID; }
            set
            {
                _addressDBModel.StoreID = value;
                OnPropertyChanged("StoreID");
            }
        }

        public string Article
        {
            get { return _addressDBModel.Article; }
            set
            {
                _addressDBModel.Article = value;
                OnPropertyChanged("Article");
            }
        }

        public string Zone
        {
            get { return _addressDBModel.Zone; }
            set
            {
                _addressDBModel.Zone = value;
                OnPropertyChanged("Zone");
            }
        }
        public string Row
        {
            get { return _addressDBModel.Row; }
            set
            {
                _addressDBModel.Row = value;
                OnPropertyChanged("Row");
            }
        }
        public string Place
        {
            get { return _addressDBModel.Place; }
            set
            {
                _addressDBModel.Place = value;
                OnPropertyChanged("Place");
            }
        }
        public string Level
        {
            get { return _addressDBModel.Level; }
            set
            {
                _addressDBModel.Level = value;
                OnPropertyChanged("Level");
            }
        }
        public string Qty
        {
            get { return _addressDBModel.Qty; }
            set
            {
                _addressDBModel.Qty = value;
                OnPropertyChanged("Qty");
            }
        }

        public bool IsPrimaryPlace
        {
            get { return _addressDBModel.IsPrimaryPlace; }
            set
            {
                _addressDBModel.IsPrimaryPlace = value;
                OnPropertyChanged("IsPrimaryPlace");
            }
        }

        public bool IsSalesLocation
        {
            get { return _addressDBModel.IsSalesLocation; }
            set
            {
                _addressDBModel.IsSalesLocation = value;
                OnPropertyChanged("IsSalesLocation");
            }
        }

        public byte[] RowVersion
        {
            get { return _addressDBModel.RowVersion; }
            set
            {
                _addressDBModel.RowVersion = value;
                OnPropertyChanged("RowVersion");
            }
        }

        public AddressDBModel AddressDBModel { get => _addressDBModel; set => _addressDBModel = value; }

        public AddressModel(AddressDBModel addressDBModel)
        {
            _addressDBModel = addressDBModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
