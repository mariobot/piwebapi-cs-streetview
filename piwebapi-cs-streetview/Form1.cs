using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace piwebapi_cs_streetview
{
    public partial class Form1 : Form
    {
        static string streetViewBaseUrl = "https://maps.googleapis.com/maps/api/streetview?key=AIzaSyAlCo2dLOV-Dmyt-l4WMCDt7EORd0t8vLc";
        static string baseUrl = "https://maboterow7/piwebapi/";
        private PIWebAPIClient client;
        private StreetViewEvent svEvent;
        private int currentEventCount;

        public Form1()
        {
            InitializeComponent();
            client = new PIWebAPIClient("mabotero", "Lenovo2018");
            currentEventCount = 0;
            GetEventFrames();            
        }        

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}&size={1}x{2}", streetViewBaseUrl, webBrowser1.Width - 40, webBrowser1.Height - 40));

            if (searchTb.Text != string.Empty)
            {
                sb.Append(string.Format("&location={0}", searchTb.Text));                
                webBrowser1.Navigate(sb.ToString());
            }
            else {
                MessageBox.Show("Please provide a location to search");
            }
        }

        public async void GetEventFrames() {
            try
            {
                // Get AF database WebID
                string afServerName = "MABOTEROW7";
                string afDatabaseName = "GoogleStreetView";
                string url = string.Format(@"{0}/assetdatabases?path=\\{1}\{2}", baseUrl, afServerName, afDatabaseName);
                JObject jobj = await client.GetAsync(url);
                string webId = jobj["WebId"].ToString();

                // Get Event Frames
                string templateName = "StreetView_EFTemplate";
                //url = "https://maboterow7/piwebapi/assetdatabases/F1RDwoJtf9-ekUGM_8UYLFM6tA92lZ0EAnK0KJ745imBpvbwTUFCT1RFUk9XN1xHT09HTEVTVFJFRVRWSUVX/eventframes?templatename=StreetView_EFTemplate&sortField=starttime&sortOrder=descending&startTime=-1d";
                url = string.Format(@"{0}assetdatabases/{1}/eventframes?templatename={2}&sortField=starttime&sortOrder=descending&startTime=-1d",
                                           baseUrl, webId, templateName);
                jobj = await client.GetAsync(url);

                //Populate Combobox
                for (int i = 0; i < jobj["Items"].Count() ; i++)
                {
                    StreetViewEvent svEvent = new StreetViewEvent(
                        jobj["Items"][i]["Name"].ToString(),
                        jobj["Items"][i]["StartTime"].ToString(),
                        jobj["Items"][i]["EndTime"].ToString(),
                        jobj["Items"][i]["WebId"].ToString()
                    );
                    eventFrameCb.Items.Add(svEvent);                    
                }
            }
            catch (Exception e){ MessageBox.Show(e.Message); }
        }

        private async void eventFrameCb_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                svEvent = (StreetViewEvent)eventFrameCb.SelectedItem;
                
                // Load timespan value information                
                if (!svEvent.ValuesLoaded)
                {
                    string url = string.Format("{0}/streamsets/{1}/recorded", baseUrl, svEvent.WebId);
                    JObject jobj = await client.GetAsync(url);
                    //Load Event Detail
                    LoadEventDetail(jobj);
                }

                // Display the first image
                currentEventCount = 0;
                StreetViewQuery();

                // Populate Start and End
                startTimeLbl.Text = svEvent.StartTime.ToLocalTime().ToString();
                endTimeLbl.Text = svEvent.EndTime.ToLocalTime().ToString();

            }            
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void LoadEventDetail(JObject obj) {
            JToken latitudeJobj = obj["Items"].Where(x => x["Name"].Value<string>() == "Latitude").FirstOrDefault();
            JToken longitudeJobj = obj["Items"].Where(x => x["Name"].Value<string>() == "Longitude").FirstOrDefault();
            JToken headingsJobj = obj["Items"].Where(x => x["Name"].Value<string>() == "Heading").FirstOrDefault();
            JToken pitchesJobj = obj["Items"].Where(x => x["Name"].Value<string>() == "Pitch").FirstOrDefault();

            svEvent.Timestamps = latitudeJobj["Items"].Select(x => DateTime.Parse(x["Timestamp"].ToString())).ToList();

            svEvent.Latitudes = latitudeJobj["Items"].Select(x => Convert.ToDouble(x["Value"])).ToList();
            svEvent.Longitudes = longitudeJobj["Items"].Select(x => Convert.ToDouble(x["Value"])).ToList();
            svEvent.Headings = headingsJobj["Items"].Select(x => Convert.ToDouble(x["Value"])).ToList();
            svEvent.Pitches = pitchesJobj["Items"].Select(x => Convert.ToDouble(x["Value"])).ToList();

            svEvent.ValuesLoaded = true;
        }

        private void StreetViewQuery() {

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}&size={1}x{2}", streetViewBaseUrl, webBrowser1.Width - 40, webBrowser1.Height - 40));
            sb.Append(string.Format("&location={0},{1}&heading={2}&pitch={3}",
                                     svEvent.Latitudes[currentEventCount],
                                     svEvent.Longitudes[currentEventCount],
                                     svEvent.Headings[currentEventCount],
                                     svEvent.Pitches[currentEventCount]));

            webBrowser1.Navigate(sb.ToString());

            imageLbl.Text = string.Format("{0} or {1}", currentEventCount + 1, svEvent.Timestamps.Count());
            timeLbl.Text = svEvent.Timestamps[currentEventCount].ToLocalTime().ToString();

        }
    }
}
