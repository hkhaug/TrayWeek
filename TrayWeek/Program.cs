using System;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace TrayWeek
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Mutex mutex = new Mutex(false, "Global\\" + GetApplicationGuid()))
            {
                if (mutex.WaitOne(0, false))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new AppContext());
                }
            }
        }

        static string GetApplicationGuid()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            GuidAttribute attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return attribute.Value;
        }

        class AppContext : ApplicationContext
        {
            private NotifyIcon notifyIcon;

            public AppContext()
            {
                notifyIcon = new NotifyIcon()
                {
                    Text = "Week number. Click to close.",
                    Icon = WeekIcon(WeekToday()),
                    Visible = true
                };
                notifyIcon.Click += Exit;
            }

            void Exit(object sender, EventArgs e)
            {
                notifyIcon.Visible = false;
                Application.Exit();
            }

            private static int WeekToday()
            {
                DateTime date = DateTime.Today;
                int day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            private Icon WeekIcon(int week)
            {
                string weekStr = week.ToString();
                Font font = new Font("Calibri", 30, FontStyle.Bold, GraphicsUnit.Pixel);
                Brush brush = new SolidBrush(Color.Black);
                Bitmap bitmap = new Bitmap(32, 32);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(Color.White);
                graphics.DrawString(weekStr, font, brush, (week < 10 ? 4 : -4), -3);
                IntPtr handle = (bitmap.GetHicon());
                return Icon.FromHandle(handle);
            }
        }
    }
}
