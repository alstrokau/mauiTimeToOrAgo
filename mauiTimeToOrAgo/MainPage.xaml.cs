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
                lbOut.Text = "--:--:--";
                return;
            }

            var now = DateTime.Now;
            var dt = targetDateTime.Value;

            if (dt > now)
            {
                var span = dt - now;
                lbOut.Text = FormatTimeSpan(span) + " left";
            }
            else
            {
                var span = now - dt;
                lbOut.Text = FormatTimeSpan(span) + " ago";
            }
        }

        private static string FormatTimeSpan(TimeSpan span)
        {
            if (span.TotalDays >= 1)
                return $"{(int)span.TotalDays}d {span.Hours}h {span.Minutes}m {span.Seconds}s";
            if (span.TotalHours >= 1)
                return $"{(int)span.TotalHours}h {span.Minutes}m {span.Seconds}s";
            if (span.TotalMinutes >= 1)
                return $"{(int)span.TotalMinutes}m {span.Seconds}s";
            return $"{span.Seconds}s";
        }

        private void OnSaveDateClicked(object? sender, EventArgs e)
        {
            var date = dpTargetDate.Date;
            var time = tpTargetTime.Time;
            var dt = date + time;
            targetDateTime = dt;
            Preferences.Set(PrefKey, dt.ToString());
            lblStoredDate.Text = $"Saved: {dt:G}";
        }

        private void OnClearDateClicked(object? sender, EventArgs e)
        {
            targetDateTime = null;
            Preferences.Remove(PrefKey);
            lblStoredDate.Text = "No date saved";
            lbOut.Text = "--:--:--";
        }
    }
}