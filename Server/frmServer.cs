using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Server
{
    public partial class Server : Form
    {
        TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
        TcpClient client;
        String clNo;
        Dictionary<string, TcpClient> clientList = new Dictionary<string, TcpClient>();
        CancellationTokenSource cancellation = new CancellationTokenSource();
        List<string> chat = new List<string>();

        public Server()
        {
            InitializeComponent();
        }
        //resets the token when the server restarts
        private void btnStart_Click(object sender, EventArgs e)
        {
            cancellation = new CancellationTokenSource(); 
            startServer();
        }

        public void updateUI(String m)
        {
            this.Invoke((MethodInvoker)delegate // To Write the Received data
            {
                textBox1.AppendText(">>" + m + Environment.NewLine);
            });
        }

        public async void startServer()
        {
            listener.Start();
            updateUI("Server Started");
            try
            {
                int counter = 0;
                while (true)
                {
                    counter++;
                    client = await Task.Run(() => listener.AcceptTcpClientAsync(), cancellation.Token);

                    /* get username */
                    byte[] name = new byte[50];
                    NetworkStream stre = client.GetStream(); //Gets The Stream of The Connection
                    stre.Read(name, 0, name.Length); //Receives Data 
                    String username = Encoding.ASCII.GetString(name); // Converts Bytes Received to String
                    username = username.Substring(0, username.IndexOf("$"));

                    /* add to dictionary, listbox and send userList  */
                    clientList.Add(username, client);
                    listBox1.Items.Add(username);
                    updateUI("Connected to user " + username + " - " + client.Client.RemoteEndPoint);

                    await Task.Delay(1000).ContinueWith(t => {
                        sendUsersList(true);
                        sendUsersList(false, true, getWeatherInfo(), "", "");
                        sendUsersList(false, true, "", "", getIMKBInfo());
                    });


                    var c = new Thread(() => ServerReceive(client, username));
                    c.Start();

                }
            }
            catch (Exception)
            {
                listener.Stop();
            }

        }

        public void announce(string msg, string uName, bool flag)
        {
            try
            {
                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    NetworkStream broadcastStream = broadcastSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    if (flag)
                    {
                        chat.Add("gChat");
                        chat.Add(uName + ": " + msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    else
                    {
                        chat.Add("gChat");
                        chat.Add(msg);
                        broadcastBytes = ObjectToByteArray(chat);
                    }
                    broadcastStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    broadcastStream.Flush();
                    chat.Clear();
                }
            }
            catch (Exception er)
            {

            }
        }  //end broadcast function


        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }



        public void ServerReceive(TcpClient clientn, String username)
        {
            byte[] data = new byte[1000];
            String text = null;
            while (true)
            {
                try
                {
                    NetworkStream stream = clientn.GetStream(); //Gets The Stream of The Connection
                    stream.Read(data, 0, data.Length); //Receives Data 
                    List<string> parts = (List<string>)ByteArrayToObject(data);

                    switch (parts[0])
                    {
                        case "gChat":
                            this.Invoke((MethodInvoker)delegate // To Write the Received data
                            {
                                textBox1.Text += username + ": " + parts[1] + Environment.NewLine;
                            });
                            announce(parts[1], username, true);
                            break;
                    }

                    parts.Clear();
                }
                catch (Exception r)
                {
                    updateUI("Client Disconnected: " + username);
                   
                    clientList.Remove(username);

                    this.Invoke((MethodInvoker)delegate
                    {
                        listBox1.Items.Remove(username);
                        sendUsersList(false, false, "", username);
                    });
                   
                    break;
                }
            }
        }

        private void btnServerStop_Click(object sender, EventArgs e)
        {
            try
            {
                listener.Stop();
                updateUI("Server Stopped");
                foreach (var Item in clientList)
                {
                    TcpClient broadcastSocket;
                    broadcastSocket = (TcpClient)Item.Value;
                    broadcastSocket.Close();
                }
            }
            catch (SocketException er)
            {

            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient workerSocket = null;

                String clientName = listBox1.GetItemText(listBox1.SelectedItem);
                workerSocket = (TcpClient)clientList.FirstOrDefault(x => x.Key == clientName).Value; //find the client by username in dictionary
                workerSocket.Close();

            }
            catch (SocketException se)
            {
            }
        }

        public void sendUsersList(bool isJoin, bool isAnnounce = false, string weatherInfo = "", string username = "", string IMKBInfo = "")
        {
            try
            {

                byte[] userList = new byte[1024];
                byte[] weatherList = new byte[1024];
                byte[] IMKBList = new byte[1024];
                byte[] joinList = new byte[1024];
                string[] clist = listBox1.Items.OfType<string>().ToArray();
                List<string> users = new List<string>();
                List<string> weatherInformations = new List<string>();
                List<string> IMKBInformations = new List<string>();
                List<string> joinedUsers = new List<string>();
                if (isJoin == true || username != "")
                {
                    users.Add("userList");
                    joinedUsers.Add("joinedInfo");
                    foreach (String name in clist)
                    {
                        users.Add(name);

                        if (isJoin)
                        {
                            joinedUsers.Add(">>> " + name + " unlock the outer door of CinsAparment at " + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                        }
                        else
                        {                         
                            if (username != "")
                            {
                                joinedUsers.Add(">>> " + username + " has gone from CinsAparment at " + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss tt"));
                            }
                           
                        }

                    }
                    userList = ObjectToByteArray(users);
                    joinList = ObjectToByteArray(joinedUsers);
                    foreach (var Item in clientList)
                    {
                        TcpClient broadcastSocket;
                        broadcastSocket = (TcpClient)Item.Value;
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        broadcastStream.Write(userList, 0, userList.Length);
                        broadcastStream.Flush();
                        users.Clear();
                    }
                    foreach (var Item in clientList)
                    {
                        TcpClient broadcastSocket;
                        broadcastSocket = (TcpClient)Item.Value;
                        NetworkStream broadcastStream = broadcastSocket.GetStream();
                        broadcastStream.Write(joinList, 0, joinList.Length);
                        broadcastStream.Flush();
                      //  joinedUsers.Clear();
                    }
                }
                else if(isAnnounce == true)
                {
                    if (weatherInfo != "")
                    {
                        weatherInformations.Add("weatherInfo");
                        weatherInformations.Add(">>> " + weatherInfo);
                        weatherList = ObjectToByteArray(weatherInformations);
                        foreach (var Item in clientList)
                        {
                            TcpClient broadcastSocket;
                            broadcastSocket = (TcpClient)Item.Value;
                            NetworkStream broadcastStream = broadcastSocket.GetStream();
                            broadcastStream.Write(weatherList, 0, weatherList.Length);
                            broadcastStream.Flush();
                            weatherInformations.Clear();
                        }
                    }
                    else if (IMKBInfo != "")
                    {
                        IMKBInformations.Add("IMKBInfo");
                        IMKBInformations.Add(">>> " + IMKBInfo);
                        IMKBList = ObjectToByteArray(IMKBInformations);
                        foreach (var Item in clientList)
                        {
                            TcpClient broadcastSocket;
                            broadcastSocket = (TcpClient)Item.Value;
                            NetworkStream broadcastStream = broadcastSocket.GetStream();
                            broadcastStream.Write(IMKBList, 0, IMKBList.Length);
                            broadcastStream.Flush();
                            IMKBInformations.Clear();
                        }

                    }
                }



            }
            catch (SocketException se)
            {
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendUsersList(false, true, getWeatherInfo(), "","");
        }

        public string getWeatherInfo()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var client = new WebClient();
            var htmlCode = client.DownloadString("https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=13.41&current_weather=true&hourly=temperature_2m,relativehumidity_2m,windspeed_10m");
            var myDeserializedClass = JsonConvert.DeserializeObject<Root>(htmlCode);
            return " Temperature: " + myDeserializedClass.current_weather.temperature.ToString() +
                " - Speed of Wind: " + myDeserializedClass.current_weather.windspeed.ToString() +
                " - Direction of Wind: " + myDeserializedClass.current_weather.winddirection.ToString() +
                " - Time: " + myDeserializedClass.current_weather.time.ToString();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            sendUsersList(false, true, "", "", getIMKBInfo());
        }

        public string getIMKBInfo()
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            var web = new HtmlWeb();
            var document = web.Load("https://www.doviz.com/");
            var usd = document.DocumentNode.SelectSingleNode("//*[@id=\"narrow-table-with-flag\"]/tbody/tr[1]/td[2]").InnerText;
            var euro = document.DocumentNode.SelectSingleNode("//*[@id=\"narrow-table-with-flag\"]/tbody/tr[2]/td[2]").InnerText;
            return "Dollar ($) : " + usd + " - Euro (€) : "+ euro;
        }
    }
}

