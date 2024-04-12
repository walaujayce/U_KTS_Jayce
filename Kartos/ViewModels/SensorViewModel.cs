using Kartos.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kartos.ViewModels
{
    public class SensorViewModel
    {
        public string Title { get; } = "Sensor";

        public ObservableCollection<Sensor> Sensors { get; set; }

        public SensorViewModel() 
        {
            Sensors = new ObservableCollection<Sensor>
            {
                new Sensor() { Name = "UMAP", Column = 10, Row =10},
                new Sensor() { Name = "UEXT", Column = 10, Row =10},
                new Sensor() { Name = "MDS", Column = 10, Row =10},
            };

        }
    
    }

}
