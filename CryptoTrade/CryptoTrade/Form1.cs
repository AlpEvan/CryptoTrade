using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CryptoTrade
{
    public partial class Form1 : Form
    {
        private int _currentUserId;
        private decimal _walletUSDT = 10_000m;
        private readonly Dictionary<string, decimal> _holdings = new();
        private readonly Dictionary<string, TickerData> _tickers = new();
        private string _selectedSymbol = "BTCUSDT";

        private BinanceService? _binance;
        private DatabaseService _db = new DatabaseService(password: "Super");

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

        public Form1(int userId)
        {
            InitializeComponent();
            _currentUserId = userId;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (var c in _cryptos) _holdings[c.Symbol] = 0m;

            // Chargement du portefeuille
            var (wallet, savedHoldings) = _db.LoadLastPortfolio(_currentUserId);
            _walletUSDT = wallet;
            foreach (var h in savedHoldings)
            {
                if (_holdings.ContainsKey(h.Key)) _holdings[h.Key] = h.Value;
            }

            PopulateCryptoList();
            UpdateWalletDisplay();
            StartBinanceService();
        }

        private void StartBinanceService()
        {
            var symbols = _cryptos.Select(c => c.Symbol).ToList();
            _binance = new BinanceService(symbols);
            _binance.OnTickerUpdate += ticker =>
            {
                _tickers[ticker.Symbol] = ticker;
                if (!IsHandleCreated) return;
                Invoke(() => {
                    UpdateCryptoRow(ticker.Symbol);
                    if (ticker.Symbol == _selectedSymbol) UpdateDetailPanel(ticker);
                    UpdateWalletDisplay();
                });
            };
            _binance.Start();
        }

        private void PopulateCryptoList()
        {
            listViewCryptos.Items.Clear();
            foreach (var c in _cryptos)
            {
                var item = new ListViewItem($"{c.Emoji} {c.Name}");
                item.SubItems.Add("--");
                item.SubItems.Add("--");
                item.SubItems.Add("");
                item.Tag = c.Symbol;
                listViewCryptos.Items.Add(item); // FIX: Ajout de .Items ici
            }
        }

        private void UpdateCryptoRow(string symbol)
        {
            foreach (ListViewItem item in listViewCryptos.Items)
            {
                if (item.Tag?.ToString() != symbol) continue;
                if (!_tickers.TryGetValue(symbol, out var t)) break;

                item.SubItems[1].Text = t.Price.ToString("N2");
                item.SubItems[2].Text = $"{t.Change24h:+0.00;-0.00}%";
                item.SubItems[3].Text = _holdings[symbol] > 0 ? "●" : "";
                break;
            }
        }

        private void UpdateWalletDisplay()
        {
            decimal cryptoValue = _holdings.Sum(h => _tickers.ContainsKey(h.Key) ? h.Value * _tickers[h.Key].Price : 0);

            lblWalletAmount.Text = $"{_walletUSDT:N2} USDT";

            // On vérifie si tu as bien créé les labels dans le designer pour éviter que ça plante
            if (lblCryptoValue != null) lblCryptoValue.Text = $"{cryptoValue:N2} USDT";
            if (lblTotalNetWorth != null) lblTotalNetWorth.Text = $"Total: {(_walletUSDT + cryptoValue):N2} USDT";
        }

        private void BtnBuy_Click(object sender, EventArgs e)
        {
            string cleanInput = txtBuyAmount.Text.Replace(".", ",");
            if (!decimal.TryParse(cleanInput, out var usdt) || usdt <= 0 || usdt > _walletUSDT) return;
            if (!_tickers.TryGetValue(_selectedSymbol, out var t)) return;

            decimal qty = usdt / t.Price;
            _walletUSDT -= usdt;
            _holdings[_selectedSymbol] += qty;

            _db.InsertTrade(_currentUserId, "ACHAT", _selectedSymbol, qty, t.Price, usdt);
            SaveCurrentPortfolio();
            AddToHistory("ACHAT", _selectedSymbol, qty, t.Price, usdt);
            RefreshTradePanel();
        }

        private void BtnSell_Click(object sender, EventArgs e)
        {
            string cleanInput = txtSellAmount.Text.Replace(".", ",");
            decimal held = _holdings.GetValueOrDefault(_selectedSymbol);

            if (!decimal.TryParse(cleanInput, out var qty) || qty <= 0 || qty > held) return;
            if (!_tickers.TryGetValue(_selectedSymbol, out var t)) return;

            decimal total = qty * t.Price;
            _walletUSDT += total;
            _holdings[_selectedSymbol] -= qty;

            _db.InsertTrade(_currentUserId, "VENTE", _selectedSymbol, qty, t.Price, total);
            SaveCurrentPortfolio();
            AddToHistory("VENTE", _selectedSymbol, qty, t.Price, total);
            RefreshTradePanel();
        }

        private void SaveCurrentPortfolio()
        {
            decimal cryptoValue = _holdings.Sum(h => _tickers.ContainsKey(h.Key) ? h.Value * _tickers[h.Key].Price : 0);
            _db.SavePortfolioSnapshot(_currentUserId, _walletUSDT, _walletUSDT + cryptoValue, 0, _holdings);
        }

        private void UpdateDetailPanel(TickerData t)
        {
            lblCurrentPrice.Text = $"{t.Price:N2} USDT";
            lblSelectedCrypto.Text = t.Symbol;
        }

        private void RefreshTradePanel()
        {
            lblBuyBalance.Text = $"Disponible : {_walletUSDT:N2} $";
            lblSellHolding.Text = $"Détenu : {_holdings.GetValueOrDefault(_selectedSymbol):F8}";
        }

        private void AddToHistory(string type, string symbol, decimal qty, decimal price, decimal total)
        {
            var item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
            item.SubItems.AddRange(new[] { type, symbol, $"{qty:F6}", $"{price:N2}", $"{total:N2} $" });
            item.ForeColor = type == "ACHAT" ? Color.LimeGreen : Color.Red;
            listViewHistory.Items.Insert(0, item);
        }

        private void listViewCryptos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCryptos.SelectedItems.Count == 0) return;
            _selectedSymbol = listViewCryptos.SelectedItems[0].Tag?.ToString() ?? "BTCUSDT";
            if (_tickers.TryGetValue(_selectedSymbol, out var t)) UpdateDetailPanel(t);
            RefreshTradePanel();
        }

        private void txtBuyAmount_TextChanged(object sender, EventArgs e) { }
        private void txtSellAmount_TextChanged(object sender, EventArgs e) { }
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