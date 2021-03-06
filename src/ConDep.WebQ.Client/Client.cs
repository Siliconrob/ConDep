﻿using System;
using System.Net;
using System.Runtime.Serialization.Json;
using ConDep.WebQ.Data;

namespace ConDep.WebQ.Client
{
  public interface IClient
  {
    WebQItem Enqueue(string environment);
    WebQItem Peek(WebQItem item);
    void Dequeue(WebQItem item);
    WebQItem SetAsStarted(WebQItem item);
  }

  public class Client : IClient
  {
    private readonly Uri _webQAddress;
    private bool? _waitingInQueue;

    public Client(Uri webQAddress)
    {
      _webQAddress = webQAddress;
    }

    public WebQItem Enqueue(string environment)
    {
      var request = WebRequest.Create(new Uri(_webQAddress, environment));
      request.Method = "PUT";
      request.ContentLength = 0;

      using (var response = request.GetResponse())
      {
        var serializer = new DataContractJsonSerializer(typeof(WebQItem));
        using (var stream = response.GetResponseStream())
        {
          if (stream != null)
          {
            _waitingInQueue = true;
            return serializer.ReadObject(stream) as WebQItem;
          }
          return new WebQItem {Position = -1};
        }
      }
    }

    public WebQItem Peek(WebQItem item)
    {
      try
      {
        var request = WebRequest.Create(new Uri(_webQAddress, item.Environment + "/" + item.Id));
        request.Method = "GET";
        request.ContentLength = 0;

        using (var response = request.GetResponse())
        {
          var serializer = new DataContractJsonSerializer(typeof(WebQItem));

          using (var stream = response.GetResponseStream())
          {
            if (stream != null)
              return serializer.ReadObject(stream) as WebQItem;
            return item;
          }
        }
      }
      catch (WebException webEx)
      {
        if (webEx.Status == WebExceptionStatus.ProtocolError && webEx.Response != null)
        {
          var exResponse = (HttpWebResponse) webEx.Response;
          if (exResponse.StatusCode == HttpStatusCode.NotFound)
            throw new WebQItemNoLongerInQueueException();
        }
        throw;
      }
    }

    public void Dequeue(WebQItem item)
    {
      if (!_waitingInQueue.HasValue) return;
      if (!_waitingInQueue.Value) return;

      var request = WebRequest.Create(new Uri(_webQAddress, item.Environment + "/" + item.Id));
      request.Method = "DELETE";
      request.ContentLength = 0;

      using (request.GetResponse())
      {
        _waitingInQueue = false;
      }
    }

    public WebQItem SetAsStarted(WebQItem item)
    {
      var request = WebRequest.Create(new Uri(_webQAddress, item.Environment + "/" + item.Id));
      request.Method = "POST";
      request.ContentLength = 0;

      using (var response = request.GetResponse())
      {
        var serializer = new DataContractJsonSerializer(typeof(WebQItem));
        using (var stream = response.GetResponseStream())
        {
          if (stream != null)
            return serializer.ReadObject(stream) as WebQItem;
          return item;
        }
      }
    }
  }
}