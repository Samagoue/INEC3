using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace INEC3.SignalR
{
    public class RealTimeMapHub : Hub
    {
        //public void Send(string name, string stateFipsCode, string color, string note)
        public void Send(dynamic result)
        {
            //Clients.All.mapUpdate(name, stateFipsCode, color, note);
            Clients.All.mapUpdate(result);
        }
    }
}