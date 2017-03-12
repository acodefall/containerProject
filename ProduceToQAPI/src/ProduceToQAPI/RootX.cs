using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace ProduceToQAPI.Controllers
{
    public abstract class RootX
    {
        protected CancellationTokenSource tksrc = null;
        protected ManualResetEvent TriggerEnd = new ManualResetEvent(false);
        protected ConnectionFactory _objConnectionFactory;

        protected string qName = "queueR2";
        protected string exNam = "exchangeR3";
        protected int duration = 10;
        protected int postrate = 1000;
        string pass = "guest";
        string user = "guest";
        string server = "192.168.1.5";
        string constring = "";
        protected string routKey = "urlList3";
        protected string logPath = "";
        protected List<string> data = new List<string>();

        readonly string t1 = "qname";
        readonly string t2 = "exchange";
        readonly string t3 = "duration";
        readonly string t4 = "user";
        readonly string t5 = "pass";
        readonly string t6 = "svr";
        readonly string t7 = "rkey";
        readonly string t8 = "data";
        readonly string t9 = "connectionstring";
        readonly string t10 = "postrate";
        readonly string t11 = "containerlogpath";
        protected bool stop = false;
        //running RabbitMq inside Docker
        //https://github.com/dockerfile/rabbitmq
        public void init(string config, string logPath1, string prefix)
        {
            List<string> allLines = new List<string>();
            allLines = File.ReadAllLines(config).ToList<String>();

            foreach (string ln in allLines)
            {
                if (ln.ToLower().StartsWith(t1))
                {
                    qName = ln.Substring(t1.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t2))
                {
                    exNam = ln.Substring(t2.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t3))
                {
                    duration = Convert.ToInt32(ln.Substring(t3.Length + 1));
                }
                else if (ln.ToLower().StartsWith(t4))
                {
                    user = ln.Substring(t4.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t5))
                {
                    pass = ln.Substring(t5.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t6))
                {
                    server = ln.Substring(t6.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t7))
                {
                    routKey = ln.Substring(t7.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t8))
                {
                    data.Add(ln.Substring(t8.Length + 1));
                }
                else if (ln.ToLower().StartsWith(t9))
                {
                    constring = ln.Substring(t9.Length + 1);
                }
                else if (ln.ToLower().StartsWith(t10))
                {
                    postrate = Convert.ToInt32(ln.Substring(t10.Length + 1));
                }
                else if (ln.ToLower().StartsWith(t11))
                {
                    logPath = ln.Substring(t11.Length + 1);
                }
            }


            if (logPath1.Length > 0)
            {
                logPath = string.Format("{0}\\{1}_{2}.log", logPath1, prefix, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            else if (logPath.Length > 0)
            {
                logPath = string.Format("{0}/{1}_{2}.log", logPath, prefix, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }


            System.IO.File.AppendAllText(logPath, "ConnectionFactory" + "\r\n");


            Console.WriteLine("connectRabbit start");
            _objConnectionFactory = new ConnectionFactory();
            if (constring.Length != 0)
            {
                _objConnectionFactory.Uri = constring;
            }

            Console.WriteLine("connectRabbit start");


            if (duration != 0)
            {
                tksrc = new CancellationTokenSource(1000 * 60 * duration);


                /*CancellationTokenSource tksrc = new CancellationTokenSource(1000 * 60);
                if(tksrc != null)
                {
                    if (WaitHandle.WaitAll(new WaitHandle[1] { tksrc.Token.WaitHandle }))
                    {
                        
                    }
                }    
                */

            }
        }



        public abstract string prepareMQ();


    }




}
