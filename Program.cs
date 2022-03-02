
using CsvHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace _2HandTelegramBot
{
    class Program
    {
        private static string Token { get; set; } = "5044910720:AAE4FA0NLVtro8N09Ls48GCJfN5GD6QF_p0";
        private static string pathCsvFile = "C:\\Users\\bogda\\Desktop\\adresses.csv";
        private static TelegramBotClient client;

        private static float lat = 50.462903f;
        private static float lng = 30.382389f;

        static Dictionary<double, SecondHand> keyValuePairs = new Dictionary<double, SecondHand>();

        static List<SecondHand> CSV_Struct = new List<SecondHand>();


        static void Main(string[] args)
        {
            client = new TelegramBotClient(Token);
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            ReadInFile();

            Console.ReadLine();
            client.StopReceiving();
        }

        private static void ReadInFile()
        {
            CSV_Struct = SecondHand.ReadFile(pathCsvFile);

            CSV_Struct.Remove(CSV_Struct[0]);
           
            Distance();
        }
       
        private static void Distance()
        {
            foreach (var item in CSV_Struct)
            {
                try
                {
                    item.latitude = item.latitude.Trim('"');
                    item.longitude = item.longitude.Trim('"');

                    item.latitude = item.latitude.Replace(".", ",");
                    item.longitude = item.longitude.Replace(".", ",");

                    item.name = item.name.Trim('"');
                    item.name = item.name.Replace(".", ",");

                    item.phone = item.phone.Trim('"');
                    item.phone = item.phone.Replace(".", ",");

                    var distanse = Calculate(lng, lat, float.Parse(item.longitude), float.Parse(item.latitude));
                    keyValuePairs.Add(distanse, item);
                }
                catch (Exception)
                {
                    Console.WriteLine("EXCEPTION");
                }

            }
        }
      
        public static double Calculate(double sLatitude, double sLongitude, double eLatitude,
                               double eLongitude)
        {
            var radiansOverDegrees = (Math.PI / 180.0);

            var sLatitudeRadians = sLatitude * radiansOverDegrees;
            var sLongitudeRadians = sLongitude * radiansOverDegrees;
            var eLatitudeRadians = eLatitude * radiansOverDegrees;
            var eLongitudeRadians = eLongitude * radiansOverDegrees;

            var dLongitude = eLongitudeRadians - sLongitudeRadians;
            var dLatitude = eLatitudeRadians - sLatitudeRadians;

            var result1 = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                          Math.Cos(sLatitudeRadians) * Math.Cos(eLatitudeRadians) *
                          Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            var result2 = 3956.0 * 2.0 *
                          Math.Atan2(Math.Sqrt(result1), Math.Sqrt(1.0 - result1));

            return result2;
        }
    
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            
            if (msg.Text != null)
            {
                Console.WriteLine($"Пришло сообщение с текстом: {msg.Text} | Пользователь: {msg.Chat.Username}");
                switch (msg.Text)
                {
                    case "Магазины по-близости":
                        var minDistance = keyValuePairs.Min(s => s.Key);
                     

                        await client.SendVenueAsync(
                            chatId: msg.Chat.Id,
                            latitude: float.Parse(keyValuePairs[minDistance].latitude),
                            longitude: float.Parse(keyValuePairs[minDistance].longitude),
                            title: keyValuePairs[minDistance].name,
                            address: "",
                            replyMarkup: GetButtons());

                        keyValuePairs.Remove(minDistance);

                        break;

                    default:
                        await client.SendTextMessageAsync(msg.Chat.Id, "Выберите команду", replyMarkup: GetButtons());
                        break;
                }
            }
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton { Text = "Магазины по-близости" } }
                }
            };
        }
    }
}