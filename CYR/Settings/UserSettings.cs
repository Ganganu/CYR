using System.Configuration;

namespace CYR.Settings
{
    public class UserSettings : ConfigurationSection
    {
        [ConfigurationProperty("Name", DefaultValue = "")]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("Street", DefaultValue = "")]
        public string Street
        {
            get { return (string)this["Street"]; }
            set { this["Street"] = value; }
        }

        [ConfigurationProperty("City", DefaultValue = "")]
        public string City
        {
            get { return (string)this["City"]; }
            set { this["City"] = value; }
        }

        [ConfigurationProperty("PLZ", DefaultValue = "")]
        public string PLZ
        {
            get { return (string)this["PLZ"]; }
            set { this["PLZ"] = value; }
        }

        [ConfigurationProperty("HouseNumber", DefaultValue = "")]
        public string HouseNumber
        {
            get { return (string)this["HouseNumber"]; }
            set { this["HouseNumber"] = value; }
        }

        [ConfigurationProperty("Telefonnumber", DefaultValue = "")]
        public string Telefonnumber
        {
            get { return (string)this["Telefonnumber"]; }
            set { this["Telefonnumber"] = value; }
        }

        [ConfigurationProperty("EmailAddress", DefaultValue = "")]
        public string EmailAddress
        {
            get { return (string)this["EmailAddress"]; }
            set { this["EmailAddress"] = value; }
        }

        [ConfigurationProperty("BankName", DefaultValue = "")]
        public string BankName
        {
            get { return (string)this["BankName"]; }
            set { this["BankName"] = value; }
        }

        [ConfigurationProperty("IBAN", DefaultValue = "")]
        public string IBAN
        {
            get { return (string)this["IBAN"]; }
            set { this["IBAN"] = value; }
        }

        [ConfigurationProperty("BIC", DefaultValue = "")]
        public string BIC
        {
            get { return (string)this["BIC"]; }
            set { this["BIC"] = value; }
        }

        [ConfigurationProperty("USTIDNR", DefaultValue = "")]
        public string USTIDNR
        {
            get { return (string)this["USTIDNR"]; }
            set { this["USTIDNR"] = value; }
        }

        [ConfigurationProperty("STNR", DefaultValue = "")]
        public string STNR
        {
            get { return (string)this["STNR"]; }
            set { this["STNR"] = value; }
        }
    }
}