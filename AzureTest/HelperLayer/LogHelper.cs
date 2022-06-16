using Domain.Enum;
using AzureTest.ExtensionLayer;
namespace Helper
{
    public class LogHelper
    {
        public static void LogMessage(string message, MessageTypeEnum messageType)
        {
            Console.WriteLine($"{DateTime.Now.ToTurkeyStandardTime()} | {messageType} | {message}");
        }
    }
}