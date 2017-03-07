using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace ComsumerHashAPI.Controllers
{
    
    public class Consume1Controller : Controller
    {

        private IHostingEnvironment hostingEnv;
        ConsumerNormal objConsumerNormal = new ConsumerNormal();

        public Consume1Controller(IHostingEnvironment env)
        {
            hostingEnv = env;
        }
        // GET api/values
        [Route("api/Consume1/Start")]
        [HttpGet]
        public string Start()
        {
            StringBuilder status1 = new StringBuilder();
            status1.Append (String.Format("Environment: {0} \r\n", hostingEnv.EnvironmentName));
            status1.Append(String.Format("{0} {1} \r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),hostingEnv.EnvironmentName));
            


            Thread wrk = new Thread(work1);
          
            wrk.Start();


            return status1.ToString();
        }

        private void work1()
        {
			string logPath = "";
            string cnfg = "";
           
            
			if (hostingEnv.EnvironmentName.ToLower() == "production")
            {
                cnfg = "/root/appconfig/ConsumerNormal.txt";
            }
            else
            {
                cnfg = "Config/ConsumerNormal.txt";
                logPath = hostingEnv.ContentRootPath;
            }
          

            objConsumerNormal.init(cnfg, logPath, "ComsumerHashAPI");
            objConsumerNormal.prepareMQ();
        }
    }
}
