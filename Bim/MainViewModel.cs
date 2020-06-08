using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Bim
{
    class MainViewModel:INotifyPropertyChanged
    {
        public List<Person> persons = new List<Person>();
        public MainViewModel()
        {
            using (StreamReader r = new StreamReader("big_data_persons.json"))
            {
                string input = r.ReadToEnd();
                persons = JsonConvert.DeserializeObject<List<Person>>(input);
            }
        }

        private Person currentPerson;
        public Person CurrentPerson
        {
            get => currentPerson;
            set
            {
                currentPerson = value;
                OnPropertyChanged("CurrentPerson");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
