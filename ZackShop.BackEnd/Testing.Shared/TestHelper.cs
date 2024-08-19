using System.Net.Sockets;
using System.Net;

namespace Testing.Shared
{
    public static class TestHelper
    {
        public static int GetFreePort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
