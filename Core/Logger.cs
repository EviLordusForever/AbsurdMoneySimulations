using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
	public static class Logger
	{
        public static int logSize = 10000;
        public static string logText;

        public static void Log(string str)
        {
            FormsManager.mainForm.Invoke(new Action(() =>
            {
                FormsManager.OpenLogForm();
                string msg = CreateMessageToShout(str);
                ShoutToEverybodyConsole(msg);
                CutEverybodyConsole();
                VisualiseLog();
            }));


            void ShoutToEverybodyConsole(string msg)
            {
                logText = $"{msg}\r\n{logText}";
            }

            void CutEverybodyConsole()
            {
                if (logText.Length > logSize + 1000)
                    logText = logText.Remove(logSize);
            }

            string CreateMessageToShout(string text)
            {
                string who = "";
                string during = "";
                string place = "";
                string whoe = "   ";
                string duringe = "   ";
                string placee = "   ";

                string time = GetTimeToShow(System.DateTime.Now);
                text = ModyfyText(text);

                if (whoe.Length > who.Length)
                    who += whoe.Remove(whoe.Length - 1 - who.Length);
                if (duringe.Length > during.Length)
                    during += duringe.Remove(duringe.Length - 1 - during.Length);
                if (placee.Length > place.Length)
                    place += placee.Remove(placee.Length - 1 - place.Length);

                return $"[{time}] {who} {during} {place} {text}";


                string ModyfyText(string str)
                {
                    int size = 180;
                    string res = "";
                    str += "\r\n";

                    while (true)
                    {
                        int indexOfR = Math.Min(str.IndexOf('\r'), str.IndexOf('\n'));
                        indexOfR = Math.Min(indexOfR, str.IndexOf(Environment.NewLine));


                        if (indexOfR < size - 1 && indexOfR != -1 && indexOfR + 2 < str.Length)
                        {
                            res += str.Remove(indexOfR + 1).Replace('\r', ' ').Replace('\n', ' ') + "\r" + whoe + duringe + placee + "        ";
                            str = str.Substring(indexOfR + 1);
                        }
                        else if (str.Length <= size)
                        {
                            res += str.Replace('\r', ' ').Replace('\n', ' ');
                            break;
                        }
                        else if (str.Length > size)
                        {
                            string strRemoveSize = str.Remove(size);

                            int lastSpaceIndex = size;
                            if (strRemoveSize.Contains(" "))
                                lastSpaceIndex = strRemoveSize.LastIndexOf(' ');

                            res += str.Remove(lastSpaceIndex) + "\r" + whoe + duringe + placee + "        ";
                            str = str.Substring(lastSpaceIndex + 1);
                        }
                        else
                        {
                            res += str.Replace('\r', ' ').Replace('\n', ' ');
                            break;
                        }
                    }

                    return res;
                }
            }


        }

        public static void VisualiseLog()
        {
            FormsManager.logForm.rtb.Text = logText;
        }

        public static string GetTimeToShow(DateTime dateTime)
        {
            string h = dateTime.Hour.ToString();
            string m = dateTime.Minute.ToString();
            if (h.Length == 1)
                h = "0" + h;
            if (m.Length == 1)
                m = "0" + m;
            return h + ":" + m;
        }
    }
}
