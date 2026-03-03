# TD.PatientManagement.SOAPClient.Playwright

Assembly de tests d'intégration **Playwright** pour l'API SOAP `PatientManagementService` (TDHC).  
Framework de test : **xUnit v2** (`Microsoft.Playwright.Xunit 1.58.0`) — cohérent avec les autres projets de test de la solution.

---

## Pourquoi Playwright pour des tests SOAP ?

Playwright apporte trois bénéfices clés, même pour une API SOAP/WCF :

| Fonctionnalité | Bénéfice |
|---|---|
| **Traces (ZIP)** | Rejouez la séquence d'appels HTTP/SOAP et inspectez chaque requête/réponse dans la Playwright Trace Viewer |
| **Screenshots automatiques** | Capture d'écran immédiate sur chaque test en échec |
| **Validation WSDL via navigateur** | Vérifie que l'endpoint SOAP répond via un vrai navigateur Chromium (valide TLS, certificats auto-signés…) |

---

## Structure du projet

```
TD.PatientManagement.SOAPClient.Playwright/
├── PlaywrightConfiguration.cs          # Paramètres centralisés (endpoint, données, répertoires)
├── PlaywrightTestBase.cs               # Classe de base : hérite de PageTest (Playwright.Xunit)
│                                       #   + cycle de vie WCF + traces + screenshots xUnit
├── Tests/
│   ├── ConnectivityTests.cs            # Sanity check : WSDL accessible + client Open
│   ├── PatientSearchTests.cs           # SearchPatient (démographie, ID, numéro, vide)
│   ├── PatientDetailsTests.cs          # GetPatientDetails, GetPatientComment, HasPatientMerge
│   ├── PatientVisitTests.cs            # GetPatientVisit, SearchVisitById, GetInsurances, GetGuarantor
│   ├── RequestSearchTests.cs           # SearchRequest (numéro, patient, visite, vide)
│   └── PatientWriteTests.cs            # Create/Update/Delete Patient & Visit [Skip par défaut]
├── .runsettings                        # Config xUnit + Playwright + variables d'env
├── TD.PatientManagement.SOAPClient.Playwright.csproj
└── README.md
```

---

## Packages utilisés

| Package | Version | Rôle |
|---|---|---|
| `Microsoft.Playwright.Xunit` | 1.58.0 | Intégration Playwright pour xUnit (fournit `PageTest`) |
| `xunit` | 2.9.3 | Framework de test (cohérent avec la solution) |
| `xunit.runner.visualstudio` | 3.1.5 | Adaptateur pour VS Test Explorer et `dotnet test` |
| `Microsoft.NET.Test.Sdk` | 17.14.1 | SDK test .NET |
| `coverlet.collector` | 6.0.4 | Couverture de code |

---

## Architecture des tests

### `PlaywrightTestBase`

Hérite de `PageTest` (`Microsoft.Playwright.Xunit`) et implémente `IAsyncLifetime` (xUnit) :

```
xUnit [Fact]
  └── PlaywrightTestBase (hérite de PageTest + IAsyncLifetime)
        ├── IAsyncLifetime.InitializeAsync()  → crée SoapClient + SoapContext + démarre la trace
        ├── Test method                        → appels SOAP via CallSoapAsync<T>() + Assert xUnit
        └── IAsyncLifetime.DisposeAsync()     → screenshot si échec + sauvegarde trace + ferme WCF
```

`PageTest` de `Microsoft.Playwright.Xunit` gère automatiquement :
- La création du `IPlaywright`, `IBrowser`, et d'une `IPage` isolée par test
- La connexion de `ITestOutputHelper` aux logs Playwright

### Gestion des erreurs SOAP

`CallSoapAsync<T>()` encapsule chaque appel WCF et logue les erreurs :
- `FaultException` (erreur métier SOAP)
- `CommunicationException` (problème réseau/binding)
- `TimeoutException`

---

## Prérequis

- **.NET 10 SDK**
- **PowerShell 7+** (`pwsh`) pour l'installation automatique de Playwright au build
- Accès réseau au endpoint SOAP

### Installation des navigateurs Playwright

Automatisée au premier `dotnet build`. Pour l'effectuer manuellement :

```powershell
pwsh playwright.ps1 install chromium
```

En CI/CD (Linux) :

```bash
dotnet build
dotnet run --project . -- playwright install chromium
```

---

## Intégration dans la solution TDHC

Ajoutez le projet dans `TDHC.slnx` :

```xml
<Project Path="TD.PatientManagement.SOAPClient.Playwright/TD.PatientManagement.SOAPClient.Playwright.csproj" />
```

---

## Configuration

Toutes les valeurs sont dans `PlaywrightConfiguration.cs`, surchargeables par variable d'environnement :

| Variable | Description | Défaut |
|---|---|---|
| `SOAP_ENDPOINT_URL` | URL complète du service SOAP | URL générée WCF |
| `TEST_PATIENT_ID` | ID du patient de référence | `12345` |
| `TEST_HOSPITALIZATION_ID` | ID d'hospitalisation de référence | `67890` |
| `PLAYWRIGHT_HEADLESS` | `false` = navigateur visible (debug) | `true` |
| `PLAYWRIGHT_TRACES_DIR` | Répertoire des traces `.zip` | `<bin>/playwright-traces` |
| `PLAYWRIGHT_SCREENSHOTS_DIR` | Répertoire des screenshots d'échec | `<bin>/playwright-screenshots` |

---

## Exécution

```bash
# Tous les tests de lecture (les Write sont Skip par défaut)
dotnet test --settings .runsettings

# Cibler une classe précise
dotnet test --filter "FullyQualifiedName~ConnectivityTests"
dotnet test --filter "FullyQualifiedName~PatientSearchTests"
dotnet test --filter "FullyQualifiedName~PatientDetailsTests"
dotnet test --filter "FullyQualifiedName~PatientVisitTests"
dotnet test --filter "FullyQualifiedName~RequestSearchTests"

# Debug avec navigateur visible
PLAYWRIGHT_HEADLESS=false dotnet test --filter "FullyQualifiedName~ConnectivityTests"

# Activer les tests d'écriture : retirer le paramètre Skip dans PatientWriteTests.cs
dotnet test --filter "FullyQualifiedName~PatientWriteTests"
```

---

## Tests d'écriture (`PatientWriteTests`)

Les tests `[Fact(Skip = "...")]` sont ignorés automatiquement par xUnit.  
Pour les activer, retirez le paramètre `Skip` dans `PatientWriteTests.cs`.

> ⚠️ Ces tests modifient des données réelles. À exécuter uniquement en environnement dédié.

---

## Consulter les traces Playwright

```bash
pwsh playwright.ps1 show-trace playwright-traces/<nom_du_test>.zip
```

La trace contient : timeline des appels réseau HTTP/SOAP, snapshots DOM, screenshots intermédiaires, code source des tests.
