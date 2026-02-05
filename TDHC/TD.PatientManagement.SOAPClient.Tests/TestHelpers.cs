using PatientManagementService;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Tests;

/// <summary>
/// Méthodes utilitaires pour faciliter l'écriture des tests
/// </summary>
public static class TestHelpers
{
  /// <summary>
  /// Crée un SiteIdentifierDTO avec les valeurs par défaut
  /// </summary>
  public static SiteIdentifierDTO CreateDefaultSiteIdentifier()
  {
    return new SiteIdentifierDTO
    {
      SiteId = TestConfiguration.DefaultSiteId,
      SiteCode = TestConfiguration.DefaultSiteCode
    };
  }

  /// <summary>
  /// Crée un PatientIdentifierDTO avec les valeurs par défaut
  /// </summary>
  public static PatientIdentifierDTO CreateTestPatientIdentifier(int? patientId = null, string? patientNumber = null)
  {
    return new PatientIdentifierDTO
    {
      PatientId = patientId ?? TestConfiguration.TestPatientId,
      PatientNumber = patientNumber ?? TestConfiguration.TestPatientNumber
    };
  }

  /// <summary>
  /// Crée un PatientVisitIdentifierDTO avec les valeurs par défaut
  /// </summary>
  public static PatientVisitIdentifierDTO CreateTestPatientVisitIdentifier(int? hospitalizationId = null, string? hospitalizationNumber = null)
  {
    return new PatientVisitIdentifierDTO
    {
      HospitalizationId = hospitalizationId ?? TestConfiguration.TestHospitalizationId,
      HospitalizationNumber = hospitalizationNumber ?? TestConfiguration.TestHospitalizationNumber
    };
  }

  /// <summary>
  /// Crée un LocationIdentifierDTO avec les valeurs par défaut
  /// </summary>
  public static LocationIdentifierDTO CreateTestLocationIdentifier(int? locId = null, string? code = null)
  {
    return new LocationIdentifierDTO
    {
      LocId = locId ?? TestConfiguration.TestVisitData.LocationId,
      Code = code ?? TestConfiguration.TestVisitData.LocationCode
    };
  }

  /// <summary>
  /// Crée un PatientSearchCriteriaDTO avec les critères de base
  /// </summary>
  public static PatientSearchCriteriaDTO CreateBasicPatientSearchCriteria(
      string? name = null,
      string? firstName = null,
      int? patientId = null)
  {
    var criteria = new PatientSearchCriteriaDTO();

    if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(firstName))
    {
      criteria.Demographic = new PatientDemographicDTO
      {
        Name = name,
        FirstName = firstName
      };
    }

    if (patientId.HasValue)
    {
      criteria.Patient = CreateTestPatientIdentifier(patientId.Value);
    }

