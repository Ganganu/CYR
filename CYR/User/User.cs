using CYR.Settings;

namespace CYR.User
{
    public class User
    {
        private readonly UserSettings _userSettings;

        public User(UserSettings userSettings)
        {
            _userSettings = userSettings;

        }
        public string Name { get { return _userSettings.Name; } }
        public string Street { get { return _userSettings.Street; } }
        public string City { get { return _userSettings.City; } }
        public string PLZ { get { return _userSettings.PLZ; } }
        public string HouseNumber { get { return _userSettings.HouseNumber; } }
        public string Telefonnumber { get { return _userSettings.Telefonnumber; } }
        public string EmailAddress { get { return _userSettings.EmailAddress; } }
        public string BankName { get { return _userSettings.BankName; } }
        public string IBAN { get { return _userSettings.IBAN; } }
        public string BIC { get { return _userSettings.BIC; } }
        public string USTIDNr { get { return _userSettings.USTIDNR; } }
        public string STNR { get { return _userSettings.STNR; } }

    }
}
