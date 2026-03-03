using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests Playwright pour les opérations d'écriture SOAP :
/// CreatePatient, UpdatePatient, CreatePatientVisit, UpdatePatientVisit, DeletePatientVisit.
/// <para>
/// ⚠️ ATTENTION : Ces tests modifient réellement les données en base.
/// </para>
/// <para>
/// Ils sont marqués <c>[Fact(Skip = "...")]</c> pour ne PAS s'exécuter automatiquement.
/// Pour les activer : retirez le paramètre <c>Skip</c> ou surchargez la valeur
/// via la variable d'environnement <c>ENABLE_WRITE_TESTS=true</c>
/// et adaptez le code en conséquence.
/// </para>
/// <para>
/// Commande manuelle (après suppression du Skip) :
/// <code>dotnet test --filter "FullyQualifiedName~PatientWriteTests"</code>
/// </para>
/// </summary>
public class PatientWriteTests : PlaywrightTestBase
{
  private const string SkipReason =
      "Tests d'écriture : modifient des données réelles. " +
      "Retirez le paramètre Skip pour les exécuter manuellement en environnement dédié.";

  public PatientWriteTests(ITestOutputHelper output) : base(output) { }

  // ── Patients ──────────────────────────────────────────────────────────────────

  [Fact(DisplayName = "CreatePatient retourne un identifiant valide", Skip = SkipReason)]
  public async Task CreatePatient_WithValidData_ShouldReturnIdentifier()
  {
    // Arrange
    var patient = new PatientDTO
    {
      Identifier  = new PatientIdentifierDTO(),
      Demographic = new PatientDemographicDTO
      {
        Name      = PlaywrightConfiguration.TestPatientData.Name,
        FirstName = PlaywrightConfiguration.TestPatientData.FirstName,
        BirthDate = PlaywrightConfiguration.TestPatientData.BirthDate,
        Sex       = PlaywrightConfiguration.TestPatientData.Sex,
      },
      Address = new PatientAddressDTO
      {
        Address1   = PlaywrightConfiguration.TestPatientData.Street1,
        City       = PlaywrightConfiguration.TestPatientData.City,
        PostalCode = PlaywrightConfiguration.TestPatientData.ZipCode,
        Country    = PlaywrightConfiguration.TestPatientData.Country,
      }
    };

    // Act
    var result = await CallSoapAsync(
        () => SoapClient.CreatePatientAsync(patient),
        "CreatePatient");

    // Assert
    Assert.NotNull(result);
    Assert.NotNull(result.PatientIdentifier);
    Output.WriteLine($"[Create] Patient créé → PatientId={result.PatientIdentifier!.PatientId}");
  }

  [Fact(DisplayName = "UpdatePatient met à jour les données démographiques sans exception", Skip = SkipReason)]
  public async Task UpdatePatient_WithValidData_ShouldSucceed()
  {
    // Arrange
    var patient = new PatientDTO
    {
      Identifier  = PlaywrightConfiguration.BuildPatientIdentifier(),
      Demographic = new PatientDemographicDTO
      {
        Name      = "PLAYWRIGHT",
        FirstName = "UPDATED",
      }
    };

    // Act
    await CallSoapAsync(
        async () => { await SoapClient.UpdatePatientAsync(patient); return true; },
        $"UpdatePatient(PatientId={PlaywrightConfiguration.TestPatientId})");

    Output.WriteLine($"[Update] Patient {PlaywrightConfiguration.TestPatientId} mis à jour.");
  }

  // ── Visites ───────────────────────────────────────────────────────────────────

  [Fact(DisplayName = "CreatePatientVisit retourne un HospitalizationId valide", Skip = SkipReason)]
  public async Task CreatePatientVisit_WithValidData_ShouldReturnHospitalizationId()
  {
    // Arrange
    var visit = new PatientVisitDTO
    {
      PatientIdentifier = PlaywrightConfiguration.BuildPatientIdentifier(),
      AdmissionDate     = DateTime.Now,
      Location          = PlaywrightConfiguration.BuildLocationIdentifier(),
    };

    // Act
    var visitId = await CallSoapAsync(
        () => SoapClient.CreatePatientVisitAsync(visit),
        "CreatePatientVisit");

    // Assert
    Assert.NotNull(visitId);
    Assert.True(visitId.HospitalizationId > 0,
        $"L'identifiant d'hospitalisation est invalide : {visitId.HospitalizationId}");
    Output.WriteLine($"[CreateVisit] HospitalizationId={visitId.HospitalizationId}");
  }

  [Fact(DisplayName = "UpdatePatientVisit met à jour la visite sans exception", Skip = SkipReason)]
  public async Task UpdatePatientVisit_WithValidData_ShouldSucceed()
  {
    // Arrange
    var visit = new PatientVisitDTO
    {
      Identifier    = PlaywrightConfiguration.BuildVisitIdentifier(),
      AdmissionDate = DateTime.Now.AddDays(-1),
    };

    // Act
    await CallSoapAsync(
        async () => { await SoapClient.UpdatePatientVisitAsync(visit); return true; },
        $"UpdatePatientVisit(HospitalizationId={PlaywrightConfiguration.TestHospitalizationId})");

    Output.WriteLine($"[UpdateVisit] Visite {PlaywrightConfiguration.TestHospitalizationId} mise à jour.");
  }

  [Fact(DisplayName = "DeletePatientVisit supprime la visite sans exception", Skip = SkipReason)]
  public async Task DeletePatientVisit_WithValidId_ShouldSucceed()
  {
    // Arrange
    var visitId = PlaywrightConfiguration.BuildVisitIdentifier();

    // Act
    await CallSoapAsync(
        async () => { await SoapClient.DeletePatientVisitAsync(visitId); return true; },
        $"DeletePatientVisit(HospitalizationId={PlaywrightConfiguration.TestHospitalizationId})");

    Output.WriteLine($"[DeleteVisit] Visite {PlaywrightConfiguration.TestHospitalizationId} supprimée.");
  }
}
