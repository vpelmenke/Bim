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
        private Person[] _allPersons; 
        private Contact[] _allContacts;
        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        private int _page = 1; // номер текущей отображаемой страницы

        public MainViewModel()
        {
            Persons = new ObservableCollection<Person>();
            Contacts = new ObservableCollection<Contact>();
        }

        private Command _forwardCommand;
        private Command _backCommand;
        private Command _averageCommand;
        public Command ForwardCommand // Переход на следующую страницу
        {
            get
            {
                return _forwardCommand ??
                       (_forwardCommand = new Command(obj =>
                       {

                           _page++;
                           Contacts.Clear();
                           Persons.Clear();

                           if (FilterEnabled)
                           {
                               var filterArray = _allContacts.Where(x =>
                                   Convert.ToDateTime(x.From) >= DateIn && Convert.ToDateTime(x.To) <= DateOut
                                                                        && Convert.ToDateTime(x.To)
                                                                            .Subtract(Convert.ToDateTime(x.From)).Minutes >
                                                                        10).ToArray();// Фильтр контактов дольше 10 минут
                               var result = Paging(10, filterArray); // Итоговое отображение отфильтрованных данных
                               foreach (var row in result)
                               {
                                   Contacts.Add(row);
                               }
                           }
                           else
                           {
                               var contacts = Paging(10, _allContacts);
                               foreach (var row in contacts)
                               {
                                   Contacts.Add(row);
                               }
                           }

                           if (!Contacts.Any())
                           {
                               _backCommand.Execute(null);
                               return;
                           }

                           var persons = Paging(10, _allPersons);
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
                return _backCommand ??
                       (_backCommand = new Command(obj =>
                       {

                           if (_page <= 1) return;
                           _page--;
                           Contacts.Clear();
                           Persons.Clear();

                           if (FilterEnabled)
                           {
                               var filterArray = _allContacts.Where(x =>
                                   Convert.ToDateTime(x.From) >= DateIn && Convert.ToDateTime(x.To) <= DateOut
                                                                        && Convert.ToDateTime(x.To)
                                                                            .Subtract(Convert.ToDateTime(x.From)).Minutes >
                                                                        10).ToArray(); // Фильтр контактов больше 10 минут
                               var result = Paging(10, filterArray); // Итоговое отображение отфильтрованных данных
                               foreach (var row in result)
                               {
                                   Contacts.Add(row);
                               }
                           }
                           else
                           {
                               var contacts = Paging(10, _allContacts);
                               foreach (var row in contacts)
                               {
                                   Contacts.Add(row);
                               }
                           }

                           var persons = Paging(10, _allPersons);
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
                return _averageCommand ??
                       (_averageCommand = new Command(obj =>
                       {
                           var count = 0;
                           var age = 0;
                           foreach (var row in _allPersons)
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

        private Person _currentPerson;
        private Contact _currentContact;
        private string _userName;
        private string _averAge;
        private DateTime _dateIn = DateTime.Today;
        private DateTime _dateOut = DateTime.Today;
        private bool _filterEnabled; // нужно ли применять фильтрацию по датам

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
            get => _dateIn;
            set
            {
                _dateIn = value;
                OnPropertyChanged("DateIn");
            }
        }
        public DateTime DateOut
        {
            get => _dateOut;
            set
            {
                _dateOut = value;
                OnPropertyChanged("DateOut");
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string AverAge
        {
            get => _averAge;
            set
            {
                _averAge = value;
                OnPropertyChanged("AverAge");
            }
        }

        public Person CurrentPerson
        {
            get => _currentPerson;
            set
            {
                _currentPerson = value;
                OnPropertyChanged("CurrentPerson");
            }
        }

        public Contact CurrentContact
        {
            get => _currentContact;
            set
            {
                _currentContact = value;
                OnPropertyChanged("CurrentContact");
            }
        }

        private T[] Paging<T>(int number, T[] t)
        {
            var rows = t.Take(number * _page).Skip(number * (_page - 1)).ToArray();
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
                _allPersons = JsonConvert.DeserializeObject<Person[]>(input);
            }

            using (StreamReader r = new StreamReader("big_data_contracts.json"))
            {
                string input = r.ReadToEnd();
                _allContacts = JsonConvert.DeserializeObject<Contact[]>(input);
            }

            var contacts = Paging(10, _allContacts);
            foreach (var row in contacts)
            {
                Contacts.Add(row);
            }

            var persons = Paging(10, _allPersons);
            foreach (var row in persons)
            {
                Persons.Add(row);
            }
        }
    }
}