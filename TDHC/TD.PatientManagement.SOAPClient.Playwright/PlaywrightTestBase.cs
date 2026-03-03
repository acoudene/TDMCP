using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using PatientManagementService;
using Xunit;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Playwright;

/// <summary>
/// Classe de base pour tous les tests Playwright SOAP.
/// <para>
/// Elle hérite de <see cref="PageTest"/> (<c>Microsoft.Playwright.Xunit</c>) qui gère
/// automatiquement le cycle de vie Playwright : création du browser, du contexte et de
/// la page isolée par test.
/// </para>
/// <para>
/// En plus du cycle de vie Playwright standard, cette classe orchestre :
/// <list type="bullet">
///   <item>L'instanciation et la fermeture du client WCF <see cref="PatientManagementServiceClient"/>.</item>
///   <item>Le démarrage et la sauvegarde des <b>traces</b> Playwright (screenshots + snapshots + sources).</item>
///   <item>La capture de <b>screenshots</b> automatique en cas d'échec de test.</item>
///   <item>Les logs structurés via <see cref="ITestOutputHelper"/> (xUnit).</item>
/// </list>
/// </para>
/// <remarks>
/// Le constructeur reçoit <see cref="ITestOutputHelper"/> par injection xUnit,
/// puis le transmet à <see cref="PageTest"/> qui le connecte au sink de traces Playwright.
/// </remarks>
/// </summary>
public abstract class PlaywrightTestBase : PageTest, IAsyncLifetime
{
  // ── Output xUnit ─────────────────────────────────────────────────────────────

  /// <summary>Helper xUnit pour écrire dans la sortie de test.</summary>
  protected readonly ITestOutputHelper Output;

  // ── Client SOAP ───────────────────────────────────────────────────────────────

  /// <summary>Client WCF vers l'API SOAP PatientManagementService.</summary>
  protected PatientManagementServiceClient SoapClient { get; private set; } = null!;

  // ── Playwright — contexte et page dédiés aux tests SOAP ──────────────────────

  /// <summary>
  /// Contexte Playwright isolé, créé en plus du contexte hérité de <see cref="PageTest"/>.
  /// Utile pour naviguer vers les URLs WSDL ou capturer des traces HTTP SOAP.
  /// </summary>
  protected IBrowserContext SoapContext { get; private set; } = null!;

  /// <summary>Page Playwright rattachée à <see cref="SoapContext"/>.</summary>
  protected IPage SoapPage { get; private set; } = null!;

  // ── État interne ──────────────────────────────────────────────────────────────

  private string  _testName  = string.Empty;
  private string  _traceFile = string.Empty;
  private bool    _testFailed;

  // ── Constructeur ─────────────────────────────────────────────────────────────

  /// <param name="output">
  /// Injecté par xUnit ; transmis à <see cref="PageTest"/> pour connecter
  /// les logs Playwright au runner de test.
  /// </param>
  protected PlaywrightTestBase(ITestOutputHelper output)
  {
    Output = output;
  }

  // ── Surcharge des options Playwright ─────────────────────────────────────────

  /// <inheritdoc />
  /// <remarks>
  /// <see cref="BrowserNewContextOptions.IgnoreHTTPSErrors"/> est activé car
  /// l'API SOAP utilise un certificat TLS auto-signé sur les environnements de dev
  /// (cf. TDHC9XACE.cer dans le projet client).
  /// </remarks>
  public override BrowserNewContextOptions ContextOptions() => new()
  {
    IgnoreHTTPSErrors = true,
  };

  // ── IAsyncLifetime : InitializeAsync ─────────────────────────────────────────

  /// <summary>
  /// Appelé par xUnit avant chaque test.
  /// Initialise le client SOAP, le contexte Playwright dédié et démarre la trace.
  /// </summary>
  async Task IAsyncLifetime.InitializeAsync()
  {
    // ── Nom du test en cours (pour nommer les fichiers de trace/screenshot) ──
    _testName  = GetType().Name + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
    _traceFile = Path.Combine(
        PlaywrightConfiguration.TracesDirectory,
        $"{SanitizeFileName(_testName)}.zip");

    Directory.CreateDirectory(PlaywrightConfiguration.TracesDirectory);
    Directory.CreateDirectory(PlaywrightConfiguration.ScreenshotsDirectory);

    // ── Contexte Playwright SOAP (isolé du contexte Page hérité) ──
    SoapContext = await Browser.NewContextAsync(ContextOptions());

    await SoapContext.Tracing.StartAsync(new TracingStartOptions
    {
      Title       = _testName,
      Screenshots = true,
      Snapshots   = true,
      Sources     = true,
    });

    SoapPage = await SoapContext.NewPageAsync();

    // ── Client SOAP ──
    SoapClient = PlaywrightConfiguration.CreateConfiguredClient();

    // ── Logs de démarrage ──
    Output.WriteLine($"[Setup] Test       : {_testName}");
    Output.WriteLine($"[Setup] Endpoint   : {PlaywrightConfiguration.DefaultEndpoint}");
    Output.WriteLine($"[Setup] Client URL : {SoapClient.Endpoint.Address.Uri}");
    Output.WriteLine($"[Setup] Headless   : {PlaywrightConfiguration.Headless}");
  }

