﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MVCCoreDocker.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using k8s;
using System.Net.Http;
using System.Net.Http.Headers;

namespace MVCCoreDocker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {

            IPHostEntry hostInfo = Dns.Resolve("www.yahoo.com");
            // Get the IP address list that resolves to the host names contained in the
            // Alias property.
            IPAddress[] address = hostInfo.AddressList;

            IPHostEntry host = Dns.GetHostEntry(address[0]);


            // Get the alias names of the addresses in the IP address list.
            String[] alias = hostInfo.Aliases;


            _logger = logger;
        }


        public IActionResult K8Service()
        {
            _logger.LogInformation("be4 the K8s Connetion");
#if RELEASE
            var config = KubernetesClientConfiguration.InClusterConfig();
#else
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(@"G:\aspcore\MVCCoreDocker\MVCCoreDocker\MVCCoreDocker\bin\Debug\net5.0\config");
#endif
            IKubernetes client = new Kubernetes(config);
            _logger.LogInformation("after the K8s Connetion");
             
            var list = client.ListServiceForAllNamespaces();
            _logger.LogInformation("after the K8s Pod Call");

            return View();
        }

        public async Task<IActionResult> K8s()
        {
            _logger.LogInformation("be4 the K8s Connetion");
#if RELEASE
            var config = KubernetesClientConfiguration.InClusterConfig();
#else
            var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(@"G:\aspcore\MVCCoreDocker\MVCCoreDocker\MVCCoreDocker\bin\Debug\net5.0\config");
#endif
            IKubernetes client = new Kubernetes(config);
            _logger.LogInformation("after the K8s Connetion");

            var list = client.ListNamespacedPod("default");
            _logger.LogInformation("after the K8s Pod Call");

            List<k8sClass> k8 = new List<k8sClass>();

            string ip = null;
            foreach (var item in list.Items)
            {
                k8sClass k = new k8sClass();
                k.PodName = item.Metadata.Name + " " + item.Metadata.GenerateName + "  " + "PodIP: " + item.Status.PodIP;
                k8.Add(k);
                ip = item.Status.PodIP;
                //Console.WriteLine(item.Metadata.Name);
            }

            if (list.Items.Count == 0)
            {
                // Console.WriteLine("Empty!");
            }

            // need to make a nginx pod 
            Uri myUri = new Uri("http://" + ip);

            HttpClient clienthttp = new HttpClient();

            clienthttp.BaseAddress = myUri;
            clienthttp.DefaultRequestHeaders.Accept.Clear();
            clienthttp.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var st = await clienthttp.GetAsync(myUri);

            ViewData["k8"] = k8;
            return View();
        }




        public IActionResult Index()
        {
            
            NameValueCollection coll;

            Dictionary<string, string> ss
                     = Request.Headers.ToDictionary(a => a.Key, a => string.Join(";", a.Value));


            var keys = new List<string>(ss.Keys);


            // Load Header collection into NameValueCollection object.
            Microsoft.AspNetCore.Http.IHeaderDictionary colxl = Request.Headers;
           
/*
            // Put the names of all keys into a string array.
            String[] arr1 = coll.AllKeys;
            for (loop1 = 0; loop1 < arr1.Length; loop1++)
            {
                //Response.Write("Key: " + arr1[loop1] + "<br>");
                // Get all values under this key.
                String[] arr2 = coll.GetValues(arr1[loop1]);
                for (loop2 = 0; loop2 < arr2.Length; loop2++)
                {
                   // Response.Write("Value " + loop2 + ": " + Server.HtmlEncode(arr2[loop2]) + "<br>");
                }
            }
*/
            _logger.LogInformation("test date time: " + Dns.GetHostName() + " " + DateTime.Now.ToString() 
                   + keys.ToString());

            return View();
        }

        public IActionResult Privacy()
        {

            /*  external
                kind: Service
                apiVersion: v1
                metadata:
                  name: davelapsqlserver
                spec:
                  type: ClusterIP
                  ports:
                  - port: 1433
                    targetPort: 1433
    
                ---

                kind: Endpoints
                apiVersion: v1
                metadata:
                  name: davelapsqlserver
                subsets:
                  - addresses:
                      - ip: 192.168.1.153
                    ports:
                      - port: 1433  
            */


            string connetionString = null;
            SqlConnection cnn;
#if DEBUG
            string connectionString1 = "Data Source=davelap;Initial Catalog=ContosoUniversity1XC;User ID=sa;Password=tvxs721#3";
            cnn = new SqlConnection(connectionString1);
#else
            string connectionString = "Data Source=davelapsqlserver;Initial Catalog=ContosoUniversity1XC;User ID=sa;Password=tvxs721#3";
            cnn = new SqlConnection(connectionString);
#endif
            
            List<Student153> li = new List<Student153>();

            try
            {
                SqlCommand command = new SqlCommand(
                     "SELECT * FROM Student;",
                     cnn);
                _logger.LogInformation("be4 the open call to 153");
                   cnn.Open();
                _logger.LogInformation("after the call to the open 153");

                SqlDataReader reader = command.ExecuteReader();
                var data = reader.Read();
                
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Student153 st = new Student153();
                        var ty = reader.GetInt32(0) + " " + reader.GetString(1) + reader.GetString(2);
                        st.ID = reader.GetInt32(0);
                        st.FirstMidName = reader.GetString(1);
                        st.LastName = reader.GetString(2);
                        li.Add(st);
                        _logger.LogInformation(ty);
                    }
                }
                else
                {
                   // Console.WriteLine("No rows found.");
                }
                reader.Close();

            } catch(Exception ex)
            { 
            
            }

            /*
            try
            {
          
                cnn.Open();
                 
                // MessageBox.Show("Connection Open ! ");
                cnn.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Can not open connection ! ");
            } */


            ViewData["data"] = li;
            return View();
        }



        /*

        public IActionResult About()
        {
            _logger.LogInformation("Enter About headless ***** ");
               headless
                apiVersion: v1
                kind: Service
                metadata:
                  name: headless-service
                spec:
                  clusterIP: None # <--
                  selector:
                    app: api
                  ports:
                    - protocol: TCP
                      port: 80
                      targetPort: 80 
             

            string connectionString = "";
#if DEBUG
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("localhost");
#else
            _logger.LogInformation("About be4 DNS.getHostAddresses ***** ");
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("headless-service.default.svc.cluster.local");
             _logger.LogInformation("About After DNS.getHostAddresses ***** ");
#endif



            if (ipAddresses.Length == 0)
                _logger.LogInformation("this addresses r 0");
            else
                _logger.LogInformation("the IP addresses are  " + ipAddresses.Length.ToString());

            foreach (IPAddress ip in ipAddresses)
            {
                _logger.LogInformation("this is one IP: " + ip.ToString());
                if (String.IsNullOrEmpty(connectionString))
                    connectionString = "mongodb://";
                else
                    connectionString += ",";
                connectionString += $"{ip.ToString()}:27017";
            }
            connectionString += "/database";
            //var client = new MongoClient(connectionString);
            
            

            return View();
        }

*/
        public IActionResult About()
        {
            _logger.LogInformation("Enter About headless ***** ");

            string dave = @"apiVersion: v1
            kind: Service
            metadata:
              name: headless - service
            spec:
                    clusterIP: None # <--
              selector:
                app: api
              ports:
                -protocol: TCP
                 port: 80
                  targetPort: 80";

            _logger.LogInformation(dave);
 

            string connectionString = "";
#if DEBUG
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("localhost");
#else
            _logger.LogInformation("About be4 DNS.getHostAddresses ***** ");
            System.Net.IPAddress[] ipAddresses = Dns.GetHostAddresses("headless-service.default.svc.cluster.local");
             _logger.LogInformation("About After DNS.getHostAddresses ***** ");
#endif

            List<IPClass> ipclass = new List<IPClass>();


            if (ipAddresses.Length == 0)
                _logger.LogInformation("this addresses r 0");
            else
                _logger.LogInformation("the IP addresses are  " + ipAddresses.Length.ToString());

            foreach (IPAddress ip in ipAddresses)
            {
                IPClass ipc = new IPClass();
                ipc.IPAddr = ip.ToString();
                ipclass.Add(ipc);

                _logger.LogInformation("this is one IP: " + ip.ToString());
                if (String.IsNullOrEmpty(connectionString))
                    connectionString = "mongodb://";
                else
                    connectionString += ",";
                connectionString += $"{ip.ToString()}:27017";
            }
            connectionString += "/database";
            //var client = new MongoClient(connectionString);

            if (ipclass.Count() == 0)
            {
                IPClass ipc = new IPClass();
                ipc.IPAddr = "no Headeless Resulution";
                ipclass.Add(ipc);
                IPClass ipc1 = new IPClass();
                ipc1.IPAddr = dave;
                ipclass.Add(ipc1);
                ViewData["ipdata"] = ipclass;
            }
            else
            {
                IPClass ipc1 = new IPClass();
                ipc1.IPAddr = dave;
                ipclass.Add(ipc1);
                ViewData["ipdata"] = ipclass;
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



/*
 *   using (var client = new System.Net.Http.HttpClient())
            {
                // Call *mywebapi*, and display its response in the page
                var request = new System.Net.Http.HttpRequestMessage();
                //request.RequestUri = new Uri("http://webapi2/WeatherForecast"); // ASP.NET 3 (VS 2019 only)
                request.RequestUri = new Uri("http://webapi2/api/Student"); // ASP.NET 3 (VS 2019 only)
                //request.RequestUri = new Uri("http://mywebapi/api/values/1"); // ASP.NET 2.x
                var response = await client.SendAsync(request);
                ViewData["Message"] += " and " + await response.Content.ReadAsStringAsync();
            }
 */