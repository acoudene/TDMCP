using ModelContextProtocol.Server;
using Simple.OData.Client;
using System.ComponentModel;
using System.Text.Json;
using TD.Technical.ODataClient.ConfigModel;

namespace TDMCPApp.Tools;

[McpServerToolType]
public sealed class TDDatabaseTool
{
  private ConfigurationModelsContext CreateContext() => new ConfigurationModelsContext(new Uri("https://tdhc-app-dev-2.technidata.net:38431/TDHC9XACE/Technical/TDDatabaseService.svc"));  

  private readonly string[] _entitySets = new[]
    {
        "INDIC_SETTINGS", "INDICATOR_TYPES", "USER_INDICATORS", "PARAM_VALUES_DEFAULT", "PARAM_VALUES_SECTION", "PARAM_VALUES_VIEWS", "DEFAULT_FUNCTIONS", "DEFFUNCTIONS_PRESENTATION", "DEFFUNCTIONS_SECTIONS", "SAMPLES_CHRONOID", "DICT_DEVICE_PARAMETERS", "SIMPLE_NON_CONFORMITY", "SP_REQUESTS_NC", "SP_TUBES_NC", "EVENTLISTS", "USER_DB_SESSIONS", "SYSSESSIONID", "DICT_WEEK_DAYS", "DICT_TYPICAL_WEEKS", "DICT_TYPICAL_WEEKS_DPTS", "DICT_TYPICAL_WEEKS_LABOS", "RESPONSIBLES", "SHIFT_DEFINITION", "ALERT_DISTRIBUTION", "ALERT_EVENTS", "COUNTERS_VALUES", "DICT_ALERTS", "DICT_COUNTER_PROPERTIES", "DICT_COUNTER_THRESHOLDS", "DICT_COUNTERS", "DICT_DISTRIBUTION_CONTACTS", "DICT_DISTRIBUTION_LIST", "DICT_DISTRIBUTION_USERS", "REQUEST_CLINICALNOTES", "UNIX_MESSAGES", "BINARY_DATA", "BINARY_PARAMETERS", "SYSCONSTRAINT", "PROBEREQUESTSWITHPENDINGTESTS", "PROBEREQUESTSWITHCRITICALTESTS", "PROBEREQUESTSWITHSAMPTOBERECS", "PROBEREQUESTSWITHPENDINGMBREQS", "PROBEREQUESTSWITHTOBEPHONEDMBS", "DICT_QA_DOCTYPES", "QA_DOCUMENT_STEPS", "QA_DOCUMENTS", "SP_REQUESTS", "SP_TESTS", "SP_TUBES", "SP_TUBETESTS", "PATIENTS", "DICT_TESTS", "DICT_TEST_SAMPLES", "DICT_SAMPLES_TYPES", "DICT_COLLECTORS", "DICT_TEXTS", "DICT_TESTS_V3", "DICT_TUBES_TYPES_V3", "SP_REQUEST_DOCTORS", "SP_REQUESTS_OCOM", "DOCTORS", "HOSPITALIZATIONS", "LOCATIONS", "REQUESTS", "DICT_RACKS", "DICT_STORAGE_LOCATIONS", "RACK_POSITIONS", "SAMPLE_TRACKING", "SAMPLES", "DICT_TRANSPORTATION_STORAGE", "DICT_LABORATORIES", "DICT_SITES", "DICT_DOCTORS", "DICT_LOCATIONS", "DICT_WORKSTATIONS", "DICT_CHAPTERS", "TESTS", "TESTS_COMMENT", "WS_TEST_COMMENT", "SAMPLE_AVAILABILITY", "SAMPLE_COMMENTS", "DICT_TEST_PERF_ON", "DEPARTMENT_LABORATORY", "LABORATORY_SITE", "TECHGROUP_DEPARTMENT", "WORKSTATION_TECHGROUP", "DICT_DEPARTMENTS", "DICT_TECHNICAL_GROUPS", "DICT_WORKSTATIONS_V3", "DICT_LOCATIONS_V3", "ROLE_PRIV", "ROLES", "USER_ROLE", "USERS", "PARAM_VALUES", "CONFIGS", "SAMPLE_TRANSPORT", "DICT_CATEGORIES", "DICT_PAT_CLASS", "REQUESTS_OCOM", "DICT_TEST_COMP", "PATIENTS_PCOM", "STORAGE_LOCATION_POST", "STORAGE_LOCATION_PRE", "LOCATION_DOCTORS", "USERS_HC", "DICT_BINARY_DATA", "DICT_TESTS_PERF_ON_ADD_DATA", "DICT_MEDICAL_DISCIPLINES", "ACTION_MARKS", "ALTERNATE_IDENTIFIERS", "DICT_IDENTIFIERNAME", "DICT_DEVICES", "DICT_SPECIALITIES", "JOB_PARAMETERS", "JOBS", "REVIEW", "USER_PASSWORDS", "ATR_COPIES", "ACTS", "CUSTOMER_PROFILES", "DICT_ACTS", "DICT_BILL_TESTS", "DICT_ORGANISATS", "DICT_ORGTYPES", "INVOICE_DETAILS", "INVOICES", "PAYMENT_COMMENTS", "PAYMENT_DETAILS", "PAYMENTS", "STAT_COMMENTS", "STATEMENTS", "SUBREQUESTS_HC", "SUBREQUESTS_MB", "DICT_TESTS_OCOM", "SUBREQMB_ACTIONS", "THRESHOLD_DEPT", "THRESHOLD_LAB", "SP_TESTS_DICT_TESTS", "SP_REQUESTS_DICT_DOCTORS", "SP_REQUESTS_DICT_LOCATIONS", "DICT_TESTS_PERF_ON_ADD_DATA_DICT_SAMPLES_TYPESSet", "DICT_TESTS_DICT_TESTS_PERF_ON_ADD_DATA", "USER_FUNCTIONS", "DICT_PAY_METHODS", "UNIX_REVIEW_COUNTERS"
    };

