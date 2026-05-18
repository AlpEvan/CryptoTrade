namespace CryptoTrade
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            // Panels principaux
            this.panelTop = new Panel();
            this.panelLeft = new Panel();
            this.panelCenter = new Panel();
            this.panelBottom = new Panel();

            // Header
            this.lblTitle = new Label();
            this.lblWalletLabel = new Label();
            this.lblWalletAmount = new Label();
            this.lblWalletChange = new Label();
            this.panelWallet = new Panel();

            // Liste des cryptos
            this.lblCryptoListTitle = new Label();
            this.listViewCryptos = new ListView();
            this.colSymbol = new ColumnHeader();
            this.colPrice = new ColumnHeader();
            this.colChange = new ColumnHeader();
            this.colVolume = new ColumnHeader();

            // Zone centrale - Détail crypto sélectionnée
            this.panelCryptoDetail = new Panel();
            this.lblSelectedCrypto = new Label();
            this.lblCurrentPrice = new Label();
            this.lblPriceChange = new Label();
            this.lblHighLow = new Label();
            this.lblVolume = new Label();

            // Zone d'achat/vente
            this.panelTrade = new Panel();
            this.lblTradeTitle = new Label();
            this.tabControlTrade = new TabControl();
            this.tabBuy = new TabPage();
            this.tabSell = new TabPage();

            // Achat
            this.lblBuyAmount = new Label();
            this.txtBuyAmount = new TextBox();
            this.lblBuyTotal = new Label();
            this.lblBuyTotalValue = new Label();
            this.btnBuy = new Button();
            this.lblBuyBalance = new Label();

            // Vente
            this.lblSellAmount = new Label();
            this.txtSellAmount = new TextBox();
            this.lblSellTotal = new Label();
            this.lblSellTotalValue = new Label();
            this.btnSell = new Button();
            this.lblSellHolding = new Label();

            // Historique des trades
            this.lblHistoryTitle = new Label();
            this.listViewHistory = new ListView();
            this.colHistDate = new ColumnHeader();
            this.colHistType = new ColumnHeader();
            this.colHistCrypto = new ColumnHeader();
            this.colHistAmount = new ColumnHeader();
            this.colHistPrice = new ColumnHeader();
            this.colHistTotal = new ColumnHeader();

            // Status bar
            this.lblStatus = new Label();
            this.lblLiveIndicator = new Label();

            this.panelTop.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelCenter.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelWallet.SuspendLayout();
            this.panelCryptoDetail.SuspendLayout();
            this.panelTrade.SuspendLayout();
            this.tabControlTrade.SuspendLayout();
            this.tabBuy.SuspendLayout();
            this.tabSell.SuspendLayout();
            SuspendLayout();

            // ─── FORM ───────────────────────────────────────────────
            this.Text = "CryptoTrade";
            this.ClientSize = new Size(1280, 800);
            this.MinimumSize = new Size(1100, 700);
            this.BackColor = Color.FromArgb(18, 18, 26);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.Load += Form1_Load;

            // ─── PANEL TOP (Header) ──────────────────────────────────
            this.panelTop.Dock = DockStyle.Top;
            this.panelTop.Height = 65;
            this.panelTop.BackColor = Color.FromArgb(22, 22, 35);
            this.panelTop.Padding = new Padding(15, 0, 15, 0);

            // Logo / Titre
            this.lblTitle.Text = "₿ CryptoTrade";
            this.lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(247, 185, 36);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new Point(20, 18);

            // Panel Portefeuille (haut droite)
            this.panelWallet.Size = new Size(280, 50);
            this.panelWallet.Location = new Point(975, 8);
            this.panelWallet.BackColor = Color.FromArgb(30, 30, 45);
            this.panelWallet.BorderStyle = BorderStyle.None;

            this.lblWalletLabel.Text = "💰 Portefeuille";
            this.lblWalletLabel.Font = new Font("Segoe UI", 8F);
            this.lblWalletLabel.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblWalletLabel.AutoSize = true;
            this.lblWalletLabel.Location = new Point(10, 5);

            this.lblWalletAmount.Text = "10 000,00 $";
            this.lblWalletAmount.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            this.lblWalletAmount.ForeColor = Color.FromArgb(247, 185, 36);
            this.lblWalletAmount.AutoSize = true;
            this.lblWalletAmount.Location = new Point(10, 22);

            this.lblWalletChange.Text = "+0,00%";
            this.lblWalletChange.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            this.lblWalletChange.ForeColor = Color.FromArgb(14, 203, 129);
            this.lblWalletChange.AutoSize = true;
            this.lblWalletChange.Location = new Point(195, 28);

            this.panelWallet.Controls.AddRange(new Control[] {
                lblWalletLabel, lblWalletAmount, lblWalletChange
            });
            this.panelTop.Controls.AddRange(new Control[] {
                lblTitle, panelWallet
            });

            // ─── PANEL LEFT (Liste cryptos) ──────────────────────────
            this.panelLeft.Dock = DockStyle.Left;
            this.panelLeft.Width = 370;
            this.panelLeft.BackColor = Color.FromArgb(22, 22, 35);
            this.panelLeft.Padding = new Padding(10);

            this.lblCryptoListTitle.Text = "📊 Marchés";
            this.lblCryptoListTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblCryptoListTitle.ForeColor = Color.White;
            this.lblCryptoListTitle.Location = new Point(10, 10);
            this.lblCryptoListTitle.AutoSize = true;

            this.listViewCryptos.Location = new Point(10, 40);
            this.listViewCryptos.Size = new Size(350, 660);
            this.listViewCryptos.View = View.Details;
            this.listViewCryptos.FullRowSelect = true;
            this.listViewCryptos.GridLines = false;
            this.listViewCryptos.BackColor = Color.FromArgb(28, 28, 42);
            this.listViewCryptos.ForeColor = Color.White;
            this.listViewCryptos.BorderStyle = BorderStyle.None;
            this.listViewCryptos.Font = new Font("Segoe UI", 9.5F);
            this.listViewCryptos.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.listViewCryptos.SelectedIndexChanged += ListViewCryptos_SelectedIndexChanged;

            this.colSymbol.Text = "Crypto";
            this.colSymbol.Width = 100;
            this.colPrice.Text = "Prix (USDT)";
            this.colPrice.Width = 120;
            this.colChange.Text = "24h %";
            this.colChange.Width = 70;
            this.colVolume.Text = "Volume";
            this.colVolume.Width = 60;

            this.listViewCryptos.Columns.AddRange(new ColumnHeader[] {
                colSymbol, colPrice, colChange, colVolume
            });
            this.panelLeft.Controls.AddRange(new Control[] {
                lblCryptoListTitle, listViewCryptos
            });

            // ─── PANEL CENTER ────────────────────────────────────────
            this.panelCenter.Dock = DockStyle.Fill;
            this.panelCenter.BackColor = Color.FromArgb(18, 18, 26);
            this.panelCenter.Padding = new Padding(15);

            // Détail crypto sélectionnée
            this.panelCryptoDetail.Location = new Point(15, 10);
            this.panelCryptoDetail.Size = new Size(560, 100);
            this.panelCryptoDetail.BackColor = Color.FromArgb(22, 22, 35);

            this.lblSelectedCrypto.Text = "Bitcoin (BTC)";
            this.lblSelectedCrypto.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.lblSelectedCrypto.ForeColor = Color.White;
            this.lblSelectedCrypto.Location = new Point(15, 12);
            this.lblSelectedCrypto.AutoSize = true;

            this.lblCurrentPrice.Text = "-- USDT";
            this.lblCurrentPrice.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            this.lblCurrentPrice.ForeColor = Color.FromArgb(247, 185, 36);
            this.lblCurrentPrice.Location = new Point(15, 40);
            this.lblCurrentPrice.AutoSize = true;

            this.lblPriceChange.Text = "+0.00%";
            this.lblPriceChange.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblPriceChange.ForeColor = Color.FromArgb(14, 203, 129);
            this.lblPriceChange.Location = new Point(230, 50);
            this.lblPriceChange.AutoSize = true;

            this.lblHighLow.Text = "H: --  |  L: --";
            this.lblHighLow.Font = new Font("Segoe UI", 8.5F);
            this.lblHighLow.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblHighLow.Location = new Point(15, 78);
            this.lblHighLow.AutoSize = true;

            this.lblVolume.Text = "Vol: --";
            this.lblVolume.Font = new Font("Segoe UI", 8.5F);
            this.lblVolume.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblVolume.Location = new Point(200, 78);
            this.lblVolume.AutoSize = true;

            this.panelCryptoDetail.Controls.AddRange(new Control[] {
                lblSelectedCrypto, lblCurrentPrice, lblPriceChange, lblHighLow, lblVolume
            });

            // ─── PANEL TRADE (Achat / Vente) ────────────────────────
            this.panelTrade.Location = new Point(15, 120);
            this.panelTrade.Size = new Size(560, 220);
            this.panelTrade.BackColor = Color.FromArgb(22, 22, 35);

            this.lblTradeTitle.Text = "⚡ Trader";
            this.lblTradeTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblTradeTitle.ForeColor = Color.White;
            this.lblTradeTitle.Location = new Point(15, 12);
            this.lblTradeTitle.AutoSize = true;

            this.tabControlTrade.Location = new Point(10, 40);
            this.tabControlTrade.Size = new Size(540, 165);
            this.tabControlTrade.Appearance = TabAppearance.FlatButtons;
            this.tabControlTrade.BackColor = Color.FromArgb(22, 22, 35);
            this.tabControlTrade.DrawMode = TabDrawMode.OwnerDrawFixed;

            // Tab Acheter
            this.tabBuy.Text = "  Acheter  ";
            this.tabBuy.BackColor = Color.FromArgb(22, 22, 35);

            this.lblBuyAmount.Text = "Montant (USDT) :";
            this.lblBuyAmount.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblBuyAmount.Location = new Point(10, 15);
            this.lblBuyAmount.AutoSize = true;

            this.txtBuyAmount.Location = new Point(10, 35);
            this.txtBuyAmount.Size = new Size(200, 28);
            this.txtBuyAmount.BackColor = Color.FromArgb(35, 35, 52);
            this.txtBuyAmount.ForeColor = Color.White;
            this.txtBuyAmount.BorderStyle = BorderStyle.FixedSingle;
            this.txtBuyAmount.Font = new Font("Segoe UI", 10F);
            this.txtBuyAmount.PlaceholderText = "0.00";
            this.txtBuyAmount.TextChanged += TxtBuyAmount_TextChanged;

            this.lblBuyTotal.Text = "Vous recevrez :";
            this.lblBuyTotal.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblBuyTotal.Location = new Point(10, 75);
            this.lblBuyTotal.AutoSize = true;

            this.lblBuyTotalValue.Text = "0.00000000 BTC";
            this.lblBuyTotalValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblBuyTotalValue.ForeColor = Color.White;
            this.lblBuyTotalValue.Location = new Point(10, 93);
            this.lblBuyTotalValue.AutoSize = true;

            this.lblBuyBalance.Text = "Disponible : 10 000,00 $";
            this.lblBuyBalance.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblBuyBalance.Font = new Font("Segoe UI", 8.5F);
            this.lblBuyBalance.Location = new Point(230, 40);
            this.lblBuyBalance.AutoSize = true;

            this.btnBuy.Text = "✅ ACHETER";
            this.btnBuy.Location = new Point(350, 80);
            this.btnBuy.Size = new Size(160, 42);
            this.btnBuy.BackColor = Color.FromArgb(14, 203, 129);
            this.btnBuy.ForeColor = Color.White;
            this.btnBuy.FlatStyle = FlatStyle.Flat;
            this.btnBuy.FlatAppearance.BorderSize = 0;
            this.btnBuy.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnBuy.Cursor = Cursors.Hand;
            this.btnBuy.Click += BtnBuy_Click;

            this.tabBuy.Controls.AddRange(new Control[] {
                lblBuyAmount, txtBuyAmount, lblBuyTotal, lblBuyTotalValue, lblBuyBalance, btnBuy
            });

            // Tab Vendre
            this.tabSell.Text = "  Vendre  ";
            this.tabSell.BackColor = Color.FromArgb(22, 22, 35);

            this.lblSellAmount.Text = "Quantité (crypto) :";
            this.lblSellAmount.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblSellAmount.Location = new Point(10, 15);
            this.lblSellAmount.AutoSize = true;

            this.txtSellAmount.Location = new Point(10, 35);
            this.txtSellAmount.Size = new Size(200, 28);
            this.txtSellAmount.BackColor = Color.FromArgb(35, 35, 52);
            this.txtSellAmount.ForeColor = Color.White;
            this.txtSellAmount.BorderStyle = BorderStyle.FixedSingle;
            this.txtSellAmount.Font = new Font("Segoe UI", 10F);
            this.txtSellAmount.PlaceholderText = "0.00000000";
            this.txtSellAmount.TextChanged += TxtSellAmount_TextChanged;

            this.lblSellTotal.Text = "Vous recevrez :";
            this.lblSellTotal.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblSellTotal.Location = new Point(10, 75);
            this.lblSellTotal.AutoSize = true;

            this.lblSellTotalValue.Text = "0.00 USDT";
            this.lblSellTotalValue.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.lblSellTotalValue.ForeColor = Color.White;
            this.lblSellTotalValue.Location = new Point(10, 93);
            this.lblSellTotalValue.AutoSize = true;

            this.lblSellHolding.Text = "Détenu : 0.00000000 BTC";
            this.lblSellHolding.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblSellHolding.Font = new Font("Segoe UI", 8.5F);
            this.lblSellHolding.Location = new Point(230, 40);
            this.lblSellHolding.AutoSize = true;

            this.btnSell.Text = "🔴 VENDRE";
            this.btnSell.Location = new Point(350, 80);
            this.btnSell.Size = new Size(160, 42);
            this.btnSell.BackColor = Color.FromArgb(246, 70, 93);
            this.btnSell.ForeColor = Color.White;
            this.btnSell.FlatStyle = FlatStyle.Flat;
            this.btnSell.FlatAppearance.BorderSize = 0;
            this.btnSell.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnSell.Cursor = Cursors.Hand;
            this.btnSell.Click += BtnSell_Click;

            this.tabSell.Controls.AddRange(new Control[] {
                lblSellAmount, txtSellAmount, lblSellTotal, lblSellTotalValue, lblSellHolding, btnSell
            });

            this.tabControlTrade.TabPages.AddRange(new TabPage[] { tabBuy, tabSell });
            this.panelTrade.Controls.AddRange(new Control[] { lblTradeTitle, tabControlTrade });

            // ─── HISTORIQUE ──────────────────────────────────────────
            this.lblHistoryTitle.Text = "📋 Historique des trades";
            this.lblHistoryTitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.lblHistoryTitle.ForeColor = Color.White;
            this.lblHistoryTitle.Location = new Point(15, 355);
            this.lblHistoryTitle.AutoSize = true;

            this.listViewHistory.Location = new Point(15, 385);
            this.listViewHistory.Size = new Size(560, 320);
            this.listViewHistory.View = View.Details;
            this.listViewHistory.FullRowSelect = true;
            this.listViewHistory.GridLines = false;
            this.listViewHistory.BackColor = Color.FromArgb(22, 22, 35);
            this.listViewHistory.ForeColor = Color.White;
            this.listViewHistory.BorderStyle = BorderStyle.None;
            this.listViewHistory.Font = new Font("Segoe UI", 9F);

            this.colHistDate.Text = "Date";
            this.colHistDate.Width = 130;
            this.colHistType.Text = "Type";
            this.colHistType.Width = 65;
            this.colHistCrypto.Text = "Crypto";
            this.colHistCrypto.Width = 70;
            this.colHistAmount.Text = "Quantité";
            this.colHistAmount.Width = 100;
            this.colHistPrice.Text = "Prix";
            this.colHistPrice.Width = 100;
            this.colHistTotal.Text = "Total";
            this.colHistTotal.Width = 95;

            this.listViewHistory.Columns.AddRange(new ColumnHeader[] {
                colHistDate, colHistType, colHistCrypto, colHistAmount, colHistPrice, colHistTotal
            });

            this.panelCenter.Controls.AddRange(new Control[] {
                panelCryptoDetail, panelTrade, lblHistoryTitle, listViewHistory
            });

            // ─── PANEL BOTTOM (Status bar) ───────────────────────────
            this.panelBottom.Dock = DockStyle.Bottom;
            this.panelBottom.Height = 28;
            this.panelBottom.BackColor = Color.FromArgb(15, 15, 22);

            this.lblLiveIndicator.Text = "● LIVE";
            this.lblLiveIndicator.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            this.lblLiveIndicator.ForeColor = Color.FromArgb(14, 203, 129);
            this.lblLiveIndicator.Location = new Point(10, 6);
            this.lblLiveIndicator.AutoSize = true;

            this.lblStatus.Text = "Connexion en cours...";
            this.lblStatus.Font = new Font("Segoe UI", 8F);
            this.lblStatus.ForeColor = Color.FromArgb(150, 150, 180);
            this.lblStatus.Location = new Point(55, 6);
            this.lblStatus.AutoSize = true;

            this.panelBottom.Controls.AddRange(new Control[] { lblLiveIndicator, lblStatus });

            // ─── AJOUT AU FORM ───────────────────────────────────────
            this.Controls.AddRange(new Control[] {
                panelCenter, panelLeft, panelTop, panelBottom
            });

            this.panelTop.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelCenter.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelWallet.ResumeLayout(false);
            this.panelCryptoDetail.ResumeLayout(false);
            this.panelTrade.ResumeLayout(false);
            this.tabControlTrade.ResumeLayout(false);
            this.tabBuy.ResumeLayout(false);
            this.tabSell.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // ─── DÉCLARATIONS ────────────────────────────────────────────
        private Panel panelTop, panelLeft, panelCenter, panelBottom, panelWallet;
        private Panel panelCryptoDetail, panelTrade;
        private Label lblTitle, lblWalletLabel, lblWalletAmount, lblWalletChange;
        private Label lblCryptoListTitle, lblSelectedCrypto, lblCurrentPrice;
        private Label lblPriceChange, lblHighLow, lblVolume;
        private Label lblTradeTitle, lblBuyAmount, lblBuyTotal, lblBuyTotalValue, lblBuyBalance;
        private Label lblSellAmount, lblSellTotal, lblSellTotalValue, lblSellHolding;
        private Label lblHistoryTitle, lblStatus, lblLiveIndicator;
        private ListView listViewCryptos, listViewHistory;
        private ColumnHeader colSymbol, colPrice, colChange, colVolume;
        private ColumnHeader colHistDate, colHistType, colHistCrypto, colHistAmount, colHistPrice, colHistTotal;
        private TextBox txtBuyAmount, txtSellAmount;
        private Button btnBuy, btnSell;
        private TabControl tabControlTrade;
        private TabPage tabBuy, tabSell;
    }
}