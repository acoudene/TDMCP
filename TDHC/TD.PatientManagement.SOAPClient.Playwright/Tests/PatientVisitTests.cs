using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests Playwright pour la gestion des visites patient :
/// GetPatientVisit, SearchPatientVisitByIdentifier, GetInsurances, GetGuarantor.
/// </summary>
public class PatientVisitTests : PlaywrightTestBase
{
  public PatientVisitTests(ITestOutputHelper output) : base(output) { }

  [Fact(DisplayName = "GetPatientVisit retourne les visites du patient")]
  public async Task GetPatientVisit_ByPatientId_ShouldReturnVisits()
  {
    // Arrange
    var identifier = PlaywrightConfiguration.BuildPatientIdentifier();

    // Act
    var visits = await CallSoapAsync(
        () => SoapClient.GetPatientVisitAsync(identifier),
        $"GetPatientVisit(PatientId={PlaywrightConfiguration.TestPatientId})");

    // Assert
    Assert.NotNull(visits);
    Output.WriteLine($"[Visits] {visits.Length} visite(s) trouvée(s).");

    if (visits.Length > 0)
    {
      var first = visits[0];
      Output.WriteLine($"[Visits] Première visite ID : {first.Identifier?.HospitalizationId}");
      Output.WriteLine($"[Visits]          Admission : {first.AdmissionDate:yyyy-MM-dd}");
      Output.WriteLine($"[Visits]          Sortie    : {(first.DischargeDate.HasValue ? first.DischargeDate.Value.ToString("yyyy-MM-dd") : "–")}");
      Output.WriteLine($"[Visits]          Location  : {first.Location?.Code}");
    }
  }

  [Fact(DisplayName = "SearchPatientVisitByIdentifier retourne la visite demandée")]
  public async Task SearchPatientVisitByIdentifier_WithValidId_ShouldReturnVisit()
  {
    // Arrange
    var visitId = PlaywrightConfiguration.BuildVisitIdentifier();

    // Act
    var visit = await CallSoapAsync(
        () => SoapClient.SearchPatientVisitByIdentifierAsync(visitId),
        $"SearchPatientVisitByIdentifier(HospitalizationId={PlaywrightConfiguration.TestHospitalizationId})");

    // Assert
    Assert.NotNull(visit);
    Output.WriteLine($"[VisitById] HospitalizationId : {visit.Identifier?.HospitalizationId}");
    Output.WriteLine($"[VisitById] PatientId         : {visit.PatientIdentifier?.PatientId}");
    Output.WriteLine($"[VisitById] Admission         : {visit.AdmissionDate:yyyy-MM-dd}");
  }

  [Fact(DisplayName = "GetInsurances retourne les assurances associées à la visite")]
  public async Task GetInsurances_ByVisitId_ShouldReturnInsurances()
  {
    // Arrange
    var visitId = PlaywrightConfiguration.BuildVisitIdentifier();

    // Act
    var insurances = await CallSoapAsync(
        () => SoapClient.GetInsurancesAsync(visitId),
        $"GetInsurances(HospitalizationId={PlaywrightConfiguration.TestHospitalizationId})");

    // Assert
    Assert.NotNull(insurances);
    Output.WriteLine($"[Insurances] {insurances.Length} assurance(s).");

    foreach (var ins in insurances)
      Output.WriteLine($"[Insurances]  → {ins.Name}");
  }

  [Fact(DisplayName = "GetGuarantor retourne le garant associé à la visite")]
  public async Task GetGuarantor_ByVisitId_ShouldReturnGuarantor()
  {
    // Arrange
    var visitId = PlaywrightConfiguration.BuildVisitIdentifier();

    // Act
    var guarantor = await CallSoapAsync(
        () => SoapClient.GetGuarantorAsync(visitId),
        $"GetGuarantor(HospitalizationId={PlaywrightConfiguration.TestHospitalizationId})");

    // Assert
    Assert.NotNull(guarantor);
    Output.WriteLine($"[Guarantor] {guarantor.Name} {guarantor.FirstName}");
  }

  [Fact(DisplayName = "Scénario complet : visites + assurances + garant enchaînés")]
  public async Task VisitFullConsultation_ShouldSucceed()
  {
    // Arrange
    var patientId = PlaywrightConfiguration.BuildPatientIdentifier();
    var visitId   = PlaywrightConfiguration.BuildVisitIdentifier();

    // Act
    var visits     = await CallSoapAsync(() => SoapClient.GetPatientVisitAsync(patientId),   "GetPatientVisit");
    var insurances = await CallSoapAsync(() => SoapClient.GetInsurancesAsync(visitId),        "GetInsurances");
    var guarantor  = await CallSoapAsync(() => SoapClient.GetGuarantorAsync(visitId),         "GetGuarantor");

    // Assert
    Assert.NotNull(visits);
    Assert.NotNull(insurances);
    Assert.NotNull(guarantor);

    Output.WriteLine($"[Consultation visite] Visites={visits.Length} | Assurances={insurances.Length} | Garant={guarantor.Name}");
  }
}
