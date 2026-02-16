using Simple.OData.Client;
using System.Reflection;
using TD.Technical.ODataClient.ConfigModel;
using Xunit.Sdk;

namespace TD.Technical.ODataClient.Tests;

public class GeneratedODataClientTests
{
  private static Uri ServiceRootUrl => new Uri("https://tdhc-app-dev-2.technidata.net:38431/TDHC9XACE/Technical/TDDatabaseService.svc");

  private static ConfigurationModelsContext CreateContext()
  {
    var url = ServiceRootUrl;    
    return new ConfigurationModelsContext(url, options =>
    {
      // If you need auth headers, inject here (token, basic, etc.).
      // options.ConfigureRequest = req => req.Headers.Add("Authorization", "Bearer ...");

      // Helpful for some WCF Data Services v3 servers:
      options.BeforeRequestAsync = async req =>
          {
            req.Headers.TryAddWithoutValidation("DataServiceVersion", "3.0");
            req.Headers.TryAddWithoutValidation("MaxDataServiceVersion", "3.0");
            await Task.CompletedTask;
          };
    });
  }

  [Fact]
  public void Context_can_be_instantiated_without_url_validation()
  {
    // This verifies the constructor doesn't do network IO by itself.
    var ctx = new ConfigurationModelsContext(new Uri("https://tdhc-app-dev-2.technidata.net:38431/TDHC9XACE/Technical/TDDatabaseService.svc"));
    Assert.NotNull(ctx);
    Assert.NotNull(ctx.Client);
  }

  [Fact]
  public void EntitySet_properties_match_expected_property_list()
  {
    // Pure unit test: checks that all generated IBoundClient<> properties exist.
    var expected = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(ExpectedMapJson)!;

    var entitySetProps = typeof(ConfigurationModelsContext)
        .GetProperties(BindingFlags.Instance | BindingFlags.Public)
        .Where(p => p.PropertyType.IsGenericType && p.PropertyType.GetGenericTypeDefinition() == typeof(IBoundClient<>))
        .OrderBy(p => p.Name)
        .ToList();

    Assert.NotEmpty(entitySetProps);
    Assert.Equal(expected.Count, entitySetProps.Count);

    var actualNames = entitySetProps.Select(p => p.Name).OrderBy(x => x).ToArray();
    var expectedNames = expected.Keys.OrderBy(x => x).ToArray();
    Assert.Equal(expectedNames, actualNames);
  }

  [Fact]
  public async Task Live_smoke_query_each_entity_set_top1()
  {
    // Live integration test. Opt-in only.    
    var ctx = CreateContext();

    var expected = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(ExpectedMapJson)!;

    foreach (var kv in expected.OrderBy(kv => kv.Key))
    {
      var propName = kv.Key;
      var entitySetName = kv.Value;

      try
      {
        // Prefer dynamic dictionary access so we don't depend on key definitions.
        var result = await ctx.For(entitySetName).Top(1).FindEntriesAsync();
        Assert.NotNull(result);
      }
      catch (WebRequestException ex)
      {
        throw new XunitException(
            $"EntitySet '{entitySetName}' (property '{propName}') failed. HTTP status: {ex.Code}. Message: {ex.Message}");
      }
    }
  }

