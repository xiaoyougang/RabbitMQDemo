﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQDemo.Service
{
    public class QueueServiceBroker
    {
        static readonly string EXCHANGE_NAME_PRE = "EXCHANGE.RMQ.";

        static readonly string QUEUE_NAME1 = "RMQ2";
        static readonly string QUEUE_NAME2 = "RMQ3";
        static readonly string QUEUE_NAME3 = "RMQ4";

        static readonly string RK_RMQ1 = "DOLA.RMQ.ONE";
        static readonly string RK_RMQ2 = "DOLA.RMQ.TWO";
        static readonly string RK_RMQ3 = "SARA.RMQ.THREE";
        static readonly string RK_RMQ4 = "DOLA.#";
        static readonly string RK_RMQ5 = "SARA.#";


        public static ConnectionFactory InitConnectionFactory(RabbitmqSetting rabbitmqConnInfo)
        {
            //var uri = new Uri("amqp://127.0.0.1:5672/");
            var uri = new Uri(rabbitmqConnInfo.Uri);
            var factory = new ConnectionFactory
            {
                //UserName = "admin",
                //Password = "admin888",
                UserName = rabbitmqConnInfo.UserName,
                Password = rabbitmqConnInfo.Password,
                RequestedHeartbeat = 0,
                Endpoint = new AmqpTcpEndpoint(uri)
            };
            return factory;
        }

        public static void DeclareRoute(IModel channel, string exchangeType)
        {
            var exchangeName = EXCHANGE_NAME_PRE + exchangeType;
            //设置交换器的类型  
            channel.ExchangeDeclare(EXCHANGE_NAME_PRE + exchangeType, exchangeType);

            //声明一个队列，设置队列是否持久化，排他性，与自动删除  
            channel.QueueDeclare(QUEUE_NAME1, true, false, false, null);
            channel.QueueDeclare(QUEUE_NAME2, true, false, false, null);
            channel.QueueDeclare(QUEUE_NAME3, true, false, false, null);
            //绑定消息队列，交换器，routingkey  
            channel.QueueBind(QUEUE_NAME1, exchangeName, RK_RMQ1);
            channel.QueueBind(QUEUE_NAME2, exchangeName, RK_RMQ2);
            channel.QueueBind(QUEUE_NAME3, exchangeName, RK_RMQ3);
            channel.QueueBind(QUEUE_NAME1, exchangeName, RK_RMQ4);
            channel.QueueBind(QUEUE_NAME2, exchangeName, RK_RMQ4);
            channel.QueueBind(QUEUE_NAME3, exchangeName, RK_RMQ5);

        }

        public static string GetQueueName(string userName)
        {
            if (userName == "RMQ2")
                return QUEUE_NAME1;
            else if (userName == "RMQ3")
                return QUEUE_NAME2;
            else if (userName == "RMQ4")
                return QUEUE_NAME3;
            else
                return string.Empty;
        }

        public static void Produce(string message, string exchangeType, RabbitmqSetting rabbitmqConnInfo)
        {
            try
            {
                var factory = InitConnectionFactory(rabbitmqConnInfo);
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        DeclareRoute(channel, exchangeType);

                        var properties = channel.CreateBasicProperties();
                        //队列持久化  
                        properties.Persistent = true;
                        var bytes = Encoding.UTF8.GetBytes(message);
                        //转发消息业务规则
                        var routingKey = "DOLA.RMQ.ONE";

                        var exchangeName = EXCHANGE_NAME_PRE + exchangeType;
                        //发送信息  
                        channel.BasicPublish(exchangeName, routingKey, properties, bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string Consume(string userName, RabbitmqSetting rabbitmqConnInfo)
        {
            try
            {
                var factory = InitConnectionFactory(rabbitmqConnInfo);
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        DeclareRoute(channel, ExchangeType.Direct);

                        //定义这个队列的消费者  
                        QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
                        var qName = GetQueueName(userName);
                        //false为手动应答，true为自动应答  
                        channel.BasicConsume(qName, false, consumer);

                        BasicDeliverEventArgs ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        byte[] bytes = ea.Body;
                        var messageStr = Encoding.UTF8.GetString(bytes);
                        channel.BasicAck(ea.DeliveryTag, false);
                        return messageStr;

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
