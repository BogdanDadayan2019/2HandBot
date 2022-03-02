using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2HandTelegramBot
{
    public class SecondHand
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }

        public SecondHand(string name, string phone, string latitude, string longitude)
        {
            this.name = name;
            this.phone = phone;
            this.latitude = latitude;
            this.longitude = longitude;
           
        }

        public SecondHand()
        {
        }

        public void piece(string line)
        {
            string[] parts = line.Split(',');  //Разделитель в CSV файле.
            name = parts[0];
            phone = parts[1];
            latitude = parts[2];
            longitude = parts[3];
        }

        public static List<SecondHand> ReadFile(string filename)
        {
            List<SecondHand> res = new List<SecondHand>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    SecondHand p = new SecondHand();
                    p.piece(line);
                    res.Add(p);
                }
            }

            return res;
        }

    }
}