  private const string ExpectedMapJson = @"
{
  ""ACTION_MARKS"": ""ACTION_MARKS"",
  ""ACTS"": ""ACTS"",
  ""ALERT_DISTRIBUTION"": ""ALERT_DISTRIBUTION"",
  ""ALERT_EVENTS"": ""ALERT_EVENTS"",
  ""ALTERNATE_IDENTIFIERS"": ""ALTERNATE_IDENTIFIERS"",
  ""ATR_COPIES"": ""ATR_COPIES"",
  ""BINARY_DATA"": ""BINARY_DATA"",
  ""BINARY_PARAMETERS"": ""BINARY_PARAMETERS"",
  ""CONFIGS"": ""CONFIGS"",
  ""COUNTERS_VALUES"": ""COUNTERS_VALUES"",
  ""CUSTOMER_PROFILES"": ""CUSTOMER_PROFILES"",
  ""DEFAULT_FUNCTIONS"": ""DEFAULT_FUNCTIONS"",
  ""DEFFUNCTIONS_PRESENTATION"": ""DEFFUNCTIONS_PRESENTATION"",
  ""DEFFUNCTIONS_SECTIONS"": ""DEFFUNCTIONS_SECTIONS"",
  ""DEPARTMENT_LABORATORY"": ""DEPARTMENT_LABORATORY"",
  ""DICT_ACTS"": ""DICT_ACTS"",
  ""DICT_ALERTS"": ""DICT_ALERTS"",
  ""DICT_BILL_TESTS"": ""DICT_BILL_TESTS"",
  ""DICT_BINARY_DATA"": ""DICT_BINARY_DATA"",
  ""DICT_CATEGORIES"": ""DICT_CATEGORIES"",
  ""DICT_CHAPTERS"": ""DICT_CHAPTERS"",
  ""DICT_COLLECTORS"": ""DICT_COLLECTORS"",
  ""DICT_COUNTERS"": ""DICT_COUNTERS"",
  ""DICT_COUNTER_PROPERTIES"": ""DICT_COUNTER_PROPERTIES"",
  ""DICT_COUNTER_THRESHOLDS"": ""DICT_COUNTER_THRESHOLDS"",
  ""DICT_DEPARTMENTS"": ""DICT_DEPARTMENTS"",
  ""DICT_DEVICES"": ""DICT_DEVICES"",
  ""DICT_DEVICE_PARAMETERS"": ""DICT_DEVICE_PARAMETERS"",
  ""DICT_DISTRIBUTION_CONTACTS"": ""DICT_DISTRIBUTION_CONTACTS"",
  ""DICT_DISTRIBUTION_LIST"": ""DICT_DISTRIBUTION_LIST"",
  ""DICT_DISTRIBUTION_USERS"": ""DICT_DISTRIBUTION_USERS"",
  ""DICT_DOCTORS"": ""DICT_DOCTORS"",
  ""DICT_IDENTIFIERNAME"": ""DICT_IDENTIFIERNAME"",
  ""DICT_LABORATORIES"": ""DICT_LABORATORIES"",
  ""DICT_LOCATIONS"": ""DICT_LOCATIONS"",
  ""DICT_LOCATIONS_V3"": ""DICT_LOCATIONS_V3"",
  ""DICT_MEDICAL_DISCIPLINES"": ""DICT_MEDICAL_DISCIPLINES"",
  ""DICT_ORGANISATS"": ""DICT_ORGANISATS"",
  ""DICT_ORGTYPES"": ""DICT_ORGTYPES"",
  ""DICT_PAT_CLASS"": ""DICT_PAT_CLASS"",
  ""DICT_PAY_METHODS"": ""DICT_PAY_METHODS"",
  ""DICT_QA_DOCTYPES"": ""DICT_QA_DOCTYPES"",
  ""DICT_RACKS"": ""DICT_RACKS"",
  ""DICT_SAMPLES_TYPES"": ""DICT_SAMPLES_TYPES"",
  ""DICT_SITES"": ""DICT_SITES"",
  ""DICT_SPECIALITIES"": ""DICT_SPECIALITIES"",
  ""DICT_STORAGE_LOCATIONS"": ""DICT_STORAGE_LOCATIONS"",
  ""DICT_TECHNICAL_GROUPS"": ""DICT_TECHNICAL_GROUPS"",
  ""DICT_TESTS"": ""DICT_TESTS"",
  ""DICT_TESTS_DICT_TESTS_PERF_ON_ADD_DATA"": ""DICT_TESTS_DICT_TESTS_PERF_ON_ADD_DATA"",
  ""DICT_TESTS_OCOM"": ""DICT_TESTS_OCOM"",
  ""DICT_TESTS_PERF_ON_ADD_DATA"": ""DICT_TESTS_PERF_ON_ADD_DATA"",
  ""DICT_TESTS_PERF_ON_ADD_DATA_DICT_SAMPLES_TYPESSet"": ""DICT_TESTS_PERF_ON_ADD_DATA_DICT_SAMPLES_TYPESSet"",
  ""DICT_TESTS_V3"": ""DICT_TESTS_V3"",
  ""DICT_TEST_COMP"": ""DICT_TEST_COMP"",
  ""DICT_TEST_PERF_ON"": ""DICT_TEST_PERF_ON"",
  ""DICT_TEST_SAMPLES"": ""DICT_TEST_SAMPLES"",
  ""DICT_TEXTS"": ""DICT_TEXTS"",
  ""DICT_TRANSPORTATION_STORAGE"": ""DICT_TRANSPORTATION_STORAGE"",
  ""DICT_TUBES_TYPES_V3"": ""DICT_TUBES_TYPES_V3"",
  ""DICT_TYPICAL_WEEKS"": ""DICT_TYPICAL_WEEKS"",
  ""DICT_TYPICAL_WEEKS_DPTS"": ""DICT_TYPICAL_WEEKS_DPTS"",
  ""DICT_TYPICAL_WEEKS_LABOS"": ""DICT_TYPICAL_WEEKS_LABOS"",
  ""DICT_WEEK_DAYS"": ""DICT_WEEK_DAYS"",
  ""DICT_WORKSTATIONS"": ""DICT_WORKSTATIONS"",
  ""DICT_WORKSTATIONS_V3"": ""DICT_WORKSTATIONS_V3"",
  ""DOCTORS"": ""DOCTORS"",
  ""EVENTLISTS"": ""EVENTLISTS"",
  ""HOSPITALIZATIONS"": ""HOSPITALIZATIONS"",
  ""INDICATOR_TYPES"": ""INDICATOR_TYPES"",
  ""INDIC_SETTINGS"": ""INDIC_SETTINGS"",
  ""INVOICES"": ""INVOICES"",
  ""INVOICE_DETAILS"": ""INVOICE_DETAILS"",
  ""JOBS"": ""JOBS"",
  ""JOB_PARAMETERS"": ""JOB_PARAMETERS"",
  ""LABORATORY_SITE"": ""LABORATORY_SITE"",
  ""LOCATIONS"": ""LOCATIONS"",
  ""LOCATION_DOCTORS"": ""LOCATION_DOCTORS"",
  ""PARAM_VALUES"": ""PARAM_VALUES"",
  ""PARAM_VALUES_DEFAULT"": ""PARAM_VALUES_DEFAULT"",
  ""PARAM_VALUES_SECTION"": ""PARAM_VALUES_SECTION"",
  ""PARAM_VALUES_VIEWS"": ""PARAM_VALUES_VIEWS"",
  ""PATIENTS"": ""PATIENTS"",
  ""PATIENTS_PCOM"": ""PATIENTS_PCOM"",
  ""PAYMENTS"": ""PAYMENTS"",
  ""PAYMENT_COMMENTS"": ""PAYMENT_COMMENTS"",
  ""PAYMENT_DETAILS"": ""PAYMENT_DETAILS"",
  ""PROBEREQUESTSWITHCRITICALTESTS"": ""PROBEREQUESTSWITHCRITICALTESTS"",
  ""PROBEREQUESTSWITHPENDINGMBREQS"": ""PROBEREQUESTSWITHPENDINGMBREQS"",
  ""PROBEREQUESTSWITHPENDINGTESTS"": ""PROBEREQUESTSWITHPENDINGTESTS"",
  ""PROBEREQUESTSWITHSAMPTOBERECS"": ""PROBEREQUESTSWITHSAMPTOBERECS"",
  ""PROBEREQUESTSWITHTOBEPHONEDMBS"": ""PROBEREQUESTSWITHTOBEPHONEDMBS"",
  ""QA_DOCUMENTS"": ""QA_DOCUMENTS"",
  ""QA_DOCUMENT_STEPS"": ""QA_DOCUMENT_STEPS"",
  ""RACK_POSITIONS"": ""RACK_POSITIONS"",
  ""REQUESTS"": ""REQUESTS"",
  ""REQUESTS_OCOM"": ""REQUESTS_OCOM"",
  ""REQUEST_CLINICALNOTES"": ""REQUEST_CLINICALNOTES"",
  ""RESPONSIBLES"": ""RESPONSIBLES"",
  ""REVIEW"": ""REVIEW"",
  ""ROLES"": ""ROLES"",
  ""ROLE_PRIV"": ""ROLE_PRIV"",
  ""SAMPLES"": ""SAMPLES"",
  ""SAMPLES_CHRONOID"": ""SAMPLES_CHRONOID"",
  ""SAMPLE_AVAILABILITY"": ""SAMPLE_AVAILABILITY"",
  ""SAMPLE_COMMENTS"": ""SAMPLE_COMMENTS"",
  ""SAMPLE_TRACKING"": ""SAMPLE_TRACKING"",
  ""SAMPLE_TRANSPORT"": ""SAMPLE_TRANSPORT"",
  ""SHIFT_DEFINITION"": ""SHIFT_DEFINITION"",
  ""SIMPLE_NON_CONFORMITY"": ""SIMPLE_NON_CONFORMITY"",
  ""SP_REQUESTS"": ""SP_REQUESTS"",
  ""SP_REQUESTS_DICT_DOCTORS"": ""SP_REQUESTS_DICT_DOCTORS"",
  ""SP_REQUESTS_DICT_LOCATIONS"": ""SP_REQUESTS_DICT_LOCATIONS"",
  ""SP_REQUESTS_NC"": ""SP_REQUESTS_NC"",
  ""SP_REQUESTS_OCOM"": ""SP_REQUESTS_OCOM"",
  ""SP_REQUEST_DOCTORS"": ""SP_REQUEST_DOCTORS"",
  ""SP_TESTS"": ""SP_TESTS"",
  ""SP_TESTS_DICT_TESTS"": ""SP_TESTS_DICT_TESTS"",
  ""SP_TUBES"": ""SP_TUBES"",
  ""SP_TUBES_NC"": ""SP_TUBES_NC"",
  ""SP_TUBETESTS"": ""SP_TUBETESTS"",
  ""STATEMENTS"": ""STATEMENTS"",
  ""STAT_COMMENTS"": ""STAT_COMMENTS"",
  ""STORAGE_LOCATION_POST"": ""STORAGE_LOCATION_POST"",
  ""STORAGE_LOCATION_PRE"": ""STORAGE_LOCATION_PRE"",
  ""SUBREQMB_ACTIONS"": ""SUBREQMB_ACTIONS"",
  ""SUBREQUESTS_HC"": ""SUBREQUESTS_HC"",
  ""SUBREQUESTS_MB"": ""SUBREQUESTS_MB"",
  ""SYSCONSTRAINT"": ""SYSCONSTRAINT"",
  ""SYSSESSIONID"": ""SYSSESSIONID"",
  ""TECHGROUP_DEPARTMENT"": ""TECHGROUP_DEPARTMENT"",
  ""TESTS"": ""TESTS"",
  ""TESTS_COMMENT"": ""TESTS_COMMENT"",
  ""THRESHOLD_DEPT"": ""THRESHOLD_DEPT"",
  ""THRESHOLD_LAB"": ""THRESHOLD_LAB"",
  ""UNIX_MESSAGES"": ""UNIX_MESSAGES"",
  ""UNIX_REVIEW_COUNTERS"": ""UNIX_REVIEW_COUNTERS"",
  ""USERS"": ""USERS"",
  ""USERS_HC"": ""USERS_HC"",
  ""USER_DB_SESSIONS"": ""USER_DB_SESSIONS"",
  ""USER_FUNCTIONS"": ""USER_FUNCTIONS"",
  ""USER_INDICATORS"": ""USER_INDICATORS"",
  ""USER_PASSWORDS"": ""USER_PASSWORDS"",
  ""USER_ROLE"": ""USER_ROLE"",
  ""WORKSTATION_TECHGROUP"": ""WORKSTATION_TECHGROUP"",
  ""WS_TEST_COMMENT"": ""WS_TEST_COMMENT""
}
";
}
