using DnsClient;
using System;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {

            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8600);
            var client = new LookupClient(endpoint);
            var result = client.ResolveService("service.consul", "servicename");

            Console.ReadLine();
        }
    }
}
