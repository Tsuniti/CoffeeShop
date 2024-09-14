using System.Net;
using System.Text;
using CoffeeShop.Prowiders;

namespace CoffeeShop;

public class CoffeeShopHttpListener
{
    private readonly HttpListener _httpListener;
    private readonly int _port;
    private string _serverUrl;


    public CoffeeShopHttpListener(int port = 5001)
    {
        _port = port;
        _httpListener = new HttpListener();
        _serverUrl = $"http://localhost:{port}";
        _httpListener.Prefixes.Add(_serverUrl + "/create/");
        _httpListener.Prefixes.Add(_serverUrl + "/get-all/");
        _httpListener.Prefixes.Add(_serverUrl + "/get-later/");
        _httpListener.Prefixes.Add(_serverUrl + "/delete/");

    }
    
    
    public async Task ListenAsync()
    {

        _httpListener.Start();
            Console.WriteLine($"Server is listening {_port} port...");

        byte[] buffer;
        string responseBody = null;
        byte[] bytes = null;

            using (var db = new ApplicationContext())
            {
            while (true)
            {
                var httpContext = _httpListener.GetContext();
                var request = httpContext.Request;
                var response = httpContext.Response;
                switch (request.Url.AbsolutePath)
                    {
                        case "/create/":
                        
                            buffer = new byte[request.ContentLength64];
                            await request.InputStream.ReadAsync(buffer, 0, buffer.Length);
                            await db.AddOrderAsync(Encoding.UTF8.GetString(buffer));
                            response.OutputStream.Close();
                            break;

                        case "/get-all/":
                            responseBody = JsonProvider.Serialize(db.GetAllOrders());
                            bytes = Encoding.UTF8.GetBytes(responseBody);

                            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                            response.Close();
                            break;

                        case "/get-later/":

                        buffer = new byte[request.ContentLength64];
                        await request.InputStream.ReadAsync(buffer, 0, buffer.Length);
                        var tempDate = DateTime.Parse(Encoding.UTF8.GetString(buffer));
                        if (tempDate == null)
                        {
                            responseBody = "Wrong date and time";
                            bytes = Encoding.UTF8.GetBytes(responseBody);

                            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                            response.Close();
                        }
                        var orders = db.GetOrdersAfter(tempDate);

                        responseBody = JsonProvider.Serialize(orders);
                        bytes = Encoding.UTF8.GetBytes(responseBody);

                        await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                        response.Close();
                        break;

                        case "/delete/":

                            break;

                    default:
                        responseBody = "Wrong path";
                        bytes = Encoding.UTF8.GetBytes(responseBody);

                        await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                        response.Close();
                        break;

                    }
                }
            }
        }
    
}