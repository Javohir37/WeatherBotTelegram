using Newtonsoft.Json;
using System.Data;
using System.Net;
using WeatherBotTelegram;

namespace WeatherBotTelegram
{
    public class Coordinates
    {
        public static string city = "null";
        public double lat, lng;
        public static double latitude, longitude;
    }
    public class Weather
    {
        public string[] time = new string[7];
        public double[] temperature_2m_max = new double[7];
        public double[] temperature_2m_min = new double[7];
        public short[] weathercode = new short[7];
        //ready to output nes
        public static string[] date = new string[7];
        public static string[] maxTemp = new string[7];
        public static string[] minTemp = new string[7];
        public static string[] weatherCondition = new string[7];
        public static string readyToUse;
        public static bool error;
    }
    public class CLassMain : Tools
    {
        public void weather()
        {
            CLassMain objProg = new CLassMain();
            if (Coordinates.longitude == 0 && Coordinates.latitude == 0)//coordinates will be 00 if there are some kind of an error
            {
                errorCase();
                return;
            }
            WebClient client = new WebClient();
            Weather objWeather = new Weather();
            string json;
            string[] weatherVariables = { "weathercode", "temperature_2m_min", "temperature_2m_max" };
            for (int i = 0; i < weatherVariables.Length; i++)
            {
                string url = Convert.ToString($"https://api.open-meteo.com/v1/forecast?latitude={Coordinates.latitude}&longitude={Coordinates.longitude}&daily={weatherVariables[i]}&timezone=auto");
                try
                {
                    json = client.DownloadString(url);
                }
                catch (System.Net.WebException)
                {
                    errorCase();
                    return;
                }
                json = objProg.stringRemover1(json);
                if (json == "0")
                {

                }
                var parsedInfo = JsonConvert.DeserializeObject<Weather>(json);
                if (i == 0)
                {
                    Weather.date = dateConverter(parsedInfo.time);
                    Weather.weatherCondition = weatherConverter(parsedInfo.weathercode);
                }
                else if (i == 1)
                {
                    Weather.minTemp = tempConverter(parsedInfo.temperature_2m_min);
                }
                else
                {
                    Weather.maxTemp = tempConverter(parsedInfo.temperature_2m_max);
                }
            }
            readyToUseMethod();
        }
    }
}