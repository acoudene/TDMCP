using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests Playwright pour la consultation des informations patient :
/// GetPatientDetails, GetPatientComment, HasPatientMerge.
/// </summary>
public class PatientDetailsTests : PlaywrightTestBase
{
  public PatientDetailsTests(ITestOutputHelper output) : base(output) { }

  [Fact(DisplayName = "GetPatientDetails retourne les détails complets du patient")]
  public async Task GetPatientDetails_WithValidId_ShouldReturnPatient()
  {
    // Arrange
    var identifier = PlaywrightConfiguration.BuildPatientIdentifier();

    // Act
    var patient = await CallSoapAsync(
        () => SoapClient.GetPatientDetailsAsync(identifier),
        $"GetPatientDetails(PatientId={PlaywrightConfiguration.TestPatientId})");

    // Assert
    Assert.NotNull(patient);

    if (patient.Identifier is not null)
    {
      Output.WriteLine($"[Details] PatientId     : {patient.Identifier.PatientId}");
      Output.WriteLine($"[Details] PatientNumber : {patient.Identifier.PatientNumber}");
    }

    if (patient.Demographic is not null)
    {
      var d = patient.Demographic;
      Output.WriteLine($"[Details] Nom           : {d.Name}");
      Output.WriteLine($"[Details] Prénom        : {d.FirstName}");
      Output.WriteLine($"[Details] Naissance     : {d.BirthDate:yyyy-MM-dd}");
      Output.WriteLine($"[Details] Sexe          : {d.Sex}");
    }

    if (patient.Address is not null)
    {
      var a = patient.Address;
      Output.WriteLine($"[Details] Adresse       : {a.Address1}, {a.PostalCode} {a.City}");
    }
  }

  [Fact(DisplayName = "GetPatientComment retourne le commentaire associé au patient")]
  public async Task GetPatientComment_WithValidId_ShouldReturnComment()
  {
    // Arrange
    var identifier = PlaywrightConfiguration.BuildPatientIdentifier();

    // Act
    var comment = await CallSoapAsync(
        () => SoapClient.GetPatientCommentAsync(identifier),
        $"GetPatientComment(PatientId={PlaywrightConfiguration.TestPatientId})");

    // Assert
    Assert.NotNull(comment);
    Output.WriteLine($"[Comment] Texte : {comment.Text ?? "(vide)"}");
  }

  [Fact(DisplayName = "HasPatientMerge retourne un booléen sans lever d'exception")]
  public async Task HasPatientMerge_WithValidId_ShouldReturnBoolean()
  {
    // Arrange
    int? patientId = PlaywrightConfiguration.TestPatientId;

    // Act
    var hasMerge = await CallSoapAsync(
        () => SoapClient.HasPatientMergeAsync(patientId),
        $"HasPatientMerge(PatientId={patientId})");

    // Assert — on vérifie uniquement que l'appel réussit et retourne une valeur
    Output.WriteLine($"[Merge] HasPatientMerge({patientId}) = {hasMerge}");
  }

  [Fact(DisplayName = "Scénario de consultation complète : détails + commentaire enchaînés")]
  public async Task PatientFullConsultation_ShouldSucceed()
  {
    // Arrange
    var identifier = PlaywrightConfiguration.BuildPatientIdentifier();

    // Act
    var patient = await CallSoapAsync(
        () => SoapClient.GetPatientDetailsAsync(identifier),
        "GetPatientDetails (consultation complète)");

    var comment = await CallSoapAsync(
        () => SoapClient.GetPatientCommentAsync(identifier),
        "GetPatientComment (consultation complète)");

    // Assert
    Assert.NotNull(patient);
    Assert.NotNull(comment);

    Output.WriteLine("[Consultation complète] OK");
    Output.WriteLine($"  Patient : {patient.Demographic?.Name} {patient.Demographic?.FirstName}");
    Output.WriteLine($"  Comment : {comment.Text ?? "(vide)"}");
  }
}
