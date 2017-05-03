using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ConDep.Execution.Relay
{
  [ServiceContract(Name = "RelayService", Namespace = "http://condep.io/RelayService")]
  public interface IOfferCommandRelay : IDisposable
  {
    [WebInvoke(
      Method = "POST",
      UriTemplate = "RelayService/Runbook",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json,
      BodyStyle = WebMessageBodyStyle.Bare
    )]
    [OperationContract]
    ExecutionStartedStatus StartRunbook(RelaySettings settings);

    [WebGet(
      UriTemplate = "RelayService/Runbook/{deploymentId}",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json
    )]
    [OperationContract]
    ExecutionStatus GetRunbookStatus(string deploymentId);

    [WebGet(
      UriTemplate = "RelayService/Runbook/Log/{deploymentId}/{from=0}",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json
    )]
    [OperationContract]
    ExecutionLog GetRunbookExecLog(string deploymentId, string from);

    [WebInvoke(
      Method = "DELETE",
      UriTemplate = "RelayService/Runbook",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json
    )]
    [OperationContract]
    ExecutionStatus CancelRunbok(Guid deploymentId);

    [WebGet(
      UriTemplate = "RelayService/Ping",
      RequestFormat = WebMessageFormat.Json,
      ResponseFormat = WebMessageFormat.Json
    )]
    [OperationContract]
    bool Ping();
  }
}