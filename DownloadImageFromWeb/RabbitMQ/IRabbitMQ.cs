using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadImageFromWeb.RabbitMQ
{
    public interface IRabbitMQ
    {
        string Send(string message, string queueName, string exchangeName, string routingKeyName);
        string Receive(string queueName);
    }
}
