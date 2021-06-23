using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class Auto : Form
    {
        //   bool myBool = false;
        // bool decide = false;
        public static class Resolver
        {
            private static volatile bool _loaded;

            public static void RegisterDependencyResolver()
            {
                if (!_loaded)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += OnResolve;
                    _loaded = true;
                }
            }

            private static Assembly OnResolve(object sender, ResolveEventArgs args)
            {
                Assembly execAssembly = Assembly.GetExecutingAssembly();
                string resourceName = string.Format("{0}.{1}.dll",
                     execAssembly.GetName().Name,
                    new AssemblyName(args.Name).Name);

                using (System.IO.Stream stream = execAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        int read = 0, toRead = (int)stream.Length;
                        byte[] data = new byte[toRead];
                        do
                        {
                            int n = stream.Read(data, read, data.Length - read);
                            toRead -= n;
                            read += n;
                        } while (toRead > 0);
                        return Assembly.Load(data);
                    }
                    return null;
                }
            }
        }

        public Auto()
        {
            InitializeComponent();
            Resolver.RegisterDependencyResolver();

            parse_XML();
        }

        private void invokeOnFormThread(MethodInvoker method)
        {
            if (IsHandleCreated)
                Invoke(new EventHandler(delegate { method(); }));
            else
                method();
        }

        public void parse_XML()
        {
            XNamespace ns = "http://schemas.datacontract.org/2004/07/Used.XModel";
            XDocument doc = new XDocument(new XElement(ns + "XmlImport", new XAttribute("xmlns", ns.NamespaceName), new XAttribute(XNamespace.Xmlns + "i", "http://www.w3.org/2001/XMLSchema-instance")));

            string[] comboText = { "Body-type", "Color", "Engine-type", "Gear-type", "MBClass", "Model", "Steering-wheel", "Transmission", "Origin" };
            string[] boolText = { "Certified", "OnlinePayment" };
            string[] writeText = { "Description", "Displacement", "EquipmentText", "Horse-power", "OnlinePaymentUrl", "Price", "Run", "VIN", "Year" };
            string[] multText = { "Images", "EquipmentText" };
            string[] SteeringWheel = { "Левый", "Правый" };
            Dictionary<string, string> myItems = new Dictionary<string, string>();
            myItems["Model"] = "https://used.mercedes-benz.ru/api/Dictionary/MBEngineList";
            myItems["MBClass"] = "https://used.mercedes-benz.ru/api/Dictionary/MBClassList";
            myItems["MBEquipment"] = "https://used.mercedes-benz.ru/api/Dictionary/MBEquipmentList";
            myItems["BodyType"] = "https://used.mercedes-benz.ru/api/Dictionary/MBBodyTypeList";
            myItems["Color"] = "https://used.mercedes-benz.ru/api/Dictionary/MBColorBodyList";
            myItems["Origin"] = "https://used.mercedes-benz.ru/api/Dictionary/CarOriginList";
            myItems["Transmission"] = "https://used.mercedes-benz.ru/api/Dictionary/TransmissionList";
            myItems["GearType"] = "https://used.mercedes-benz.ru/api/Dictionary/GearTypeList";
            myItems["EngineType"] = "https://used.mercedes-benz.ru/api/Dictionary/EngineTypeList";
            string[] comboNames = { "BodyType", "Color", "EngineType", "GearType", "MBClass", "Model", "Transmission", "Origin" };
            string[] boolArray = { "true", "false" };
            foreach (var combo in comboNames)
            {
                foreach (var item in this.Controls.Find(combo, true))
                {
                    ((ComboBox)item).Items.AddRange(getArray(myItems[combo]));
                }
            }
            this.SteeringWheel.Items.AddRange(SteeringWheel);
            this.OnlinePayment.Items.AddRange(boolArray);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://sales.mercedes-cardinal.ru/index.php?route=feed/ecf_used");
            Task<string> task = client.GetStringAsync(client.BaseAddress);
            string text = task.Result;
            XmlDocument document = new XmlDocument();
            document.LoadXml(text);
            XmlElement xRoot = document.DocumentElement;

            List<XmlNode> myCars = new List<XmlNode>();

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    myCars.Add(childnode);
                    listBox1.Items.Add(childnode.ChildNodes[11].InnerText);
                }
            }
            EquipmentText.Columns.Add("Опции", "");
           
            EquipmentText.RowHeadersVisible = false;
            EquipmentText.Columns[0].Width = EquipmentText.Width;
            EquipmentText.AllowUserToAddRows = false;
            Description.Multiline = true;
            Description.WordWrap = true;
            listBox1.SelectedIndexChanged += (s, e) =>
            {
                foreach (XmlNode childnode2 in myCars[listBox1.SelectedIndex].ChildNodes)
                {
                    if (multText.Contains<string>(childnode2.Name) || comboText.Contains<string>(childnode2.Name) || writeText.Contains<string>(childnode2.Name) || boolText.Contains<string>(childnode2.Name))
                    {
                        switch (childnode2.Name)
                        {
                            case "Body-type":
                                foreach (var item in this.Controls.Find("BodyType", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    BodyType1.Text = childnode2.InnerText;
                                }

                                break;

                            case "Color":
                                foreach (var item in this.Controls.Find("Color", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    Color1.Text = childnode2.InnerText;
                                }

                                break;

                            case "Engine-type":
                                foreach (var item in this.Controls.Find("EngineType", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    EngineType1.Text = childnode2.InnerText;
                                }

                                break;

                            case "Gear-type":

                                foreach (var item in this.Controls.Find("GearType", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    GearType1.Text = childnode2.InnerText;
                                }

                                break;

                            case "MBClass":

                                foreach (var item in this.Controls.Find("MBClass", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";

                                    MBClass1.Text = childnode2.InnerText;
                                }

                                break;

                            case "Model":

                                foreach (var item in this.Controls.Find("Model", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    Model1.Text = childnode2.InnerText;
                                }

                                break;

                            case "Steering-wheel":

                                foreach (var item in this.Controls.Find("SteeringWheel", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    SteeringWheel1.Text = childnode2.InnerText;
                                }

                                break;
                            case "Description":
                                string[] text_mail;
                                string my_text;
                                Description.Text = string.Empty;
                                foreach (var item in this.Controls.Find("Description", true))
                                {
                                    my_text = childnode2.InnerText.Replace(". ", ".\n");
                                    my_text = my_text.Replace("  ", "\n");
                                    text_mail = my_text.Split('\n','\r','\t');
                                    foreach (var mail in text_mail)
                                    {
                                        Description.Text += mail+Environment.NewLine;
      
                                        
                                    }
                        
                          
                                }

                                break;
                            case "EquipmentText":
                                string[] text_mail1;
                                EquipmentText.Rows.Clear();
 
                                foreach (var item in this.Controls.Find("Description", true))
                                {

                                    DataGridViewTextBoxColumn dgvAge = new DataGridViewTextBoxColumn();

       
         
                                    text_mail1 = childnode2.InnerText.Split('\n', '\r', '\t');
       
                                    foreach (var mail in text_mail1) {
                              
                                        EquipmentText.Rows.Add(mail);
                                    }


                                }

                                break;
                                
                            case "Transmission":
                                foreach (var item in this.Controls.Find("Transmission", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    Transmission1.Text = childnode2.InnerText;
                                }
                                break;

                            case "Horse-power":
                                foreach (var item in this.Controls.Find("HorsePower", true))
                                {
                                    if (Convert.ToInt32(childnode2.InnerText) > 0)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                    HorsePower1.Text = childnode2.InnerText;
                                }
                                break;

                            case "OnlinePayment":
                                foreach (var item in this.Controls.Find("OnlinePayment", true))
                                {
                                    if (((ComboBox)item).Items.Contains(childnode2.InnerText))
                                    {
                                        ((ComboBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((ComboBox)item).Text = "Нет соответсвия";
                                    OnlinePayment1.Text = childnode2.InnerText;
                                }
                                break;

                            case "OnlinePaymentUrl":
                                foreach (var item in this.Controls.Find("OnlinePaymentUrl", true))
                                {
                                    if (childnode2.InnerText.Length > 0)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                }
                                break;

                            case "Run":
                                foreach (var item in this.Controls.Find("Run", true))
                                {
                                    try { 
                                    if (Convert.ToInt32(childnode2.InnerText) > 0)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                   
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                    Run1.Text = childnode2.InnerText;
                                    }
                                    catch
                                    {
                                        ((TextBox)item).Text = "Ошибка";
                                        Run1.Text = childnode2.InnerText;
                                    }
                                }
                                break;

                            case "Displacement":
                                foreach (var item in this.Controls.Find("Displacement", true))
                                {
                                    if (Double.TryParse(childnode2.InnerText, out double displacement_number))
                                    {
                                        if (Convert.ToInt32(childnode2.InnerText) > 0)
                                        {
                                            ((TextBox)item).Text = childnode2.InnerText;
                                        }
                                        else
                                            ((TextBox)item).Text = "Нет соответсвия";
                                    }

                                    else
                                        ((TextBox)item).Text = Double.Parse(childnode2.InnerText.Replace(".", ",")).ToString();
                                    displacement2.Text = childnode2.InnerText;
                                }
                                break;

                            case "Price":
                                foreach (var item in this.Controls.Find("Price", true))
                                {
                                    if (Convert.ToInt32(childnode2.InnerText) > 0)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                    Price1.Text = childnode2.InnerText;
                                }
                                break;

                            case "Year":
                                foreach (var item in this.Controls.Find("Year", true))
                                {
                                    if (Convert.ToInt32(childnode2.InnerText) > 0)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                    Year1.Text = childnode2.InnerText;
                                }
                                break;

                            case "VIN":
                                foreach (var item in this.Controls.Find("VIN", true))
                                {
                                    if (childnode2.InnerText.Length == 17)
                                    {
                                        ((TextBox)item).Text = childnode2.InnerText;
                                    }
                                    else
                                        ((TextBox)item).Text = "Нет соответсвия";
                                    VIN1.Text = childnode2.InnerText;
                                }
                                break;

                            case "Images":
                                Form newWindows = new Form();
                                /*  if (childnode2.ChildNodes != null)
                                       foreach (var childNode3 in childnode2.ChildNodes)
                                       {
                                           TextBox textBox = new TextBox();
                                           textBox.Text = ((System.Xml.XmlElement)childNode3).InnerText;
                                           this.Controls.Add(textBox);
                                       }
                                */
                                break;
                        }
                    }
                }
            };
        }

        private string[] getArray(string myUrl)
        {
            List<string> my_list = new List<string>();
            using (HttpClient client_json = new HttpClient())
            {
                client_json.BaseAddress = new Uri(myUrl);
                Task<string> task_json = client_json.GetStringAsync(client_json.BaseAddress);
                string text_json = task_json.Result;
                JToken jArray = new JArray();
                List<string> list = jArray.ToObject<List<string>>();

                List<Dictionary<string, string>> slovar = new List<Dictionary<string, string>>();
                JToken jObject = JToken.Parse(text_json);

                foreach (var item in jObject)

                {
                    slovar.Add(JObject.FromObject(item).ToObject<Dictionary<string, string>>());
                }
                foreach (var item in slovar)
                {
                    foreach (var item1 in item)
                    {
                        if (item1.Key == "Name")
                        {
                            my_list.Add(item1.Value);
                        }
                    }
                }
                my_list.Sort();
            }
            return my_list.ToArray();
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  myBool = true;
            //  decide = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //  myBool = true;
            //  decide = false;
        }
    }
}