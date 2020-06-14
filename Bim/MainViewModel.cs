using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Bim
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        private int page = 2;

        public MainViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Contacts = new ObservableCollection<Contact>();

            using (StreamReader r = new StreamReader("big_data_persons.json"))
            {
                string input = r.ReadToEnd();
                var t = JsonConvert.DeserializeObject<ObservableCollection<Person>>(input);
                Persons = Paging(10, t);
            }
            using (StreamReader r = new StreamReader("big_data_contracts.json"))
            {
                string input = r.ReadToEnd();
                var t = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(input);
                Contacts = new ObservableCollection<Contact>(t.Take(100));
            }

        }

        private Command forwardCommand;
        private Command backCommand;
        private Command goCommand;
        public Command ForwardCommand
        {
            get
            {
                return forwardCommand ??
                       (forwardCommand = new Command(obj =>
                       {
                           Persons = new ObservableCollection<Person>();

                           using (StreamReader r = new StreamReader("big_data_persons.json"))
                           {
                               string input = r.ReadToEnd();
                               var t = JsonConvert.DeserializeObject<ObservableCollection<Person>>(input);
                               page++;
                               Persons = Paging(10, t);
                           }
                       }));
            }
        }
        public Command BackCommand
        {
            get
            {
                return backCommand ??
                       (backCommand = new Command(obj =>
                       {
                           page--;
                       }));
            }
        }

        private Person currentPerson;
        private Contact currentContact;
        public Person CurrentPerson
        {
            get => currentPerson;
            set
            {
                currentPerson = value;
                OnPropertyChanged("CurrentPerson");
            }
        }

        public Contact CurrentContact
        {
            get => currentContact;
            set
            {
                currentContact = value;
                OnPropertyChanged("CurrentContact");
            }
        }


        private ObservableCollection<Person> Paging(int number, ObservableCollection<Person> t)
        {
            ObservableCollection<Person> persons = new ObservableCollection<Person>();
            persons = new ObservableCollection<Person>(t.Take(number * page).Skip(number * (page - 1)));
            return persons;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
