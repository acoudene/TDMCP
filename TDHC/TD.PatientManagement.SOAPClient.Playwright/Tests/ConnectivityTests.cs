using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright.Tests;

/// <summary>
/// Tests de connectivité : vérifie que le service SOAP est joignable,
/// que le WSDL est accessible via un navigateur réel et que le client
/// WCF peut s'ouvrir correctement vers l'endpoint configuré.
/// </summary>
public class ConnectivityTests : PlaywrightTestBase
{
  public ConnectivityTests(ITestOutputHelper output) : base(output) { }

  [Fact(DisplayName = "Le navigateur Playwright peut accéder à la page WSDL du service")]
  public async Task WSDL_ShouldBeAccessibleViaBrowser()
  {
    // Act : Playwright navigue vers l'URL ?wsdl
    await NavigateToWsdlAsync();

    // Assert : la page doit contenir du XML WSDL
    var content = await SoapPage.ContentAsync();
    Assert.True(
        content.Contains("wsdl", StringComparison.OrdinalIgnoreCase) ||
        content.Contains("definitions", StringComparison.OrdinalIgnoreCase),
        "Le contenu de la page ne ressemble pas à un document WSDL valide.");

    Output.WriteLine($"[WSDL] Aperçu ({Math.Min(256, content.Length)} car.) : " +
                     content[..Math.Min(256, content.Length)]);
  }

  [Fact(DisplayName = "Le client WCF peut atteindre l'état CommunicationState.Opened")]
  public async Task SoapClient_ShouldOpenSuccessfully()
  {
    // Act
    await SoapClient.OpenAsync();

    // Assert
    Assert.Equal(
        System.ServiceModel.CommunicationState.Opened,
        SoapClient.State);

    Output.WriteLine($"[Connectivity] État    : {SoapClient.State}");
    Output.WriteLine($"[Connectivity] Binding : {SoapClient.Endpoint.Binding.Name}");
  }

  [Fact(DisplayName = "L'URI et le binding de l'endpoint sont valides")]
  public Task SoapClient_EndpointInfo_ShouldBeValid()
  {
    Assert.NotEmpty(SoapClient.Endpoint.Address.Uri.AbsoluteUri);
    Assert.NotNull(SoapClient.Endpoint.Binding);

    Output.WriteLine($"[Endpoint] URI     : {SoapClient.Endpoint.Address.Uri}");
    Output.WriteLine($"[Endpoint] Binding : {SoapClient.Endpoint.Binding.Name}");
    return Task.CompletedTask;
  }

  [Fact(DisplayName = "Le timeout est correctement configuré sur le binding WCF")]
  public Task SoapClient_Timeout_ShouldBeConfigured()
  {
    var expected = TimeSpan.FromSeconds(PlaywrightConfiguration.DefaultTimeoutSeconds);

    Assert.Equal(expected, SoapClient.Endpoint.Binding.SendTimeout);
    Assert.Equal(expected, SoapClient.Endpoint.Binding.ReceiveTimeout);

    Output.WriteLine($"[Timeout] SendTimeout    : {SoapClient.Endpoint.Binding.SendTimeout}");
    Output.WriteLine($"[Timeout] ReceiveTimeout : {SoapClient.Endpoint.Binding.ReceiveTimeout}");
    return Task.CompletedTask;
  }
}