  // ── IAsyncLifetime : DisposeAsync ─────────────────────────────────────────────

  /// <summary>
  /// Appelé par xUnit après chaque test.
  /// Sauvegarde la trace Playwright, prend un screenshot si le test a échoué,
  /// ferme le contexte et le client WCF.
  /// </summary>
  async Task IAsyncLifetime.DisposeAsync()
  {
    // ── Screenshot en cas d'échec ──
    if (_testFailed)
    {
      try
      {
        var screenshotPath = Path.Combine(
            PlaywrightConfiguration.ScreenshotsDirectory,
            $"{SanitizeFileName(_testName)}_FAILED.png");

        await SoapPage.ScreenshotAsync(new PageScreenshotOptions
        {
          Path     = screenshotPath,
          FullPage = true,
        });

        Output.WriteLine($"[TearDown] Screenshot : {screenshotPath}");
      }
      catch (Exception ex)
      {
        Output.WriteLine($"[TearDown] Screenshot échoué : {ex.Message}");
      }
    }

    // ── Sauvegarde de la trace ──
    if (SoapContext is not null)
    {
      await SoapContext.Tracing.StopAsync(new TracingStopOptions { Path = _traceFile });
      Output.WriteLine($"[TearDown] Trace : {_traceFile}");
      await SoapContext.CloseAsync();
    }

    // ── Fermeture du client WCF ──
    if (SoapClient is not null)
    {
      try
      {
        if (SoapClient.State == System.ServiceModel.CommunicationState.Opened)
          SoapClient.Close();
      }
      catch
      {
        SoapClient.Abort();
      }
    }
  }

  // ── Utilitaires protégés ──────────────────────────────────────────────────────

  /// <summary>
  /// Navigue vers l'URL WSDL du service dans le navigateur Playwright
  /// afin de valider la disponibilité du endpoint et produire un snapshot visuel.
  /// </summary>
  protected async Task NavigateToWsdlAsync()
  {
    var wsdlUrl = $"{SoapClient.Endpoint.Address.Uri}?wsdl";
    Output.WriteLine($"[WSDL] Navigation → {wsdlUrl}");

    var response = await SoapPage.GotoAsync(wsdlUrl, new PageGotoOptions
    {
      WaitUntil = WaitUntilState.DOMContentLoaded,
      Timeout   = PlaywrightConfiguration.DefaultTimeoutSeconds * 1_000,
    });

    Assert.True(response?.Ok,
        $"La page WSDL est inaccessible (HTTP {response?.Status}) : {wsdlUrl}");
  }

  /// <summary>
  /// Marque le test courant comme échoué (pour déclencher le screenshot dans DisposeAsync).
  /// À appeler dans un bloc catch ou via un wrapper d'assertion.
  /// </summary>
  protected void MarkTestFailed() => _testFailed = true;

  /// <summary>
  /// Exécute un appel SOAP en capturant et loggant les erreurs WCF de manière structurée.
  /// En cas d'exception, marque le test comme échoué avant de relancer.
  /// </summary>
  protected async Task<T> CallSoapAsync<T>(Func<Task<T>> call, string description)
  {
    try
    {
      Output.WriteLine($"[SOAP ►] {description}");
      var result = await call();
      Output.WriteLine($"[SOAP ✓] {description}");
      return result;
    }
    catch (System.ServiceModel.FaultException fault)
    {
      _testFailed = true;
      Output.WriteLine($"[SOAP ✗] FaultException – {description}");
      Output.WriteLine($"         Message : {fault.Message}");
      Output.WriteLine($"         Reason  : {fault.Reason}");
      throw;
    }
    catch (System.ServiceModel.CommunicationException commEx)
    {
      _testFailed = true;
      Output.WriteLine($"[SOAP ✗] CommunicationException – {description}");
      Output.WriteLine($"         Message : {commEx.Message}");
      throw;
    }
    catch (TimeoutException tEx)
    {
      _testFailed = true;
      Output.WriteLine($"[SOAP ✗] TimeoutException – {description}");
      Output.WriteLine($"         Message : {tEx.Message}");
      throw;
    }
    catch
    {
      _testFailed = true;
      throw;
    }
  }

  // ── Utilitaires privés ────────────────────────────────────────────────────────

  private static string SanitizeFileName(string name) =>
      string.Concat(name.Select(c =>
          Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
}
