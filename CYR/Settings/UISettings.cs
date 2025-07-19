using System.Configuration;

namespace CYR.Settings;

public class UISettings : ConfigurationSection
{
		[ConfigurationProperty("ButtonColor", DefaultValue = "Gray")]
		public string ButtonColor
		{
			get { return (string)this["ButtonColor"]; }
			set { this["ButtonColor"] = value; }
		}

	}
