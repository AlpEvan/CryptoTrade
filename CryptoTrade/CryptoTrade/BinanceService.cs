using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace CryptoTrade
{
    public class TickerData
    {
        public string Symbol { get; set; } = "";
        public decimal Price { get; set; }
        public decimal Change24h { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Volume { get; set; }
    }

    public class BinanceService
    {
        private ClientWebSocket? _ws;
        private CancellationTokenSource _cts = new();
        private readonly List<string> _symbols;

        public event Action<TickerData>? OnTickerUpdate;
        public event Action<bool, string>? OnStatusChange;
        public event Action<string>? OnDebugLog; // ← logs de debug

        public BinanceService(List<string> symbols)
        {
            _symbols = symbols;
        }

        public void Start() => _ = ConnectAsync();

        public void Stop()
        {
            _cts.Cancel();
            _ws?.Dispose();
        }

        private async Task ConnectAsync()
        {
            // Essaie plusieurs domaines Binance
            var domains = new[]
            {
                "stream.binance.com:9443",
                "stream.binance.com:443",
                "stream1.binance.com:9443",
            };

            var streams = string.Join("/", _symbols.Select(s => s.ToLower() + "@miniTicker"));

            while (!_cts.Token.IsCancellationRequested)
            {
                foreach (var domain in domains)
                {
                    if (_cts.Token.IsCancellationRequested) break;
                    try
                    {
                        var uri = new Uri($"wss://{domain}/stream?streams={streams}");
                        OnDebugLog?.Invoke($"Tentative connexion : {uri}");
                        OnStatusChange?.Invoke(false, $"Connexion à {domain}...");

                        _ws = new ClientWebSocket();
                        _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);

                        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                        using var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, timeoutCts.Token);

                        await _ws.ConnectAsync(uri, linked.Token);
                        OnDebugLog?.Invoke($"✅ Connecté à {domain}");
                        OnStatusChange?.Invoke(true, $"Connecté — données live ({domain})");

                        await ReceiveLoopAsync();

                        // Si on arrive ici, la connexion a été fermée proprement
                        OnDebugLog?.Invoke("Connexion fermée proprement");
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        if (_cts.Token.IsCancellationRequested) return;
                        OnDebugLog?.Invoke($"Timeout sur {domain}, essai suivant...");
                    }
                    catch (Exception ex)
                    {
                        OnDebugLog?.Invoke($"❌ Erreur {domain}: {ex.Message}");
                        OnStatusChange?.Invoke(false, $"Erreur: {ex.Message}");
                    }
                    finally
                    {
                        _ws?.Dispose();
                        _ws = null;
                    }
                }

                if (!_cts.Token.IsCancellationRequested)
                {
                    OnDebugLog?.Invoke("Tous les domaines ont échoué, retry dans 5s...");
                    OnStatusChange?.Invoke(false, "Reconnexion dans 5s...");
                    await Task.Delay(5000, _cts.Token).ContinueWith(_ => { });
                }
            }
        }

        private async Task ReceiveLoopAsync()
        {
            var buffer = new byte[16384];
            int msgCount = 0;

            while (_ws!.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
            {
                using var ms = new System.IO.MemoryStream();
                WebSocketReceiveResult result;

                do
                {
                    result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                    ms.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    OnDebugLog?.Invoke("Serveur a fermé la connexion");
                    break;
                }

                var json = Encoding.UTF8.GetString(ms.ToArray());
                msgCount++;

                // Log les 3 premiers messages pour debug
                if (msgCount <= 3)
                    OnDebugLog?.Invoke($"Message #{msgCount}: {json[..Math.Min(200, json.Length)]}");

                ParseAndNotify(json);
            }
        }

        private void ParseAndNotify(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Format stream combiné : { "stream": "btcusdt@miniTicker", "data": {...} }
                if (root.TryGetProperty("data", out var data))
                {
                    var ticker = ParseTicker(data);
                    if (ticker != null)
                        OnTickerUpdate?.Invoke(ticker);
                }
                // Format stream simple : directement les données
                else if (root.TryGetProperty("s", out _))
                {
                    var ticker = ParseTicker(root);
                    if (ticker != null)
                        OnTickerUpdate?.Invoke(ticker);
                }
                else
                {
                    OnDebugLog?.Invoke($"Format inconnu: {json[..Math.Min(100, json.Length)]}");
                }
            }
            catch (Exception ex)
            {
                OnDebugLog?.Invoke($"Erreur parsing: {ex.Message} | JSON: {json[..Math.Min(100, json.Length)]}");
            }
        }

        private static TickerData? ParseTicker(JsonElement data)
        {
            try
            {
                // "24hrMiniTicker" utilise "c" pour close price aussi, 
                // mais vérifions d'abord le type d'event
                string eventType = data.TryGetProperty("e", out var e) ? e.GetString() ?? "" : "";

                // Les deux formats supportés : miniTicker et 24hrMiniTicker
                // Champs communs : s=symbol, c=closePrice, h=high, l=low, v=volume, P n'existe pas dans 24hr
                string symbol = data.GetProperty("s").GetString()!;
                decimal price = ParseDecimal(data.GetProperty("c").GetString());
                decimal high = ParseDecimal(data.GetProperty("h").GetString());
                decimal low = ParseDecimal(data.GetProperty("l").GetString());
                decimal vol = ParseDecimal(data.GetProperty("v").GetString());

                // "P" (variation %) n'existe que dans miniTicker, pas dans 24hrMiniTicker
                // Dans 24hrMiniTicker on calcule manuellement avec o (open) et c (close)
                decimal change = 0;
                if (data.TryGetProperty("P", out var pProp))
                {
                    change = ParseDecimal(pProp.GetString());
                }
                else if (data.TryGetProperty("o", out var openProp))
                {
                    decimal open = ParseDecimal(openProp.GetString());
                    if (open > 0)
                        change = ((price - open) / open) * 100m;
                }

                return new TickerData
                {
                    Symbol = symbol,
                    Price = price,
                    Change24h = change,
                    High = high,
                    Low = low,
                    Volume = vol,
                };
            }
            catch { return null; }
        }

        private static decimal ParseDecimal(string? s) =>
            decimal.TryParse(s,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var v) ? v : 0;
    }
}