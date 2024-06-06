using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ADSCrossPlatform.Code.Models
{
    public class AddressModelMAUI : INotifyPropertyChanged
    {
        private AddressDBModel _addressDBModel;
        public event PropertyChangedEventHandler? PropertyChanged;



        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
