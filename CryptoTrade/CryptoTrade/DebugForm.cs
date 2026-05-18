namespace CryptoTrade
{
    public class DebugForm : Form
    {
        private readonly ListBox _listBox;

        public DebugForm()
        {
            Text = "Debug — Binance WebSocket";
            Size = new Size(700, 400);
            StartPosition = FormStartPosition.Manual;
            Location = new Point(50, 50);
            BackColor = Color.FromArgb(18, 18, 26);

            _listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(22, 22, 35),
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 9F),
                HorizontalScrollbar = true,
            };

            var btnClear = new Button
            {
                Text = "Effacer",
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            btnClear.Click += (_, _) => _listBox.Items.Clear();

            Controls.Add(_listBox);
            Controls.Add(btnClear);
        }

        public void AddLog(string msg)
        {
            _listBox.Items.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {msg}");
            if (_listBox.Items.Count > 200)
                _listBox.Items.RemoveAt(_listBox.Items.Count - 1);
        }
    }
}