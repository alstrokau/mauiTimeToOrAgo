namespace mauiTimeToOrAgo
{
    public partial class MainPage : ContentPage
    {
        bool timerActive = false;
        private readonly bool timerStarted = false;

        const string PrefKey = "TargetDateTime";

        DateTime? targetDateTime;

        public MainPage()
        {
            InitializeComponent();

            if (Preferences.ContainsKey(PrefKey))
            {
                var s = Preferences.Get(PrefKey, string.Empty);
                if (DateTime.TryParse(s, out var dt))
                {
                    targetDateTime = dt;
                    lblStoredDate.Text = $"Saved: {dt:G}";
                    dpTargetDate.Date = dt.Date;
                    tpTargetTime.Time = dt.TimeOfDay;
                }
            }

            timerActive = true;

            if (!timerStarted)
            {
                timerStarted = true;
                Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    UpdateTimeDisplay();
                    return timerActive;
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            timerActive = true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            timerActive = false;
        }

        private void UpdateTimeDisplay()
        {
            if (targetDateTime == null)
            {
                lbOutMain.Text = "--:--:--";
                lbOutSecond.Text = "--:--:--";
                return;
            }

            var now = DateTime.Now;
            var dt = targetDateTime.Value;

            if (dt > now)
            {
                var span = dt - now;
                lbOutMain.Text = FormatTimeSpan(span).Main;
                lbOutSecond.Text = FormatTimeSpan(span).Second + "\nleft";
            }
            else
            {
                var span = now - dt;
                lbOutMain.Text = FormatTimeSpan(span).Main;
                lbOutSecond.Text = FormatTimeSpan(span).Second + "\nago";
            }
        }

        private static (string Main, string Second) FormatTimeSpan(TimeSpan span)
        {
            string result;

            if (span.TotalDays >= 1)
                result = $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes:D2}m {span.Seconds:D2}s";
            else if (span.TotalHours >= 1)
                result = $"{(int)span.TotalHours}h {span.Minutes:D2}m {span.Seconds:D2}s";
            else if (span.TotalMinutes >= 1)
                result = $"{(int)span.TotalMinutes}m {span.Seconds:D2}s";
            else
                result = $"{span.Seconds}s ";

            return (result[..result.IndexOf(' ')], result[(result.IndexOf(' ') + 1)..]);
        }

        private void OnSaveDateClicked(object? sender, EventArgs e)
        {
            var date = dpTargetDate.Date;
            var time = tpTargetTime.Time;
            var dt = date + time;
            targetDateTime = dt;
            Preferences.Set(PrefKey, dt.ToString());
            lblStoredDate.Text = $"Target DateTime: {dt:dd/MM/yyyy HH:mm:ss}";
        }

        private void OnClearDateClicked(object? sender, EventArgs e)
        {
            targetDateTime = null;
            Preferences.Remove(PrefKey);
            lblStoredDate.Text = "No date saved";
            lbOutMain.Text = "--:--:--";
            lbOutSecond.Text = "--:--:--";
        }
    }
}