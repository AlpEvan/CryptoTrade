using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
        private ClientWebSocket _ws;
        private readonly List<string> _symbols;
        public event Action<TickerData> OnTickerUpdate;

        public BinanceService(List<string> symbols) { _symbols = symbols; }

        public async void Start()
        {
            _ws = new ClientWebSocket();
            string streams = string.Join("/", _symbols.Select(s => $"{s.ToLower()}@ticker"));
            await _ws.ConnectAsync(new Uri($"wss://stream.binance.com:9443/ws/{streams}"), CancellationToken.None);

            var buffer = new byte[1024 * 8];
            while (_ws.State == WebSocketState.Open)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    OnTickerUpdate?.Invoke(new TickerData
                    {
                        Symbol = root.GetProperty("s").GetString(),
                        Price = decimal.Parse(root.GetProperty("c").GetString(), System.Globalization.CultureInfo.InvariantCulture),
                        Change24h = decimal.Parse(root.GetProperty("P").GetString(), System.Globalization.CultureInfo.InvariantCulture)
                    });
                }
            }
        }

        public void Stop() => _ws?.Dispose();
    }
}