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

        public Form1()
        {
            InitializeComponent();
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
    }
}
