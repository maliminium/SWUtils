using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for UI_Logger.xaml
    /// </summary>
    public partial class UI_Logger : UserControl
    {
        Paragraph logParagraph = new Paragraph();

        public UI_Logger()
        {
            InitializeComponent();
            rtbLog.Document.Blocks.Clear();
            rtbLog.Document.Blocks.Add(logParagraph);
            Logger.Logged += Logger_Logged;
        }

        delegate void AppendAction(string msg);
        private void Logger_Logged(string newMessage, MessageTypeEnum messageType)
        {
            var msgForeground = Brushes.Green;
            //var msgBackground = Brushes.Transparent;
            var msgWeight = FontWeights.Normal;
            //var msgStyle = FontStyles.Normal;

            switch (messageType)
            {
                case MessageTypeEnum.Normal:
                    msgForeground = Brushes.Black;
                    break;
                case MessageTypeEnum.Warning:
                    msgForeground = Brushes.DarkOrange;
                    break;
                case MessageTypeEnum.Error:
                    msgForeground = Brushes.Crimson;
                    break;
                case MessageTypeEnum.Greeting:
                    msgForeground = Brushes.ForestGreen;
                    break;
                case MessageTypeEnum.None:
                    msgForeground = Brushes.DimGray;
                    break;
                case MessageTypeEnum.Exception:
                    msgForeground = Brushes.MediumVioletRed;
                    msgWeight = FontWeights.Bold;
                    break;
                case MessageTypeEnum.Information:
                    msgForeground = Brushes.RoyalBlue;
                    break;
                default:
                    msgForeground = Brushes.Black;
                    break;
            }


            if (Dispatcher.CheckAccess())
            {
                logParagraph.Inlines.Add(new Run(newMessage + "\r") { Foreground = msgForeground, FontWeight = msgWeight });
            }
            else
            {
                logParagraph.Inlines.Add(new Run(newMessage + "\r") { Foreground = msgForeground, FontWeight = msgWeight, FontStyle = FontStyles.Italic });
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            logParagraph.Inlines.Clear();
        }
    }

    public static class Logger
    {
        public delegate void LogAction(string newMessage, MessageTypeEnum messageType);
        public static event LogAction Logged;
        private static void RaiseLogAction(string newMessage, MessageTypeEnum messageType) => Logged?.Invoke(newMessage, messageType);

        public static void Log(string message)
        {
            RaiseLogAction(message, MessageTypeEnum.None);
        }

        public static void Log(string message, MessageTypeEnum messageType)
        {
            RaiseLogAction(message, messageType);
        }

        public static void LogException(Exception ex)
        {
            Log("Exception in " + new StackTrace().GetFrame(1).GetMethod().Name + ": " + ex.Message, MessageTypeEnum.Exception);
        }
    }
}
