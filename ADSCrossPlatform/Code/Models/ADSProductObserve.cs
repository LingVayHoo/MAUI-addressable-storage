using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSCrossPlatform.Code.Models
{
    public class ADSProductObserve
    {
        public ProductAdaptedForADS ProductAdaptedForADSs { get; set; }

        public ADSProductObserve(ProductAdaptedForADS productAdaptedForADS)
        {
            ProductAdaptedForADSs = productAdaptedForADS;
        }

        public string ID
        {
            get => ProductAdaptedForADSs.ID;
            set
            {
                ProductAdaptedForADSs.ID = value;
                OnPropertyChanged(nameof(ID));
            }
        }

        public string Code
        {
            get => ProductAdaptedForADSs.Code;
            set
            {
                ProductAdaptedForADSs.Code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string Article
        {
            get => ProductAdaptedForADSs.Article;
            set
            {
                ProductAdaptedForADSs.Article = value;
                OnPropertyChanged(nameof(Article));
            }
        }

        public string Name
        {
            get => ProductAdaptedForADSs.Name;
            set
            {
                ProductAdaptedForADSs.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public double Price
        {
            get => ProductAdaptedForADSs.Price;
            set
            {
                ProductAdaptedForADSs.Price = value;
                OnPropertyChanged(nameof(Price));
            }
        }

        public string[] Storages
        {
            get => ProductAdaptedForADSs.Storages;
            set
            {
                ProductAdaptedForADSs.Storages = value;
                OnPropertyChanged(nameof(Storages));
            }
        }

        public double[] QtyInStorages
        {
            get => ProductAdaptedForADSs.QtyInStorages;
            set
            {
                ProductAdaptedForADSs.QtyInStorages = value;
                OnPropertyChanged(nameof(QtyInStorages));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
