using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Bim
{
    public class Contact : INotifyPropertyChanged
    {
        private string from;
        private string to;
        private string member1_ID;
        private string member2_ID;

        public string From
        {
            get => from;
            set
            {
                from = value;
                OnPropertyChanged("From");
            }
        }
        public string To
        {
            get => to;
            set
            {
                to = value;
                OnPropertyChanged("To");
            }
        }
        public string Member1_ID
        {
            get => member1_ID;
            set
            {
                member1_ID = value;
                OnPropertyChanged("Member1_ID");
            }
        }
        public string Member2_ID
        {
            get => member2_ID;
            set
            {
                member2_ID = value;
                OnPropertyChanged("Member2_ID");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
