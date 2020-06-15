using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Bim
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Person[] allPersons;
        private Contact[] allContacts;
        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        private int page = 1;

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

                           page++;
                           Contacts.Clear();
                           Persons.Clear();

                           if (FilterEnabled)
                           {
                               var a = allContacts.Where(x =>
                                   Convert.ToDateTime(x.From) >= DateIn && Convert.ToDateTime(x.To) <= DateOut
                                                                        && Convert.ToDateTime(x.To)
                                                                            .Subtract(Convert.ToDateTime(x.From)).Minutes >
                                                                        10).ToArray();
                               var b = Paging(10, a);
                               foreach (var row in b)
                               {
                                   Contacts.Add(row);
                               }
                           }
                           else
                           {
                               var contacts = Paging(10, allContacts);
                               foreach (var row in contacts)
                               {
                                   Contacts.Add(row);
                               }
                           }

                           if (!Contacts.Any())
                           {
                               backCommand.Execute(null);
                               return;
                           }

                           var persons = Paging(10, allPersons);
                           foreach (var row in persons)
                           {
                               Persons.Add(row);
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

                           if (page <= 1) return;
                           page--;
                           Contacts.Clear();
                           Persons.Clear();

                           if (FilterEnabled)
                           {
                               var a = allContacts.Where(x =>
                                   Convert.ToDateTime(x.From) >= DateIn && Convert.ToDateTime(x.To) <= DateOut
                                                                        && Convert.ToDateTime(x.To)
                                                                            .Subtract(Convert.ToDateTime(x.From)).Minutes >
                                                                        10).ToArray();
                               var b = Paging(10, a);
                               foreach (var row in b)
                               {
                                   Contacts.Add(row);
                               }
                           }
                           else
                           {
                               var contacts = Paging(10, allContacts);
                               foreach (var row in contacts)
                               {
                                   Contacts.Add(row);
                               }
                           }

                           var persons = Paging(10, allPersons);
                           foreach (var row in persons)
                           {
                               Persons.Add(row);
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
                           foreach (var row in allPersons)
                           { 
                               var name = row.Name.Split(' '); 
                               if (name[1] == UserName)
                               {
                                   count++;
                                   age += Convert.ToInt32(row.Age);
                               }
                           }

                           if (count > 0)
                           {
                               var result = age / count;
                               AverAge = result.ToString();
                               OnPropertyChanged("AverAge");
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
        private bool _filterEnabled;

        public bool FilterEnabled
        {
            get => _filterEnabled;
            set
            {
                _filterEnabled = value; 
                OnPropertyChanged("FilterEnabled");
            }
        }

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

        private T[] Paging<T>(int number, T[] t)
        {
            var rows = t.Take(number * page).Skip(number * (page - 1)).ToArray();
            return rows;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public void LoadData()
        {
            using (StreamReader r = new StreamReader("big_data_persons.json"))
            {
                string input = r.ReadToEnd();
                allPersons = JsonConvert.DeserializeObject<Person[]>(input);
            }

            using (StreamReader r = new StreamReader("big_data_contracts.json"))
            {
                string input = r.ReadToEnd();
                allContacts = JsonConvert.DeserializeObject<Contact[]>(input);
            }

            var contacts = Paging(10, allContacts);
            foreach (var row in contacts)
            {
                Contacts.Add(row);
            }

            var persons = Paging(10, allPersons);
            foreach (var row in persons)
            {
                Persons.Add(row);
            }
        }
    }
}