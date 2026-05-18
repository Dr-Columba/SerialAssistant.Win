using SerialAssistant.Core.Enums;

namespace SerialAssistant.Core.Models
{
    /*
     * Represents application settings
     */
    public class AppSettings
    {
        public string LastPortName
        {
            get;
            set;
        } = string.Empty;

        public int BaudRate
        {
            get;
            set;
        } = 9600;

        public int DataBits
        {
            get;
            set;
        } = 8;

        public string Parity
        {
            get;
            set;
        } = "None";

        public string StopBits
        {
            get;
            set;
        } = "One";

        public SendMode SendMode
        {
            get;
            set;
        } = SendMode.Text;

        public DisplayMode DisplayMode
        {
            get;
            set;
        } = DisplayMode.Text;

        public SendLineEnding SendLineEnding
        {
            get;
            set;
        } = SendLineEnding.None;

        public bool ShowTimestamp
        {
            get;
            set;
        } = true;

        public bool ShowDirection
        {
            get;
            set;
        } = true;

        public int MaxDisplayBytes
        {
            get;
            set;
        } = 262144;

        public static AppSettings CreateDefault()
        {
            return new AppSettings
            {
                LastPortName = string.Empty,
                BaudRate = 9600,
                DataBits = 8,
                Parity = "None",
                StopBits = "One",
                SendMode = SendMode.Text,
                DisplayMode = DisplayMode.Text,
                SendLineEnding = SendLineEnding.None,
                ShowTimestamp = true,
                ShowDirection = true,
                MaxDisplayBytes = 262144
            };
        }
    }
}
