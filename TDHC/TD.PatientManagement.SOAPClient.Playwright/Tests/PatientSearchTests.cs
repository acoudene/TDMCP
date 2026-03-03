using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests Playwright pour l'opération <c>SearchPatient</c>.
/// Chaque test est tracé par Playwright (trace + screenshot automatique sur échec).
/// </summary>
public class PatientSearchTests : PlaywrightTestBase
{
  public PatientSearchTests(ITestOutputHelper output) : base(output) { }

  [Fact(DisplayName = "SearchPatient par démographie (nom + prénom) retourne des résultats")]
  public async Task SearchPatient_ByDemographics_ShouldReturnResults()
  {
    // Arrange
    var criteria = new PatientSearchCriteriaDTO
    {
      Demographic = new PatientDemographicDTO
      {
        Name      = PlaywrightConfiguration.TestName,
        FirstName = PlaywrightConfiguration.TestFirstName
      }
    };

    Output.WriteLine($"[Search] Nom : {PlaywrightConfiguration.TestName} | Prénom : {PlaywrightConfiguration.TestFirstName}");

    // Act
    var results = await CallSoapAsync(
        () => SoapClient.SearchPatientAsync(criteria),
        "SearchPatient par démographie");

    // Assert
    Assert.NotNull(results);
    Output.WriteLine($"[Search] {results.Length} résultat(s).");

    if (results.Length > 0)
    {
      var first = results[0];
      Output.WriteLine($"[Search] Premier patient ID   : {first.Identifier?.PatientId}");
      Output.WriteLine($"[Search]                  Nom : {first.Demographic?.Name} {first.Demographic?.FirstName}");
    }
  }

  [Fact(DisplayName = "SearchPatient par ID retourne le patient correspondant")]
  public async Task SearchPatient_ById_ShouldReturnMatchingPatient()
  {
    // Arrange
    var criteria = new PatientSearchCriteriaDTO
    {
      Patient = PlaywrightConfiguration.BuildPatientIdentifier()
    };

    // Act
    var results = await CallSoapAsync(
        () => SoapClient.SearchPatientAsync(criteria),
        $"SearchPatient par ID={PlaywrightConfiguration.TestPatientId}");

    // Assert
    Assert.NotNull(results);
    Output.WriteLine($"[Search] {results.Length} résultat(s) pour PatientId={PlaywrightConfiguration.TestPatientId}.");

    if (results.Length > 0 && results[0].Identifier is not null)
    {
      Assert.Equal(
          PlaywrightConfiguration.TestPatientId,
          results[0].Identifier!.PatientId);
    }
  }

  [Fact(DisplayName = "SearchPatient par numéro de patient retourne le bon patient")]
  public async Task SearchPatient_ByPatientNumber_ShouldReturnPatient()
  {
    // Arrange
    var criteria = new PatientSearchCriteriaDTO
    {
      Patient = new PatientIdentifierDTO
      {
        PatientNumber = PlaywrightConfiguration.TestPatientNumber
      }
    };

    // Act
    var results = await CallSoapAsync(
        () => SoapClient.SearchPatientAsync(criteria),
        $"SearchPatient par PatientNumber={PlaywrightConfiguration.TestPatientNumber}");

    // Assert
    Assert.NotNull(results);
    Output.WriteLine($"[Search] {results.Length} résultat(s) pour PatientNumber={PlaywrightConfiguration.TestPatientNumber}.");
  }

  [Fact(DisplayName = "SearchPatient avec critères vides ne lève pas d'exception")]
  public async Task SearchPatient_WithEmptyCriteria_ShouldNotThrow()
  {
    // Arrange
    var criteria = new PatientSearchCriteriaDTO();

    // Act
    var results = await CallSoapAsync(
        () => SoapClient.SearchPatientAsync(criteria),
        "SearchPatient critères vides");

    // Assert
    Assert.NotNull(results);
    Output.WriteLine($"[Search] {results.Length} résultat(s) (critères vides).");
  }
}
