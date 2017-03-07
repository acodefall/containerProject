using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace ProduceToQAPI.Controllers
{
    
    public class ProduceToQController : Controller
    {
       
        PublishToQ objPublishToQ = new PublishToQ();
        public IHostingEnvironment hostingEnv;
     
        public ProduceToQController(IHostingEnvironment hosting)
        {
            hostingEnv = hosting;
        }
        // GET api/values
        [HttpGet]
        [Route("api/PostToQ/Start")]
        public String Start()
        {

            StringBuilder status1 = new StringBuilder();
            status1.Append(String.Format("Environment: {0} \r\n", hostingEnv.EnvironmentName));
            status1.Append(String.Format("{0} {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss  "), hostingEnv.EnvironmentName));
            Thread wrk = new Thread(work1);
            wrk.Start();
            
            return status1.ToString();
        }

        

        private void work1()
        {
            string logPath = "";
            string cnfg = "";
           
            Console.WriteLine("Thread -> {0}", Thread.CurrentThread.ManagedThreadId);
            if (hostingEnv.EnvironmentName.ToLower() == "production")
            {
                cnfg = "/root/appconfig/PublishToQ.txt";
            }
            else
            {
                cnfg = "Config/PublishToQ.txt";
                logPath = hostingEnv.ContentRootPath;
            }
             objPublishToQ.init(cnfg, logPath, "ProduceToQAPI");
         
            Console.WriteLine("\r\n Thread -> {0} completed", Thread.CurrentThread.ManagedThreadId);
        }

       

        

      
    }
}