    return criteria;
  }

  /// <summary>
  /// Crée un PatientDTO de test pour la création
  /// </summary>
  public static PatientDTO CreateTestPatient()
  {
    return new PatientDTO
    {
      Demographic = new PatientDemographicDTO
      {
        Name = TestConfiguration.TestPatientData.Name,
        FirstName = TestConfiguration.TestPatientData.FirstName,
        BirthDate = TestConfiguration.TestPatientData.BirthDate,
        Sex = TestConfiguration.TestPatientData.Sex
      },
      Address = new PatientAddressDTO
      {
        Address1 = TestConfiguration.TestPatientData.Street1,
        City = TestConfiguration.TestPatientData.City,
        PostalCode = TestConfiguration.TestPatientData.ZipCode,
        Country = TestConfiguration.TestPatientData.Country
      }
    };
  }

  /// <summary>
  /// Crée un PatientVisitDTO de test pour la création
  /// </summary>
  public static PatientVisitDTO CreateTestPatientVisit(int? patientId = null)
  {
    return new PatientVisitDTO
    {
      PatientIdentifier = CreateTestPatientIdentifier(patientId),
      AdmissionDate = TestConfiguration.TestVisitData.AdmissionDate,
      Location = CreateTestLocationIdentifier()
    };
  }

  /// <summary>
  /// Affiche les détails d'un patient dans les logs de test
  /// </summary>
  public static void LogPatientDetails(PatientDTO patient, ITestOutputHelper output)
  {
    if (patient == null)
    {
      output.WriteLine("Patient: null");
      return;
    }

    output.WriteLine("=== Détails du Patient ===");

    if (patient.Identifier != null)
    {
      output.WriteLine($"ID Patient: {patient.Identifier.PatientId}");
      output.WriteLine($"Numéro Patient: {patient.Identifier.PatientNumber}");
    }

    if (patient.Demographic != null)
    {
      var demo = patient.Demographic;
      output.WriteLine($"Nom: {demo.Name}");
      output.WriteLine($"Prénom: {demo.FirstName}");
      output.WriteLine($"Date de naissance: {demo.BirthDate:yyyy-MM-dd}");
      output.WriteLine($"Sexe: {demo.Sex}");

      if (demo.Title != null)
        output.WriteLine($"Titre: {demo.Title.Code}");

      if (!string.IsNullOrEmpty(demo.GivenNames))
        output.WriteLine($"Autres prénoms: {demo.GivenNames}");
    }

    if (patient.Address != null)
    {
      var address = patient.Address;
      output.WriteLine("--- Adresse ---");
      output.WriteLine($"Rue: {address.Address1}");
      if (!string.IsNullOrEmpty(address.Address2))
        output.WriteLine($"Complément: {address.Address2}");
      output.WriteLine($"Ville: {address.City}");
      output.WriteLine($"Code postal: {address.PostalCode}");
      output.WriteLine($"Pays: {address.Country}");
    }

    output.WriteLine("========================");
  }

  /// <summary>
  /// Affiche les détails d'une visite dans les logs de test
  /// </summary>
  public static void LogVisitDetails(PatientVisitDTO visit, ITestOutputHelper output)
  {
    if (visit == null)
    {
      output.WriteLine("Visite: null");
      return;
    }

    output.WriteLine("=== Détails de la Visite ===");

    if (visit.Identifier != null)
    {
      output.WriteLine($"ID Visite: {visit.Identifier.HospitalizationId}");
      output.WriteLine($"Numéro Visite: {visit.Identifier.HospitalizationNumber}");
    }

    if (visit.PatientIdentifier != null)
    {
      output.WriteLine($"ID Patient: {visit.PatientIdentifier.PatientId}");
    }

    output.WriteLine($"Date d'admission: {visit.AdmissionDate:yyyy-MM-dd HH:mm:ss}");

    if (visit.DischargeDate.HasValue)
      output.WriteLine($"Date de sortie: {visit.DischargeDate:yyyy-MM-dd HH:mm:ss}");

    if (visit.Location != null)
    {
      output.WriteLine($"Localisation: {visit.Location.Code} ({visit.Location.Name})");
    }

    output.WriteLine("============================");
  }

  /// <summary>
  /// Affiche un résumé des résultats de recherche
  /// </summary>
  public static void LogSearchResults<T>(T[] results, ITestOutputHelper output, string entityName = "résultats")
  {
    if (results == null)
    {
      output.WriteLine($"Aucun {entityName} (null)");
      return;
    }

    output.WriteLine($"Nombre de {entityName}: {results.Length}");

    if (results.Length == 0)
    {
      output.WriteLine($"Aucun {entityName} trouvé");
    }
  }

  /// <summary>
  /// Exécute une action de test avec gestion des erreurs standardisée
  /// </summary>
  public static async Task<T> ExecuteTestActionAsync<T>(
      Func<Task<T>> action,
      ITestOutputHelper output,
      string actionDescription)
  {
    try
    {
      output.WriteLine($"Début: {actionDescription}");
      var result = await action();
      output.WriteLine($"Succès: {actionDescription}");
      return result;
    }
    catch (System.ServiceModel.FaultException fault)
    {
      output.WriteLine($"Erreur SOAP: {actionDescription}");
      output.WriteLine($"Message: {fault.Message}");
      output.WriteLine($"Reason: {fault.Reason}");
      if (fault.InnerException != null)
        output.WriteLine($"Inner: {fault.InnerException.Message}");
      throw;
    }
    catch (System.ServiceModel.CommunicationException commEx)
    {
      output.WriteLine($"Erreur de communication: {actionDescription}");
      output.WriteLine($"Message: {commEx.Message}");
      if (commEx.InnerException != null)
        output.WriteLine($"Inner: {commEx.InnerException.Message}");
      throw;
    }
    catch (TimeoutException timeoutEx)
    {
      output.WriteLine($"Timeout: {actionDescription}");
      output.WriteLine($"Message: {timeoutEx.Message}");
      throw;
    }
    catch (Exception ex)
    {
      output.WriteLine($"Erreur inattendue: {actionDescription}");
      output.WriteLine($"Type: {ex.GetType().Name}");
      output.WriteLine($"Message: {ex.Message}");
      output.WriteLine($"Stack: {ex.StackTrace}");
      throw;
    }
  }

  /// <summary>
  /// Vérifie si un patient est valide (non null avec données de base)
  /// </summary>
  public static bool IsValidPatient(PatientDTO patient)
  {
    return patient != null &&
           patient.Identifier != null &&
           patient.Demographic != null;
  }

  /// <summary>
  /// Vérifie si une visite est valide (non null avec données de base)
  /// </summary>
  public static bool IsValidVisit(PatientVisitDTO visit)
  {
    return visit != null &&
           visit.Identifier != null &&
           visit.PatientIdentifier != null;
  }

  /// <summary>
  /// Compare deux dates en ignorant les millisecondes
  /// </summary>
  public static bool AreDatesEqual(DateTime? date1, DateTime? date2)
  {
    if (!date1.HasValue && !date2.HasValue)
      return true;

    if (!date1.HasValue || !date2.HasValue)
      return false;

    var d1 = new DateTime(date1.Value.Year, date1.Value.Month, date1.Value.Day,
                          date1.Value.Hour, date1.Value.Minute, date1.Value.Second);
    var d2 = new DateTime(date2.Value.Year, date2.Value.Month, date2.Value.Day,
                          date2.Value.Hour, date2.Value.Minute, date2.Value.Second);

    return d1 == d2;
  }

  /// <summary>
  /// Génère un identifiant unique pour les tests
  /// </summary>
  public static string GenerateUniqueTestId(string prefix = "TEST")
  {
    return $"{prefix}_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid():N}".Substring(0, 50);
  }
}
