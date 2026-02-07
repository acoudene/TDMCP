using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TDMCPApp.Tools;

var useHttp = args.Contains("--http");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMcpServer()
    .WithTools<PatientManagementTool>();

if (useHttp)
{
  builder.Services.AddMcpServer()
      .WithHttpTransport(o => o.Stateless = true);
}
else
{
  builder.Services.AddMcpServer()
      .WithStdioServerTransport();
}

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithMetrics(b => b.AddMeter("*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation())
    .WithLogging()
    .UseOtlpExporter();

var app = builder.Build();

if (useHttp)
{
  app.MapMcp("/mcp");
}

app.Run();