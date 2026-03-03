using PatientManagementService;

namespace TD.PatientManagement.SOAPClient.Playwright;

/// <summary>
/// Configuration centralisée pour les tests Playwright d'intégration SOAP.
/// Les valeurs peuvent être surchargées via des variables d'environnement
/// pour s'adapter aux différents environnements (dev, recette, prod).
/// </summary>
public static class PlaywrightConfiguration
{
  // ── Endpoint WCF ──────────────────────────────────────────────────────────────

  /// <summary>
  /// Endpoint WCF utilisé par les tests (nom généré par le Connected Service).
  /// </summary>
  public static readonly PatientManagementServiceClient.EndpointConfiguration DefaultEndpoint =
      PatientManagementServiceClient.EndpointConfiguration.WSHttpBinding_IPatientManagementService;

  /// <summary>
  /// URL personnalisée pour surcharger l'endpoint généré.
  /// Positionnée via la variable d'environnement SOAP_ENDPOINT_URL,
  /// ou null pour utiliser l'URL du app.config générée.
  /// Exemple : "https://tdhc-app-dev-2.technidata.net:38431/TDHC9XACE/PatientManagement/PatientManagementService.svc"
  /// </summary>
  public static readonly string? CustomEndpointUrl =
      Environment.GetEnvironmentVariable("SOAP_ENDPOINT_URL");

  // ── Données de référence ───────────────────────────────────────────────────────

  /// <summary>Identifiant du patient utilisé pour les tests de lecture.</summary>
  public static readonly int TestPatientId =
      int.TryParse(Environment.GetEnvironmentVariable("TEST_PATIENT_ID"), out var pid)
          ? pid
          : 12345;

  /// <summary>Numéro du patient de référence.</summary>
  public const string TestPatientNumber = "PAT001";

  /// <summary>Identifiant d'hospitalisation utilisé pour les tests de visite.</summary>
  public static readonly int TestHospitalizationId =
      int.TryParse(Environment.GetEnvironmentVariable("TEST_HOSPITALIZATION_ID"), out var hid)
          ? hid
          : 67890;

  /// <summary>Numéro d'hospitalisation de référence.</summary>
  public const string TestHospitalizationNumber = "HOSP001";

  /// <summary>Numéro de demande utilisé pour les tests SearchRequest.</summary>
  public const string TestRequestNumber = "REQ-001";

  /// <summary>Nom utilisé pour les recherches démographiques.</summary>
  public const string TestName = "DOE";

  /// <summary>Prénom utilisé pour les recherches démographiques.</summary>
  public const string TestFirstName = "JOHN";

  /// <summary>Timeout global (secondes) appliqué au client WCF.</summary>
  public const int DefaultTimeoutSeconds = 30;

  // ── Paramètres Playwright ─────────────────────────────────────────────────────

  /// <summary>
  /// Mode headless du navigateur Playwright.
  /// Positionnez PLAYWRIGHT_HEADLESS=false pour ouvrir un vrai navigateur (debug).
  /// </summary>
  public static bool Headless =>
      !string.Equals(
          Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS"),
          "false",
          StringComparison.OrdinalIgnoreCase);

  /// <summary>Répertoire de sortie des traces Playwright (.zip).</summary>
  public static string TracesDirectory =>
      Environment.GetEnvironmentVariable("PLAYWRIGHT_TRACES_DIR")
      ?? Path.Combine(AppContext.BaseDirectory, "playwright-traces");

  /// <summary>Répertoire de sortie des screenshots en cas d'échec.</summary>
  public static string ScreenshotsDirectory =>
      Environment.GetEnvironmentVariable("PLAYWRIGHT_SCREENSHOTS_DIR")
      ?? Path.Combine(AppContext.BaseDirectory, "playwright-screenshots");

  // ── Données d'écriture ────────────────────────────────────────────────────────

  /// <summary>Données utilisées pour la création d'un patient de test.</summary>
  public static class TestPatientData
  {
    public const  string   Name      = "TEST";
    public const  string   FirstName = "PLAYWRIGHT";
    public static DateTime BirthDate => new(1990, 6, 15);
    public static Sex      Sex       => Sex.Female;
    public const  string   Street1   = "42 Rue du Test";
    public const  string   City      = "Testville";
    public const  string   ZipCode   = "75000";
    public const  string   Country   = "FR";
  }

  /// <summary>Données utilisées pour la création d'une visite de test.</summary>
  public static class TestVisitData
  {
    public const int    LocationId   = 1;
    public const string LocationCode = "LOC-001";
  }

  // ── Factory ───────────────────────────────────────────────────────────────────

  /// <summary>
  /// Crée et configure un client WCF <see cref="PatientManagementServiceClient"/>
  /// prêt à l'emploi avec le timeout défini dans <see cref="DefaultTimeoutSeconds"/>.
  /// </summary>
  public static PatientManagementServiceClient CreateConfiguredClient()
  {
    var client = string.IsNullOrEmpty(CustomEndpointUrl)
        ? new PatientManagementServiceClient(DefaultEndpoint)
        : new PatientManagementServiceClient(DefaultEndpoint, CustomEndpointUrl);

    var timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds);
    client.Endpoint.Binding.SendTimeout    = timeout;
    client.Endpoint.Binding.ReceiveTimeout = timeout;

    return client;
  }

  // ── Helpers de fabrication de DTOs ────────────────────────────────────────────

  public static PatientIdentifierDTO BuildPatientIdentifier(int? id = null) =>
      new() { PatientId = id ?? TestPatientId };

  public static PatientVisitIdentifierDTO BuildVisitIdentifier(int? id = null) =>
      new() { HospitalizationId = id ?? TestHospitalizationId };

  public static LocationIdentifierDTO BuildLocationIdentifier() =>
      new() { LocId = TestVisitData.LocationId, Code = TestVisitData.LocationCode };
}
