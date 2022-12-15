using System.Globalization;

namespace CalendarWeek
{
    public class CalendarWeekApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;
        private readonly Calendar _calendar;
        private readonly CalendarWeekRule _calendarWeekRule;
        private readonly DayOfWeek _firstDayOfWeek;
        private CalendarForm _calendarForm;

        private string CurrentCalendarWeek => _calendar.GetWeekOfYear(DateTime.Now, _calendarWeekRule, _firstDayOfWeek).ToString();

        public CalendarWeekApplicationContext()
        {
            var cultureInfo = new CultureInfo("de-DE");
            _calendar = cultureInfo.Calendar;
            _calendarWeekRule = cultureInfo.DateTimeFormat.CalendarWeekRule;
            _firstDayOfWeek = cultureInfo.DateTimeFormat.FirstDayOfWeek;

            // Create a bitmap and draw text on it
            var bitmap = new Bitmap(16, 16);
            var graphics = Graphics.FromImage(bitmap);
            var font = new Font("Segoe UI", 8, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.White);
            graphics.DrawString(CurrentCalendarWeek, font, brush, 0, 0);
            // Convert the bitmap with text to an Icon
            var icon = Icon.FromHandle(bitmap.GetHicon());

            var ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, Exit));

            // Initialize Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = icon,
                Text = "CalendarWeek",
                ContextMenuStrip = ContextMenuStrip,
                Visible = true
            };

            _trayIcon.MouseMove += NotifyIcon_MouseMove;
            _trayIcon.MouseClick += NotifyIcon_MouseClick;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (_calendarForm == null || _calendarForm.IsDisposed)
                _calendarForm = new CalendarForm();

            if (_calendarForm.Visible)
                _calendarForm.Hide();
            else
                _calendarForm.Show();
        }

        private void NotifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            _trayIcon.Text = $"Week: {CurrentCalendarWeek}";
        }

        private void Exit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
