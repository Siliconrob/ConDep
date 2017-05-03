using System;
using ConDep.WebQ.Data;

namespace ConDep.WebQ.Client
{
  public class WebQueueEventArgs : EventArgs
  {
    public WebQueueEventArgs(string message, WebQItem item)
    {
      Message = message;
      Item = item;
    }

    public string Message { get; }

    public WebQItem Item { get; }
  }
}