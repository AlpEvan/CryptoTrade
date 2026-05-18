namespace CryptoTrade
{
    public partial class Form1 : Form
    {
        // ─── DONNÉES ──────────────────────────────────────────────────
        private decimal _walletUSDT = 10_000m;
        private readonly Dictionary<string, decimal> _holdings = new();
        private readonly Dictionary<string, TickerData> _tickers = new();
        private string _selectedSymbol = "BTCUSDT";
        private readonly List<TradeRecord> _history = new();
        private BinanceService? _binance;

        // Cryptos disponibles dans le jeu
        private readonly List<(string Symbol, string Name, string Emoji)> _cryptos = new()
        {
            ("BTCUSDT",  "Bitcoin",   "₿"),
            ("ETHUSDT",  "Ethereum",  "Ξ"),
            ("BNBUSDT",  "BNB",       "🔶"),
            ("SOLUSDT",  "Solana",    "◎"),
            ("XRPUSDT",  "XRP",       "✕"),
            ("ADAUSDT",  "Cardano",   "₳"),
            ("DOGEUSDT", "Dogecoin",  "🐕"),
            ("AVAXUSDT", "Avalanche", "🔺"),
            ("DOTUSDT",  "Polkadot",  "●"),
            ("MATICUSDT","Polygon",   "🟣"),
        };

        public Form1() => InitializeComponent();

        // ─── CHARGEMENT ───────────────────────────────────────────────
        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var c in _cryptos)
                _holdings[c.Symbol] = 0m;

            PopulateCryptoList();
            UpdateWalletDisplay();
            StartBinanceService();

            // ← Ouvre la fenêtre de debug au démarrage
            ShowDebugWindow();
        }

        // Fenêtre de debug flottante
        private DebugForm? _debugForm;
        private void ShowDebugWindow()
        {
            _debugForm = new DebugForm();
            _debugForm.Show(this);
        }

        public void AddDebugLog(string msg)
        {
            if (_debugForm == null || _debugForm.IsDisposed) return;
            if (_debugForm.IsHandleCreated)
                _debugForm.Invoke(() => _debugForm.AddLog(msg));
        }

        // ─── BINANCE SERVICE ──────────────────────────────────────────
        private void StartBinanceService()
        {
            var symbols = _cryptos.Select(c => c.Symbol).ToList();
            _binance = new BinanceService(symbols);

            // ← Branche les logs de debug
            _binance.OnDebugLog += msg =>
            {
                AddDebugLog(msg);
                System.Diagnostics.Debug.WriteLine($"[Binance] {msg}");
            };

            _binance.OnTickerUpdate += ticker =>
            {
                _tickers[ticker.Symbol] = ticker;
                if (!IsHandleCreated) return;
                Invoke(() =>
                {
                    UpdateCryptoRow(ticker.Symbol);
                    if (ticker.Symbol == _selectedSymbol)
                        UpdateDetailPanel(ticker);
                    UpdateWalletDisplay();
                });
            };

            _binance.OnStatusChange += (isLive, msg) =>
            {
                if (!IsHandleCreated) return;
                Invoke(() =>
                {
                    lblStatus.Text = msg;
                    lblLiveIndicator.Text = isLive ? "● LIVE" : "● OFF";
                    lblLiveIndicator.ForeColor = isLive
                        ? Color.FromArgb(14, 203, 129)
                        : Color.FromArgb(246, 70, 93);
                });
            };

            _binance.Start();
        }

        // ─── LISTE CRYPTOS ────────────────────────────────────────────
        private void PopulateCryptoList()
        {
            listViewCryptos.Items.Clear();
            foreach (var c in _cryptos)
            {
                var item = new ListViewItem($"{c.Emoji} {c.Name}");
                item.SubItems.Add("--");      // Prix
                item.SubItems.Add("--");      // 24h%
                item.SubItems.Add("");        // Holdings indicator
                item.Tag = c.Symbol;
                item.ForeColor = Color.White;
                listViewCryptos.Items.Add(item);
            }
        }

        private void UpdateCryptoRow(string symbol)
        {
            foreach (ListViewItem item in listViewCryptos.Items)
            {
                if (item.Tag?.ToString() != symbol) continue;

                if (!_tickers.TryGetValue(symbol, out var t)) break;

                item.SubItems[1].Text = FormatPrice(t.Price);
                item.SubItems[2].Text = $"{t.Change24h:+0.00;-0.00}%";
                item.SubItems[2].ForeColor = t.Change24h >= 0
                    ? Color.FromArgb(14, 203, 129)
                    : Color.FromArgb(246, 70, 93);

                // Point jaune si on détient cette crypto
                decimal held = _holdings.GetValueOrDefault(symbol);
                item.SubItems[3].Text = held > 0 ? "●" : "";
                item.SubItems[3].ForeColor = Color.FromArgb(247, 185, 36);
                break;
            }
        }

        private void ListViewCryptos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCryptos.SelectedItems.Count == 0) return;
            _selectedSymbol = listViewCryptos.SelectedItems[0].Tag?.ToString() ?? "BTCUSDT";

            if (_tickers.TryGetValue(_selectedSymbol, out var t))
                UpdateDetailPanel(t);

            RefreshTradePanel();
        }

        // ─── PANEL DÉTAIL CRYPTO ──────────────────────────────────────
        private void UpdateDetailPanel(TickerData t)
        {
            var meta = _cryptos.First(c => c.Symbol == t.Symbol);
            string ticker = t.Symbol.Replace("USDT", "");

            lblSelectedCrypto.Text = $"{meta.Emoji} {meta.Name} ({ticker}/USDT)";
            lblCurrentPrice.Text = $"{FormatPrice(t.Price)} USDT";

            lblPriceChange.Text = $"{t.Change24h:+0.00;-0.00}%";
            lblPriceChange.ForeColor = t.Change24h >= 0
                ? Color.FromArgb(14, 203, 129)
                : Color.FromArgb(246, 70, 93);

            lblHighLow.Text = $"H: {FormatPrice(t.High)}  |  L: {FormatPrice(t.Low)}";
            lblVolume.Text = $"Vol: {FormatVolume(t.Volume)} {ticker}";
        }

        // ─── PANEL TRADE ──────────────────────────────────────────────
        private void RefreshTradePanel()
        {
            string ticker = _selectedSymbol.Replace("USDT", "");
            lblBuyBalance.Text = $"Disponible : {_walletUSDT:N2} $";
            lblSellHolding.Text = $"Détenu : {_holdings.GetValueOrDefault(_selectedSymbol):F8} {ticker}";
            RecalcBuy();
            RecalcSell();
        }

        private void TxtBuyAmount_TextChanged(object sender, EventArgs e) => RecalcBuy();
        private void TxtSellAmount_TextChanged(object sender, EventArgs e) => RecalcSell();

        private void RecalcBuy()
        {
            string ticker = _selectedSymbol.Replace("USDT", "");
            if (decimal.TryParse(txtBuyAmount.Text, out var usdt)
                && _tickers.TryGetValue(_selectedSymbol, out var t) && t.Price > 0)
                lblBuyTotalValue.Text = $"{usdt / t.Price:F8} {ticker}";
            else
                lblBuyTotalValue.Text = $"0.00000000 {ticker}";
        }

        private void RecalcSell()
        {
            if (decimal.TryParse(txtSellAmount.Text, out var qty)
                && _tickers.TryGetValue(_selectedSymbol, out var t))
                lblSellTotalValue.Text = $"{qty * t.Price:N2} USDT";
            else
                lblSellTotalValue.Text = "0.00 USDT";
        }

        // ─── ACHAT ────────────────────────────────────────────────────
        private void BtnBuy_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtBuyAmount.Text, out var usdt) || usdt <= 0)
            { ShowError("Entre un montant USDT valide."); return; }

            if (!_tickers.TryGetValue(_selectedSymbol, out var t) || t.Price <= 0)
            { ShowError("Prix non disponible, attends quelques secondes."); return; }

            if (usdt > _walletUSDT)
            { ShowError($"Solde insuffisant.\nDisponible : {_walletUSDT:N2} $"); return; }

            decimal qty = usdt / t.Price;
            _walletUSDT -= usdt;
            _holdings[_selectedSymbol] += qty;

            string ticker = _selectedSymbol.Replace("USDT", "");
            AddToHistory("ACHAT", _selectedSymbol, qty, t.Price, usdt);
            UpdateWalletDisplay();
            RefreshTradePanel();
            UpdateCryptoRow(_selectedSymbol);
            txtBuyAmount.Clear();

            ShowSuccess($"✅ Acheté {qty:F6} {ticker}\npour {usdt:N2} $ à {FormatPrice(t.Price)} $");
        }

        // ─── VENTE ────────────────────────────────────────────────────
        private void BtnSell_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtSellAmount.Text, out var qty) || qty <= 0)
            { ShowError("Entre une quantité valide."); return; }

            if (!_tickers.TryGetValue(_selectedSymbol, out var t) || t.Price <= 0)
            { ShowError("Prix non disponible, attends quelques secondes."); return; }

            decimal held = _holdings.GetValueOrDefault(_selectedSymbol);
            if (qty > held)
            { ShowError($"Quantité insuffisante.\nDétenu : {held:F8}"); return; }

            decimal total = qty * t.Price;
            _holdings[_selectedSymbol] -= qty;
            _walletUSDT += total;

            string ticker = _selectedSymbol.Replace("USDT", "");
            AddToHistory("VENTE", _selectedSymbol, qty, t.Price, total);
            UpdateWalletDisplay();
            RefreshTradePanel();
            UpdateCryptoRow(_selectedSymbol);
            txtSellAmount.Clear();

            ShowSuccess($"🔴 Vendu {qty:F6} {ticker}\npour {total:N2} $ à {FormatPrice(t.Price)} $");
        }

        // ─── PORTEFEUILLE ─────────────────────────────────────────────
        private void UpdateWalletDisplay()
        {
            decimal cryptoValue = _holdings.Sum(h =>
                _tickers.TryGetValue(h.Key, out var t) ? h.Value * t.Price : 0);
            decimal total = _walletUSDT + cryptoValue;

            lblWalletAmount.Text = $"{total:N2} $";

            decimal gainPct = (total - 10_000m) / 100m;
            lblWalletChange.Text = $"{gainPct:+0.00;-0.00}%";
            lblWalletChange.ForeColor = total >= 10_000m
                ? Color.FromArgb(14, 203, 129)
                : Color.FromArgb(246, 70, 93);
        }

        // ─── HISTORIQUE ───────────────────────────────────────────────
        private void AddToHistory(string type, string symbol, decimal qty, decimal price, decimal total)
        {
            _history.Insert(0, new TradeRecord
            {
                Date = DateTime.Now,
                Type = type,
                Symbol = symbol,
                Quantity = qty,
                Price = price,
                Total = total
            });

            string ticker = symbol.Replace("USDT", "");
            var item = new ListViewItem(DateTime.Now.ToString("dd/MM HH:mm:ss"));
            item.SubItems.Add(type);
            item.SubItems.Add(ticker);
            item.SubItems.Add($"{qty:F6}");
            item.SubItems.Add(FormatPrice(price));
            item.SubItems.Add($"{total:N2} $");
            item.ForeColor = type == "ACHAT"
                ? Color.FromArgb(14, 203, 129)
                : Color.FromArgb(246, 70, 93);

            listViewHistory.Items.Insert(0, item);
        }

        // ─── HELPERS ─────────────────────────────────────────────────
        private static string FormatPrice(decimal p) =>
            p >= 1000 ? p.ToString("N2") :
            p >= 1 ? p.ToString("N4") :
                        p.ToString("N6");

        private static string FormatVolume(decimal v) =>
            v >= 1_000_000 ? $"{v / 1_000_000:N2}M" :
            v >= 1_000 ? $"{v / 1_000:N2}K" :
                             $"{v:N2}";

        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void ShowSuccess(string msg) =>
            MessageBox.Show(msg, "Trade exécuté", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // ─── FERMETURE ────────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _binance?.Stop();
            base.OnFormClosing(e);
        }
    }

    public class TradeRecord
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = "";
        public string Symbol { get; set; } = "";
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}