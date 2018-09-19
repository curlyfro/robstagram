using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace robstagram.Hubs
{
  /// <summary>
  /// Custom Hub class that manages connections, groups and messaging.
  /// </summary>
    public class AppHub : Hub
    {
      /// <summary>
      /// Can be called by any connected client. It sends data to all clients.
      /// </summary>
      /// <param name="id"></param>
      /// <returns></returns>
      public async Task SendLike(int id)
      {
        await Clients.All.SendAsync("ReceiveLike", id);
      }
    }
}
