namespace ADSCrossPlatform.Code.Models
{
    public class SACViewModel
    {
        private List<SAC> _sacs;

        public SACViewModel()
        {
            _sacs = new List<SAC>();
        }



        public List<SAC> Sacs { get => _sacs; set => _sacs = value; }

        public SAC SelectedSAC { get; set; }
    }
}
