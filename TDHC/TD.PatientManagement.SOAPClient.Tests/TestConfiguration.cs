using PatientManagementService;

namespace TD.PatientManagement.SOAPClient.Tests;

/// <summary>
/// Configuration centralisée pour les tests d'intégration
/// Modifiez ces valeurs selon votre environnement
/// </summary>
public static class TestConfiguration
{
  /// <summary>
  /// Endpoint par défaut à utiliser pour les tests
  /// </summary>
  public static readonly PatientManagementServiceClient.EndpointConfiguration DefaultEndpoint =
      PatientManagementServiceClient.EndpointConfiguration.WSHttpBinding_IPatientManagementService;

  /// <summary>
  /// URL personnalisée (optionnelle) - laissez null pour utiliser l'URL par défaut
  /// </summary>
  public static readonly string? CustomEndpointUrl = null;
  // Exemple: "https://votre-serveur.exemple.com/Service.svc";

  /// <summary>
  /// Site ID par défaut pour les tests
  /// </summary>
  public const int DefaultSiteId = 1;

  /// <summary>
  /// Site Code par défaut pour les tests
  /// </summary>
  public const string DefaultSiteCode = "SITE01";

  /// <summary>
  /// ID de patient valide pour les tests (à adapter selon vos données)
  /// </summary>
  public const int TestPatientId = 12345;

  /// <summary>
  /// Patient Number pour les tests
  /// </summary>
  public const string TestPatientNumber = "PAT001";

  /// <summary>
  /// ID de visite (hospitalization) patient valide pour les tests (à adapter selon vos données)
  /// </summary>
  public const int TestHospitalizationId = 67890;

  /// <summary>
  /// Hospitalization Number pour les tests
  /// </summary>
  public const string TestHospitalizationNumber = "HOSP001";

  /// <summary>
  /// Numéro de demande valide pour les tests
  /// </summary>
  public const string TestRequestNumber = "REQ-001";

  /// <summary>
  /// Nom (Name) pour les recherches de test
  /// </summary>
  public const string TestName = "DOE";

  /// <summary>
  /// Prénom pour les recherches de test
  /// </summary>
  public const string TestFirstName = "JOHN";

  /// <summary>
  /// Timeout par défaut pour les opérations (en secondes)
  /// </summary>
  public const int DefaultTimeoutSeconds = 30;

  /// <summary>
  /// Active ou désactive les tests qui modifient les données
  /// </summary>
  public const bool EnableWriteTests = false;

  /// <summary>
  /// Active ou désactive les logs détaillés
  /// </summary>
  public const bool EnableVerboseLogging = true;

  /// <summary>
  /// Données de test pour la création d'un nouveau patient
  /// </summary>
  public static class TestPatientData
  {
    public const string Name = "TEST";
    public const string FirstName = "INTEGRATION";
    public const string Street1 = "123 Test Street";
    public const string City = "Test City";
    public const string ZipCode = "12345";
    public const string Country = "BE";

    public static DateTime BirthDate => new DateTime(1990, 1, 1);
    public static Sex Sex => Sex.Male;
  }

  /// <summary>
  /// Données de test pour la création d'une nouvelle visite
  /// </summary>
  public static class TestVisitData
  {
    public const int LocationId = 1;
    public const string LocationCode = "LOC-001";

    public static DateTime AdmissionDate => DateTime.Now;
  }

  /// <summary>
  /// Crée un client configuré avec les paramètres par défaut
  /// </summary>
  public static PatientManagementServiceClient CreateConfiguredClient()
  {
    if (!string.IsNullOrEmpty(CustomEndpointUrl))
    {
      return new PatientManagementServiceClient(DefaultEndpoint, CustomEndpointUrl);
    }

    return new PatientManagementServiceClient(DefaultEndpoint);
  }

  /// <summary>
  /// Configure le timeout du client
  /// </summary>
  public static void ConfigureTimeout(PatientManagementServiceClient client, int timeoutSeconds)
  {
    var timeout = TimeSpan.FromSeconds(timeoutSeconds);
    client.Endpoint.Binding.SendTimeout = timeout;
    client.Endpoint.Binding.ReceiveTimeout = timeout;
  }
}
