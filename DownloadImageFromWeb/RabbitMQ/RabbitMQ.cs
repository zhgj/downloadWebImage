using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadImageFromWeb.RabbitMQ
{
    public class RabbitMQ : IRabbitMQ
    {
        private string hostName = "112.74.23.60";

        public string Receive(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = hostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, false, false, false, null);
                    var consumer = new QueueingBasicConsumer(channel);


                    channel.BasicConsume(queueName, true, consumer);//自动删除消息

                    //channel.BasicConsume("hello", false, consumer);//需要接受方发送ack回执,删除消息

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();//挂起的操作
                    channel.BasicAck(ea.DeliveryTag, false);//与channel.BasicConsume("hello", false, null, consumer);对应

                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    return message;
                }
            }
        }

        public string Send(string message, string queueName, string exchangeName, string routingKeyName)
        {
            exchangeName = "";
            var factory = new ConnectionFactory() { HostName = hostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //声明queue
                    channel.QueueDeclare(queue: queueName,//队列名
                                         durable: false,//是否持久化
                                         exclusive: false,//排它性
                                         autoDelete: false,//一旦客户端连接断开则自动删除queue
                                         arguments: null);//如果安装了队列优先级插件则可以设置优先级

                    // string message = "Hello World!";//待发送的消息
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: exchangeName,//exchange名称
                                         routingKey: routingKeyName,//如果存在exchange,则消息被发送到名称为hello的queue的客户端
                                         basicProperties: null,
                                         body: body);//消息体
                }
            }
            return message;
        }
    }
}
