using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
namespace ComsumerHashAPI
{
    public class ConsumerNormal : RootX
    {
        IModel nodel = null;
        StringBuilder workedList = new StringBuilder();

        public override string prepareMQ()
        {
            System.IO.File.AppendAllText(logPath, "Consuming Starts...");
            System.IO.File.AppendAllText(logPath,  String.Format("{0} Connecting RabbitMQ \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            try
            {
                using (IConnection _Connection = _objConnectionFactory.CreateConnection())
                {
                    System.IO.File.AppendAllText(logPath, String.Format("{0} Connected RabbitMQ \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    nodel = _Connection.CreateModel();


                    nodel.QueueDeclare(queue: qName, durable: true, exclusive: false, autoDelete: false, arguments: null);


                    EventingBasicConsumer myEventingBasicConsumer = new EventingBasicConsumer(nodel);
                    myEventingBasicConsumer.Received += goThroughWorkEventHandler;
                    nodel.BasicConsume(queue: qName, noAck: false, consumer: myEventingBasicConsumer);

                    if (tksrc != null)
                    {
                        if (WaitHandle.WaitAll(new WaitHandle[1] { tksrc.Token.WaitHandle }))
                        {

                        }
                    }
                    nodel.Close();
                }
            }
            catch(Exception ex)
            {
                System.IO.File.AppendAllText(logPath, String.Format("{0} Exception: {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), ex.Message));
            }
            finally
            {
                System.IO.File.AppendAllText(logPath, String.Format("{0} Consumption phase is over\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            System.IO.File.WriteAllText(logPath, "Consuming ends...");

            return "";
        }

        public void goThroughWorkEventHandler(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);
            string lg1 = string.Format(" [{0}] {1}", Thread.CurrentThread.ManagedThreadId, message) ;
            System.IO.File.WriteAllText(logPath, lg1);
            workedList.Append(lg1);
            Console.WriteLine(lg1);
            string sHash1 = String.Empty;
            bool exp = false;
            try
            {
                System.IO.File.AppendAllText(logPath, String.Format("{0} Fetached an item from RabbitMQ {1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
                sHash1 = downLoad(message).Result;
            }
            catch(Exception e)
            {
                exp = true;
                string err = string.Format(" .......[{0}]   {1}   {2} EXCEPTION \r\n", Thread.CurrentThread.ManagedThreadId, sHash1, message);
                System.IO.File.WriteAllText(logPath, err);
                Console.WriteLine(err);
                workedList.Append(err);
            }
            
            if(exp == false)
            {
                string lg = string.Format("[{0}]   {1}   {2} \r\n", Thread.CurrentThread.ManagedThreadId, sHash1, message);
                System.IO.File.WriteAllText(logPath, lg);
                Console.WriteLine(lg);
                workedList.Append(lg);
            }
            nodel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        }

        async Task<string> downLoad(string url)
        {
            StringBuilder sHash = new StringBuilder();
            HttpClient clnt = new HttpClient();
            clnt.Timeout = new TimeSpan(0, 0, 30);

            System.IO.File.AppendAllText(logPath, String.Format("{0} down loading: {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), url));
            HttpResponseMessage resp = await clnt.GetAsync(url);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                System.IO.File.AppendAllText(logPath, String.Format("{0} down loading: {1} connected \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), url));
                int stdSize = 4064;
                int TotalRead = 0;


                byte[] tmp = new byte[16];
                List<Byte> byLst = new List<byte>();
                using (Stream dataStream = await resp.Content.ReadAsStreamAsync())
                {
                    System.IO.File.AppendAllText(logPath, String.Format("{0} hashing: {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), url));
                    while (true)
                    {
                        Byte[] bts = new byte[stdSize];

                        int read = dataStream.Read(bts, 0, stdSize);
                        TotalRead = TotalRead + read;
                        if (read > 0)
                        {
                            Array.Resize(ref bts, read);
                            byLst.AddRange(bts);
                        }

                        //Quit
                        if (read < stdSize)
                        {
                            //compute final hash
                            MD5 mdH = MD5.Create();
                            byte[] retVal = mdH.ComputeHash(byLst.ToArray<Byte>());

                            for (int i = 0; i < retVal.Length; i++)
                            {
                                sHash.Append(retVal[i].ToString("x2"));
                            }
                            break;
                        }
                    }
                }
                System.IO.File.AppendAllText(logPath, String.Format("{0} hash: {1} {2} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sHash, url));
            }
            return sHash.ToString();
        }

    }
}

/*
 *  channel.QueueDeclare(queue: "task_queue",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "task_queue",
                                 noAck: false,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
 */
