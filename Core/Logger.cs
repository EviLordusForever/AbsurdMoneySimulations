using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbsurdMoneySimulations
{
    public static class Logger
    {
        public static int logSize = 15000;
        public static string logText;
        public static StreamWriter writer;
        public static bool updated;

        public static Thread flusherThread;
        public static Thread visualizerThread;


        public static void Log(string text)
        {
            FormsManager.OpenLogForm();

            string msg = CreateMessageToShout(text);
            string date = GetDateToShow(System.DateTime.Now);
            string time = GetTimeToShow(System.DateTime.Now);
			LogToFile($"[{date}][{time}] {msg}");
			LogToWindow($"[{date}][{time}] {msg}");
            CutVisibleLog();

			void LogToFile(string text)
			{
                FormsManager.mainForm.Invoke(new Action(() =>
                {
                    writer.Write($"{text}\r\n");
                }));
			}

			void LogToWindow(string text)
            {
                logText = $"{text}\r\n{logText}";
                updated = true;
            }

            void CutVisibleLog()
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

                string timee = "          ";
                string datee = "            ";


                text = ModyfyText(text);

                if (whoe.Length > who.Length)
                    who += whoe.Remove(whoe.Length - 1 - who.Length);
                if (duringe.Length > during.Length)
                    during += duringe.Remove(duringe.Length - 1 - during.Length);
                if (placee.Length > place.Length)
                    place += placee.Remove(placee.Length - 1 - place.Length);

                return $"{who} {during} {place} {text}";


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
                            res += str.Remove(indexOfR + 1).Replace('\r', ' ').Replace('\n', ' ') + "\r" + datee + timee + whoe + duringe + placee + " ";
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

                            res += str.Remove(lastSpaceIndex) + "\r" + datee + timee + whoe + duringe + placee + " ";
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

        public static void Log(float text)
        {
            Log(text.ToString());
        }

        public static void Log(int text)
        {
            Log(text.ToString());
        }

        public static void Visualiser()
        {
            visualizerThread = new Thread(VisualiserThread);
            visualizerThread.Name = "LogVisuliser";
            visualizerThread.Start();

            void VisualiserThread()
            {
                try
                {
                    while (true)
                    {
                        if (updated)
                        {
                            if (FormsManager.logForm.WindowState != FormWindowState.Minimized)
                            {
                                FormsManager.mainForm.Invoke(new Action(() =>
                                {
                                    FormsManager.logForm.rtb.Text = logText;
                                }));
                                updated = false;
                            }
                        }
                        Thread.Sleep(250);
                    }
                }
                catch (ThreadInterruptedException ex)
                {
                }
            }
        }

        static void Flusher()
        {
            flusherThread = new Thread(FlusherThread);
            flusherThread.Name = "LogFlusher";
            flusherThread.Start();

            void FlusherThread()
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(20000);
                        writer.Flush();
                    }
                }
                catch (ThreadInterruptedException ex)
                {
                    writer.Flush();
                }
            }
        }

        public static string GetTimeToShow(DateTime dateTime)
        {
            string h = dateTime.Hour.ToString();
            string m = dateTime.Minute.ToString();
            string s = dateTime.Second.ToString();
            if (h.Length == 1)
                h = "0" + h;
            if (m.Length == 1)
                m = "0" + m;
            if (s.Length == 1)
                s = "0" + s;
            return h + ":" + m + ":" + s;
        }

        public static string GetDateToShow(DateTime dateTime)
        {
            string d = dateTime.Day.ToString();
            string m = dateTime.Month.ToString();
            string y = dateTime.Year.ToString();

            if (d.Length == 1)
                d = "0" + d;
            if (m.Length == 1)
                m = "0" + m;
            return d + "." + m + "." + y;
        }

        public static void Quit()
        {
            visualizerThread.Interrupt();
            flusherThread.Interrupt();
        }

        static Logger()
        {
            logText = "";
            writer = new StreamWriter(Disk.programFiles + "Logs\\log.log", true);
            Flusher();
            Visualiser();
        }
    }
}
