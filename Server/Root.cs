using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class Root
	{
		public double latitude { get; set; }
		public double longitude { get; set; }
		public double generationTime_ms { get; set; }
		public int utc_offset_seconds { get; set; }
		public string timezone { get; set; }
		public string timezone_abbreviation { get; set; }
		public double elevation { get; set; }
		public CurrentWeather current_weather { get; set; }
		public HourlyUnits hourly_units { get; set; }
		public Hourly hourly { get; set; }
	}
	public class CurrentWeather
	{
		public double temperature { get; set; }
		public double windspeed { get; set; }
		public double winddirection { get; set; }
		public int weathercode { get; set; }
		public string time { get; set; }
	}
	public class HourlyUnits
	{
		public string time { get; set; }
		public string temperature_2m { get; set; }
	}
	public class Hourly
	{
		public List<string> time { get; set; }
		public List<double> temperature_2m { get; set; }
	}
}
