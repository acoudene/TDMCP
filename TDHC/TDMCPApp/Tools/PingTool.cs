using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TDMCPApp.Tools;

[McpServerToolType]
public sealed class PingTool
{
  [McpServerTool, Description("Get a ping to the server.")]
  [McpMeta("category", "ping")]
  public async Task<string> Ping(string message)
  {
    return $"Received: {message}";
  }
}