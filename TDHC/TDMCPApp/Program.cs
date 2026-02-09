using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using TDMCPApp.Tools;

bool useHttp = args.Contains("--http");
bool useStateless = args.Contains("--stateless");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMcpServer()
    .WithTools<PatientManagementTool>();

if (useHttp)
{
  if (useStateless)
  {
    builder.Services.AddMcpServer()
      .WithHttpTransport(o => o.Stateless = true);
  }
  else
  {
    builder.Services.AddMcpServer()
        .WithHttpTransport();
  }
}
else
{
  builder.Services.AddMcpServer()
      .WithStdioServerTransport();
}

//builder.Services.AddOpenTelemetry()
//    .WithTracing(b => b.AddSource("*")
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation())
//    .WithMetrics(b => b.AddMeter("*")
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation())
//    .WithLogging()
//    .UseOtlpExporter();

var app = builder.Build();

if (useHttp)
{
  app.MapMcp("/mcp");
}

app.Run();