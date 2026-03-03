using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests Playwright pour l'opération <c>SearchRequest</c>.
/// </summary>
public class RequestSearchTests : PlaywrightTestBase
{
  public RequestSearchTests(ITestOutputHelper output) : base(output) { }

  [Fact(DisplayName = "SearchRequest par numéro de demande retourne des résultats")]
  public async Task SearchRequest_ByRequestNumber_ShouldReturnRequests()
  {
    // Arrange
    var criteria = new RequestSearchCriteriaDTO
    {
      Request = new RequestIdentifierDTO
      {
        RequestNumber = PlaywrightConfiguration.TestRequestNumber
      }
    };

    // Act
    var requests = await CallSoapAsync(
        () => SoapClient.SearchRequestAsync(criteria),
        $"SearchRequest(RequestNumber={PlaywrightConfiguration.TestRequestNumber})");

    // Assert
    Assert.NotNull(requests);
    Output.WriteLine($"[Request] {requests.Length} demande(s) pour RequestNumber={PlaywrightConfiguration.TestRequestNumber}.");

    if (requests.Length > 0)
      Output.WriteLine($"[Request] Première demande : {requests[0].Identifier?.RequestNumber}");
  }

  [Fact(DisplayName = "SearchRequest par ID patient retourne les demandes associées")]
  public async Task SearchRequest_ByPatientId_ShouldReturnRequests()
  {
    // Arrange
    var criteria = new RequestSearchCriteriaDTO
    {
      Patient = PlaywrightConfiguration.BuildPatientIdentifier()
    };

    // Act
    var requests = await CallSoapAsync(
        () => SoapClient.SearchRequestAsync(criteria),
        $"SearchRequest(PatientId={PlaywrightConfiguration.TestPatientId})");

    // Assert
    Assert.NotNull(requests);
    Output.WriteLine($"[Request] {requests.Length} demande(s) pour PatientId={PlaywrightConfiguration.TestPatientId}.");
  }

  [Fact(DisplayName = "SearchRequest avec critères vides ne lève pas d'exception")]
  public async Task SearchRequest_WithEmptyCriteria_ShouldNotThrow()
  {
    // Arrange
    var criteria = new RequestSearchCriteriaDTO();

    // Act
    var requests = await CallSoapAsync(
        () => SoapClient.SearchRequestAsync(criteria),
        "SearchRequest critères vides");

    // Assert
    Assert.NotNull(requests);
    Output.WriteLine($"[Request] {requests.Length} demande(s) (critères vides).");
  }
}
