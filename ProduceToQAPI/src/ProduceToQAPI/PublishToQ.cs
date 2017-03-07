using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using RabbitMQ.Client;
namespace ProduceToQAPI.Controllers
{
    //C:\Temp\Work_VisualStudio\S1\RabbitMq1\RabbitMq1\PublishToQ.txt
    public class PublishToQ : RootX
    {
        private StringBuilder status = new StringBuilder();
        

        void writeLog(string s)
        {
            Console.WriteLine(s);
            System.IO.File.AppendAllText(logPath + "\r\n", s);
        }
        public override string prepareMQ()
        {
            writeLog(String.Format("{0} Connecting RabbitMQ \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            using (IConnection _Connection = _objConnectionFactory.CreateConnection())
            {
                writeLog(String.Format("{0} Connected RabbitMQ \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                using (IModel nodel = _Connection.CreateModel())
                {

                    nodel.ExchangeDeclare(exchange: exNam, type: ExchangeType.Direct, durable: true,autoDelete: false, arguments: null);
                    nodel.QueueDeclare(queue: qName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    nodel.QueueBind(queue: qName, exchange: exNam, routingKey: routKey, arguments: null);
                    writeLog(String.Format("{0} created the Q and Exchange in RabbitMQ.\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    IBasicProperties props = nodel.CreateBasicProperties();
                    props.Persistent = true;

                    Random rdm = new Random();
                    foreach (string s in data)
                    {
                        if (stop)
                        {
                            break;
                        }

                        Thread.Sleep(rdm.Next(100, postrate));
                        writeLog(String.Format("q -> {0}", s));
                        byte[] dby = Encoding.UTF8.GetBytes(s.ToCharArray());
                        writeLog(String.Format("{0} publishing the item {1} to RabbitMQ.\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), s));
                        nodel.BasicPublish(exchange: exNam, routingKey: routKey, basicProperties: props, body: dby);
                    }
                    writeLog("....Queuing is done....");
                    writeLog(String.Format("{0} All the items got queued to RabbitMQ. {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), data.Count));
                }
            }
            return writeLog("");
        }
    }
}
