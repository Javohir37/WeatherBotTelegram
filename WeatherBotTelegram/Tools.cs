using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherBotTelegram
{
    public class BotParams{
        public static bool isLocation = false;
    }
    public class Tools
    {
        public void errorCase()
        {
            Weather.readyToUse = "Sorry, I somehow got error while doing that :( \nTry another nearest city";
        }
        public string[] dateConverter(string[] date)
        {
            string month, day;
            int nMonth;
            for (int i = 0; i < date.Length; i++)
            {
                month = date[i][5] + "" + date[i][6];
                day = date[i][8] + "" + date[i][9];
                nMonth = Convert.ToInt32(month);
                if (day[0] == '0')
                {
                    day = day.Remove(0, 1);
                }
                switch (nMonth)
                {
                    case 1:
                        date[i] = "January " + day;
                        break;
                    case 2:
                        date[i] = "February " + day;
                        break;
                    case 3:
                        date[i] = "March " + day;
                        break;
                    case 4:
                        date[i] = "April " + day;
                        break;
                    case 5:
                        date[i] = "May " + day;
                        break;
                    case 6:
                        date[i] = "June " + day;
                        break;
                    case 7:
                        date[i] = "July " + day;
                        break;
                    case 8:
                        date[i] = "August " + day;
                        break;
                    case 9:
                        date[i] = "September " + day;
                        break;
                    case 10:
                        date[i] = "October " + day;
                        break;
                    case 11:
                        date[i] = "Novermber " + day;
                        break;
                    case 12:
                        date[i] = "December " + day;
                        break;
                }
            }
            return date;
        }
        public string stringRemover1(string json)
        {
            if (json[json.Length - 2] == 'e')//there will be 'e' in the end of json if error is true
            {
                return "0";//error catching
            }
            string temp = "";
            int squareBracketsNumber = 0;
            for (int i = 0; i < json.Length - 1; i++)
            {
                if (json[i] == '{')
                {
                    squareBracketsNumber++;
                }
                if (squareBracketsNumber >= 3)
                {
                    temp += json[i];
                }
            }
            return temp;
        }
        public string stringRemover(string json)
        {
            // eroor catcher
            if (json[json.Length - 2] == '0')
            {
                return "{'lat':0,'lng':0}";
            }
            string temp = "";
            int openBracket = 0, closeBracket = 0;
            for (int i = 0; i < json.Length - 1; i++)
            {
                if (json[i] == '}')
                {
                    closeBracket++;
                }
                if (closeBracket == 3)
                {
                    break;
                }
                if (json[i] == '{')
                {
                    openBracket++;
                }
                if (openBracket >= 6)
                {
                    temp += json[i];
                }
            }
            if (temp[temp.Length - 1] != '}')
            {
                temp += "}";
            }
            return temp;
        }
        public void coordinates(string city)
        {
            string url = Convert.ToString($"https://api.opencagedata.com/geocode/v1/json?q={city}&key=cabd3f4b59a149c3b7f643f5a5969fe4&limit=1&no_annotations=1");
            WebClient client = new WebClient();
            string json = client.DownloadString(url);
            json = stringRemover(json);
            Coordinates coordinates1 = JsonConvert.DeserializeObject<Coordinates>(json);
            Coordinates.longitude = coordinates1.lng;
            Coordinates.latitude = coordinates1.lat;
            Coordinates.city = "something";// program uses this value to check whether the user already sent location
        }
        public string[] tempConverter(double[] temp)
        {
            string[] tempShort = new string[7];
            for (int i = 0; i < temp.Length; i++)
            {
                tempShort[i] = Convert.ToString(Math.Round(temp[i]));
                if (Math.Round(temp[i]) > 0)
                {
                    tempShort[i] = "+" + tempShort[i];
                }
                if (tempShort[i].Length == 2)
                {
                    tempShort[i] += "   ";
                }
                else
                {
                    tempShort[i] += " ";
                }
            }

            return tempShort;
        }
        public string[] weatherConverter(short[] weatherCode)
        {
            string[] weatherCondition = new string[7];
            for (int i = 0; i < weatherCode.Length; i++)
            {
                if (weatherCode[i] == 0) weatherCondition[i] = "☀";//sun
                else if (weatherCode[i] == 3) weatherCondition[i] = "☁";//cloud
                else if (weatherCode[i] == 71 && weatherCode[i] == 73 && weatherCode[i] == 75 && weatherCode[i] == 77 && weatherCode[i] == 85 && weatherCode[i] == 86) weatherCondition[i] = "🌨";//snow
                else if (weatherCode[i] == 51 && weatherCode[i] == 53 && weatherCode[i] == 56 && weatherCode[i] == 81 && weatherCode[i] == 82) weatherCondition[i] = "☔";//showers
                else if (weatherCode[i] == 1 && weatherCode[i] == 2) weatherCondition[i] = "⛅";
                else if (weatherCode[i] == 45 && weatherCode[i] == 48) weatherCondition[i] = "🌫";//fog
                else weatherCondition[i] = "🌧";//cloud rain
            }
            return weatherCondition;
        }
        public void readyToUseMethod()
        {
            Weather.readyToUse = "";
            for (int i = 0; i < 7; i++)
            {
                Weather.readyToUse += $"{Weather.date[i],-12}{Weather.weatherCondition[i]} day: {Weather.maxTemp[i]} night: {Weather.minTemp[i]}\n";
            }
        }
    }
}