  [McpServerTool, Description("Lists all available entity sets exposed by ConfigurationModelsContext.")]
  public IReadOnlyList<string> ListEntitySets() => _entitySets;

  [McpServerTool, Description("Runs an OData query against an entity set and returns the results as JSON. Supports $filter, $select, $orderby, $top, $skip, and $expand.")]
  public async Task<string> Query(
      string serviceRootUrl,
      string entitySet,
      string? filter = null,
      string? select = null,
      string? orderBy = null,
      int? top = 50,
      int? skip = null,
      string? expand = null,
      Dictionary<string, string>? headers = null,
      CancellationToken cancellationToken = default)
  {
    var ctx = CreateContext();
    var q = ctx.For(entitySet);

    if (!string.IsNullOrWhiteSpace(filter)) q = q.Filter(filter);
    if (!string.IsNullOrWhiteSpace(select)) q = q.Select(select);
    if (!string.IsNullOrWhiteSpace(orderBy)) q = q.OrderBy(orderBy);
    if (!string.IsNullOrWhiteSpace(expand)) q = q.Expand(expand);
    if (skip.HasValue) q = q.Skip(skip.Value);
    if (top.HasValue) q = q.Top(top.Value);

    // Simple.OData.* doesn't currently take CancellationToken on all methods; this is best-effort.
    var entries = await q.FindEntriesAsync().ConfigureAwait(false);

    return JsonSerializer.Serialize(entries, JsonOptions);
  }

  [McpServerTool, Description("Fetches a single entity by key from an entity set. Provide key fields as a dictionary (propertyName -> value). Returns the entity as JSON or null if not found.")]
  public async Task<string> GetByKey(
      string serviceRootUrl,
      string entitySet,
      Dictionary<string, object> key,
      Dictionary<string, string>? headers = null,
      CancellationToken cancellationToken = default)
  {
    if (key is null || key.Count == 0) throw new ArgumentException("Key dictionary must not be empty.", nameof(key));

    var ctx = CreateContext();

    object? entry = null;
    try
    {
      entry = await ctx.For(entitySet).Key(key).FindEntryAsync().ConfigureAwait(false);
    }
    catch (WebRequestException ex) when (IsNotFound(ex))
    {
      entry = null;
    }

    return JsonSerializer.Serialize(entry, JsonOptions);
  }

  [McpServerTool, Description("Creates (POST) a new entity in an entity set. Provide the entity fields as a dictionary. Returns the created entity (or service response) as JSON.")]
  public async Task<string> Create(
      string serviceRootUrl,
      string entitySet,
      Dictionary<string, object> data,
      Dictionary<string, string>? headers = null,
      CancellationToken cancellationToken = default)
  {
    if (data is null) throw new ArgumentNullException(nameof(data));
    var ctx = CreateContext();

    var created = await ctx.For(entitySet).Set(data).InsertEntryAsync().ConfigureAwait(false);
    return JsonSerializer.Serialize(created, JsonOptions);
  }

  [McpServerTool, Description("Updates (PATCH/MERGE) an entity in an entity set. Provide the key fields and the fields to change. Returns the updated entity (or service response) as JSON.")]
  public async Task<string> Update(
      string serviceRootUrl,
      string entitySet,
      Dictionary<string, object> key,
      Dictionary<string, object> changes,
      Dictionary<string, string>? headers = null,
      CancellationToken cancellationToken = default)
  {
    if (key is null || key.Count == 0) throw new ArgumentException("Key dictionary must not be empty.", nameof(key));
    if (changes is null) throw new ArgumentNullException(nameof(changes));

    var ctx = CreateContext();

    var updated = await ctx.For(entitySet).Key(key).Set(changes).UpdateEntryAsync().ConfigureAwait(false);
    return JsonSerializer.Serialize(updated, JsonOptions);
  }

  [McpServerTool, Description("Deletes an entity from an entity set. Provide key fields as a dictionary. Returns true if deletion succeeded.")]
  public async Task<bool> Delete(
      string serviceRootUrl,
      string entitySet,
      Dictionary<string, object> key,
      Dictionary<string, string>? headers = null,
      CancellationToken cancellationToken = default)
  {
    if (key is null || key.Count == 0) throw new ArgumentException("Key dictionary must not be empty.", nameof(key));

    var ctx = CreateContext();

    await ctx.For(entitySet).Key(key).DeleteEntryAsync().ConfigureAwait(false);
    return true;
  }

  [McpServerTool, Description("Returns (best-effort) a map of entitySet -> CLR entity type name, based on the generated ConfigurationModelsContext properties.")]
  public IReadOnlyDictionary<string, string> GetEntityClrTypes()
  {
    // Uses reflection so you don't have to keep this in sync manually.
    var ctxType = typeof(ConfigurationModelsContext);

    var dict = ctxType
        .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
        .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition().FullName == "Simple.OData.Client.IBoundClient`1")
        .ToDictionary(
            p => p.Name,
            p => p.PropertyType.GetGenericArguments()[0].FullName ?? p.PropertyType.GetGenericArguments()[0].Name,
            StringComparer.OrdinalIgnoreCase);

    return dict;
  }

  

  private bool IsNotFound(WebRequestException ex)
  {
    // Simple.OData.Client wraps underlying HTTP errors. We keep this conservative.
    var msg = ex.Message ?? string.Empty;
    return msg.Contains("404", StringComparison.OrdinalIgnoreCase) ||
           msg.Contains("NotFound", StringComparison.OrdinalIgnoreCase);
  }

  private readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
  {
    WriteIndented = true
  };
}
