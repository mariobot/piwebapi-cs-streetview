using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace piwebapi_cs_streetview
{
    public class StreetViewEvent
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string WebId { get; set; }

        public bool ValuesLoaded { get; set; }

        public IList<DateTime> Timestamps { get; set; }

        public IList<double> Latitudes { get; set; }

        public IList<double> Longitudes { get; set; }

        public IList<double> Headings { get; set; }

        public IList<double> Pitches { get; set; }

        public StreetViewEvent(string name, string startDate, string endDate, string webid) {
            Name = name;
            StartTime = DateTime.Parse(startDate);
            EndTime = DateTime.Parse(endDate);
            WebId = webid;
            ValuesLoaded = false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
