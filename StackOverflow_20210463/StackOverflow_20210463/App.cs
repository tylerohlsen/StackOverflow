using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace AppServices
{
    public class App : IApp
    {
        public cSecurity _csec;

        public string GetItems(int agentID, string agentName)
        {
            // Need to use some functions from the cSecurity class here???
            return _csec.getItems();
        }
    }
}
