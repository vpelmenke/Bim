using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Bim
{
    public class MainViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        private int page = 0;
        public MainViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Contacts = new ObservableCollection<Contact>();
        }

        private Command forwardCommand;
        private Command backCommand;
        private Command averageCommand;
        private Command showCommand;
        public Command ForwardCommand // Переход на следующую страницу
        {
            get
            {
                return forwardCommand ??
                       (forwardCommand = new Command(obj =>
                       {
                           Persons = new ObservableCollection<Person>();
                           Contacts = new ObservableCollection<Contact>();
                           page++;
                           using (StreamReader r = new StreamReader("big_data_persons.json"))
                           {
                               string input = r.ReadToEnd();
                               var a = JsonConvert.DeserializeObject<ObservableCollection<Person>>(input);
                               Persons = PagingForPersons(10, a);
                               OnPropertyChanged("Persons");
                           }
                           using (StreamReader r = new StreamReader("big_data_contracts.json"))
                           {
                               string input = r.ReadToEnd();
                               var b = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(input);
                               Contacts = PagingForContacts(10, b);
                               OnPropertyChanged("Contacts");
                           }
                       }));
            }
        }
        public Command BackCommand // Переход на прошлую страницу
        {
            get
            {
                return backCommand ??
                       (backCommand = new Command(obj =>
                       {
                           Persons = new ObservableCollection<Person>();
                           Contacts = new ObservableCollection<Contact>();
                           if (page > 0)page--;
                           using (StreamReader r = new StreamReader("big_data_persons.json"))
                           {
                               string input = r.ReadToEnd();
                               var a = JsonConvert.DeserializeObject<ObservableCollection<Person>>(input);
                               Persons = PagingForPersons(10, a);
                               OnPropertyChanged("Persons");
                           }

                           using (StreamReader r = new StreamReader("big_data_contracts.json"))
                           {
                               string input = r.ReadToEnd();
                               var b = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(input);
                               Contacts = PagingForContacts(10, b);
                               OnPropertyChanged("Contacts");
                           }
                       }));
            }
        }

        public Command AverageCommand // Расчет среднего возраста
        {
            get
            {
                return averageCommand ?? 
                       (averageCommand = new Command(obj =>
                       {
                           var count = 0;
                           var age = 0;
                           using (StreamReader r = new StreamReader("big_data_persons.json"))
                           {
                               string input = r.ReadToEnd();
                               var a = JsonConvert.DeserializeObject<ObservableCollection<Person>>(input);
                               foreach (var VARIABLE in a)
                               {
                                   var name = VARIABLE.Name.Split(' ');
                                   if (name[1] == UserName)
                                   {
                                       count++;
                                       age += Convert.ToInt32(VARIABLE.Age);
                                   }
                               }

                               if (count > 0)
                               {
                                   var result = age / count;
                                   AverAge = result.ToString();
                                   OnPropertyChanged("AverAge");
                               }
                           }
                       }));
            }
        }

        public Command ShowCommand // Отображение контактов в указанном интервале, которые длились > 10 минут.
        {
            get
            {
                return showCommand ??
                       (showCommand = new Command(obj =>
                       {
                           using (StreamReader r = new StreamReader("big_data_contracts.json"))
                           {
                               string input = r.ReadToEnd();
                               var b = JsonConvert.DeserializeObject<ObservableCollection<Contact>>(input);
                               Contacts.Clear();
                               foreach (var VARIABLE in b)
                               {
                                   var dateFrom = Convert.ToDateTime(VARIABLE.From);
                                   var dateTo = Convert.ToDateTime(VARIABLE.To);
                                   if (dateFrom >= DateIn && dateTo <= DateOut)
                                       if (dateTo.Subtract(dateFrom).Minutes > 10)
                                           Contacts.Add(VARIABLE);

                                   if(Contacts.Count == 10) break; // Здесь нужно написать нормальный обработчик вывода.
                               }
                               OnPropertyChanged("Contacts");
                           }
                       }));
            }
        }

        private Person currentPerson;
        private Contact currentContact;
        private string userName;
        private string averAge;
        private DateTime dateIn = DateTime.Today;
        private DateTime dateOut = DateTime.Today;

        public DateTime DateIn
        {
            get => dateIn;
            set
            {
                dateIn = value;
                OnPropertyChanged("DateIn");
            }
        }
        public DateTime DateOut
        {
            get => dateOut;
            set
            {
                dateOut = value;
                OnPropertyChanged("DateOut");
            }
        }

        public string UserName
        {
            get => userName;
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string AverAge
        {
            get => averAge;
            set
            {
                averAge = value;
                OnPropertyChanged("AverAge");
            }
        }

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


        private ObservableCollection<Person> PagingForPersons(int number, ObservableCollection<Person> t)
        {
            ObservableCollection<Person> persons = new ObservableCollection<Person>();
            persons = new ObservableCollection<Person>(t.Take(number * page).Skip(number * (page - 1)));
            return persons;
        }

        private ObservableCollection<Contact> PagingForContacts(int number, ObservableCollection<Contact> t)
        {
            ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();
            contacts = new ObservableCollection<Contact>(t.Take(number * page).Skip(number * (page - 1)));
            return contacts;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
