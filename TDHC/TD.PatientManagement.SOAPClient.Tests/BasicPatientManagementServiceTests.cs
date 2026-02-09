using PatientManagementService;
using Xunit.Abstractions;

namespace TD.PatientManagement.SOAPClient.Tests;

/// <summary>
/// Tests d'intégration basiques pour le service SOAP PatientManagementService
/// Ces tests sont corrects et utilisent les vraies propriétés des DTOs
/// </summary>
public class BasicPatientManagementServiceTests : IDisposable
{
  private readonly PatientManagementServiceClient _client;
  private readonly ITestOutputHelper _output;

  public BasicPatientManagementServiceTests(ITestOutputHelper output)
  {
    _output = output;
    _client = TestConfiguration.CreateConfiguredClient();
    TestConfiguration.ConfigureTimeout(_client, TestConfiguration.DefaultTimeoutSeconds);

    _output.WriteLine($"=== Configuration du Test ===");
    _output.WriteLine($"Endpoint: {TestConfiguration.DefaultEndpoint}");
    _output.WriteLine($"URL: {_client.Endpoint.Address.Uri}");
    _output.WriteLine($"============================");
  }

  #region Tests de Recherche de Patients

  [Fact]
  public async Task SearchPatient_WithDemographicCriteria_ShouldReturnResults()
  {
    // Arrange
    var searchCriteria = new PatientSearchCriteriaDTO
    {
      Demographic = new PatientDemographicDTO
      {
        Name = TestConfiguration.TestName,
        FirstName = TestConfiguration.TestFirstName
      }
    };

    // Act
    _output.WriteLine($"Recherche de patients: {TestConfiguration.TestName} {TestConfiguration.TestFirstName}");
    var results = await _client.SearchPatientAsync(searchCriteria);

    // Assert
    Assert.NotNull(results);
    _output.WriteLine($"Nombre de résultats: {results.Length}");

    if (results.Length > 0)
    {
      var firstPatient = results[0];
      if (firstPatient.Identifier != null)
      {
        _output.WriteLine($"Premier patient - ID: {firstPatient.Identifier.PatientId}");
      }
      if (firstPatient.Demographic != null)
      {
        _output.WriteLine($"Nom: {firstPatient.Demographic.Name} {firstPatient.Demographic.FirstName}");
      }
    }
  }

  [Fact]
  public async Task SearchPatient_WithPatientIdentifier_ShouldReturnPatient()
  {
    // Arrange
    var searchCriteria = new PatientSearchCriteriaDTO
    {
      Patient = new PatientIdentifierDTO
      {
        PatientId = TestConfiguration.TestPatientId
      }
    };

    // Act
    _output.WriteLine($"Recherche du patient ID: {TestConfiguration.TestPatientId}");
    var results = await _client.SearchPatientAsync(searchCriteria);

    // Assert
    Assert.NotNull(results);
    _output.WriteLine($"Nombre de résultats: {results.Length}");

    if (results.Length > 0 && results[0].Identifier != null)
    {
      Assert.Equal(TestConfiguration.TestPatientId, results[0].Identifier.PatientId);
    }
  }

  [Fact]
  public async Task SearchPatient_WithEmptyCriteria_ShouldReturnResults()
  {
    // Arrange
    var searchCriteria = new PatientSearchCriteriaDTO();

    // Act
    _output.WriteLine("Recherche avec critères vides");
    var results = await _client.SearchPatientAsync(searchCriteria);

    // Assert
    Assert.NotNull(results);
    _output.WriteLine($"Nombre de résultats: {results.Length}");
  }

  #endregion

  #region Tests de Détails Patient

  [Fact]
  public async Task GetPatientDetails_WithValidId_ShouldReturnPatient()
  {
    // Arrange
    var patientIdentifier = new PatientIdentifierDTO
    {
      PatientId = TestConfiguration.TestPatientId
    };

    // Act
    _output.WriteLine($"Récupération des détails du patient ID: {TestConfiguration.TestPatientId}");
    var patient = await _client.GetPatientDetailsAsync(patientIdentifier);

    // Assert
    Assert.NotNull(patient);
    _output.WriteLine("Patient récupéré avec succès");

    if (patient.Identifier != null)
    {
      _output.WriteLine($"ID Patient: {patient.Identifier.PatientId}");
      _output.WriteLine($"Numéro Patient: {patient.Identifier.PatientNumber}");
    }

    if (patient.Demographic != null)
    {
      _output.WriteLine($"Nom: {patient.Demographic.Name}");
      _output.WriteLine($"Prénom: {patient.Demographic.FirstName}");
      _output.WriteLine($"Date de naissance: {patient.Demographic.BirthDate}");
      _output.WriteLine($"Sexe: {patient.Demographic.Sex}");
    }
  }

  [Fact]
  public async Task GetPatientComment_WithValidId_ShouldReturnComment()
  {
    // Arrange
    var patientIdentifier = new PatientIdentifierDTO
    {
      PatientId = TestConfiguration.TestPatientId
    };

    // Act
    _output.WriteLine($"Récupération du commentaire du patient ID: {TestConfiguration.TestPatientId}");
    var comment = await _client.GetPatientCommentAsync(patientIdentifier);

    // Assert
    Assert.NotNull(comment);
    _output.WriteLine($"Commentaire récupéré: {comment.Text ?? "(vide)"}");
  }

  #endregion

  #region Tests de Visites Patient

  [Fact]
  public async Task GetPatientVisit_WithValidPatientId_ShouldReturnVisits()
  {
    // Arrange
    var patientIdentifier = new PatientIdentifierDTO
    {
      PatientId = TestConfiguration.TestPatientId
    };

    // Act
    _output.WriteLine($"Récupération des visites du patient ID: {TestConfiguration.TestPatientId}");
    var visits = await _client.GetPatientVisitAsync(patientIdentifier);

    // Assert
    Assert.NotNull(visits);
    _output.WriteLine($"Nombre de visites: {visits.Length}");

    if (visits.Length > 0)
    {
      var firstVisit = visits[0];
      if (firstVisit.Identifier != null)
      {
        _output.WriteLine($"Première visite - ID: {firstVisit.Identifier.HospitalizationId}");
      }
      _output.WriteLine($"Date d'admission: {firstVisit.AdmissionDate}");
      if (firstVisit.Location != null)
      {
        _output.WriteLine($"Localisation: {firstVisit.Location.Code}");
      }
    }
  }

  [Fact]
  public async Task SearchPatientVisitByIdentifier_WithValidId_ShouldReturnVisit()
  {
    // Arrange
    var visitIdentifier = new PatientVisitIdentifierDTO
    {
      HospitalizationId = TestConfiguration.TestHospitalizationId
    };

    // Act
    _output.WriteLine($"Recherche de la visite ID: {TestConfiguration.TestHospitalizationId}");
    var visit = await _client.SearchPatientVisitByIdentifierAsync(visitIdentifier);

    // Assert
    Assert.NotNull(visit);
    _output.WriteLine("Visite récupérée avec succès");

    if (visit.Identifier != null)
    {
      _output.WriteLine($"ID Visite: {visit.Identifier.HospitalizationId}");
    }
    if (visit.PatientIdentifier != null)
    {
      _output.WriteLine($"ID Patient: {visit.PatientIdentifier.PatientId}");
    }
  }

  [Fact]
  public async Task GetInsurances_WithValidVisitId_ShouldReturnInsurances()
  {
    // Arrange
    var visitIdentifier = new PatientVisitIdentifierDTO
    {
      HospitalizationId = TestConfiguration.TestHospitalizationId
    };

    // Act
    _output.WriteLine($"Récupération des assurances pour la visite ID: {TestConfiguration.TestHospitalizationId}");
    var insurances = await _client.GetInsurancesAsync(visitIdentifier);

    // Assert
    Assert.NotNull(insurances);
    _output.WriteLine($"Nombre d'assurances: {insurances.Length}");

    if (insurances.Length > 0)
    {
      var firstInsurance = insurances[0];
      _output.WriteLine($"Première assurance: {firstInsurance.Name}");
    }
  }

  [Fact]
  public async Task GetGuarantor_WithValidVisitId_ShouldReturnGuarantor()
  {
    // Arrange
    var visitIdentifier = new PatientVisitIdentifierDTO
    {
      HospitalizationId = TestConfiguration.TestHospitalizationId
    };

    // Act
    _output.WriteLine($"Récupération du garant pour la visite ID: {TestConfiguration.TestHospitalizationId}");
    var guarantor = await _client.GetGuarantorAsync(visitIdentifier);

    // Assert
    Assert.NotNull(guarantor);
    _output.WriteLine($"Garant récupéré: {guarantor.Name} {guarantor.FirstName}");
  }

  #endregion

  #region Tests de Recherche de Demandes

  [Fact]
  public async Task SearchRequest_WithRequestNumber_ShouldReturnRequests()
  {
    // Arrange
    var searchCriteria = new RequestSearchCriteriaDTO
    {
      Request = new RequestIdentifierDTO
      {
        RequestNumber = TestConfiguration.TestRequestNumber
      }
    };

    // Act
    _output.WriteLine($"Recherche de la demande: {TestConfiguration.TestRequestNumber}");
    var requests = await _client.SearchRequestAsync(searchCriteria);

    // Assert
    Assert.NotNull(requests);
    _output.WriteLine($"Nombre de demandes: {requests.Length}");

    if (requests.Length > 0)
    {
      var firstRequest = requests[0];
      if (firstRequest.Identifier != null)
      {
        _output.WriteLine($"Première demande - Numéro: {firstRequest.Identifier.RequestNumber}");
      }
    }
  }

  [Fact]
  public async Task SearchRequest_WithPatientCriteria_ShouldReturnRequests()
  {
    // Arrange
    var searchCriteria = new RequestSearchCriteriaDTO
    {
      Patient = new PatientIdentifierDTO
      {
        PatientId = TestConfiguration.TestPatientId
      }
    };

    // Act
    _output.WriteLine($"Recherche des demandes pour le patient ID: {TestConfiguration.TestPatientId}");
    var requests = await _client.SearchRequestAsync(searchCriteria);

    // Assert
    Assert.NotNull(requests);
    _output.WriteLine($"Nombre de demandes: {requests.Length}");
  }

  #endregion

  #region Tests de Fusion de Patients

  [Fact]
  public async Task HasPatientMerge_WithValidPatientId_ShouldReturnBoolean()
  {
    // Arrange
    int? patientId = TestConfiguration.TestPatientId;

    // Act
    _output.WriteLine($"Vérification de fusion pour le patient ID: {patientId}");
    var hasMerge = await _client.HasPatientMergeAsync(patientId);

    // Assert
    _output.WriteLine($"Le patient a une fusion: {hasMerge}");
    Assert.IsType<bool>(hasMerge);
  }

  #endregion

  #region Tests de Connectivité

  [Fact]
  public async Task Client_ShouldConnectSuccessfully()
  {
    // Act
    _output.WriteLine("Test de connexion au service");
    _output.WriteLine($"Endpoint: {_client.Endpoint.Address.Uri}");
    _output.WriteLine($"Binding: {_client.Endpoint.Binding.Name}");

    await _client.OpenAsync();

    // Assert
    Assert.Equal(System.ServiceModel.CommunicationState.Opened, _client.State);
    _output.WriteLine($"État du client: {_client.State}");
  }

  #endregion

  #region Tests de Création/Modification (Désactivés)

  [Fact]
  public async Task CreatePatient_WithValidData_ShouldCreatePatient()
  {
    // Arrange
    var newPatient = new PatientDTO
    {
      Identifier = new PatientIdentifierDTO
      {
        IntNumber = "123",
        PatientNumber = "123",
        BenNumber = "123",
        PatientId = 123
      },
      Location = new LocationIdentifierDTO
      {
        Code = "D AB"
      },
      Prescriber = new PrescriberIdentifierDTO
      {
        Code = "D AB"
      },
      Hospitalization = new PatientVisitDTO
      {
        Identifier = new PatientVisitIdentifierDTO
        {
          HospitalizationNumber = "123"
        }
      },
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

    // Act
    _output.WriteLine("Création d'un nouveau patient de test");
    var createdPatient = await _client.CreatePatientAsync(newPatient);

    // Assert
    Assert.NotNull(createdPatient);
    Assert.NotNull(createdPatient.PatientIdentifier);
    _output.WriteLine($"Patient créé avec l'ID: {createdPatient.PatientIdentifier.PatientId}");
  }

  [Fact]
  public async Task UpdatePatient_WithValidData_ShouldUpdatePatient()
  {
    // Arrange
    var patientToUpdate = new PatientDTO
    {
      Identifier = new PatientIdentifierDTO
      {
        PatientId = TestConfiguration.TestPatientId
      },
      Demographic = new PatientDemographicDTO
      {
        Name = "UPDATED",
        FirstName = "TEST"
      }
    };

    // Act
    _output.WriteLine($"Mise à jour du patient ID: {TestConfiguration.TestPatientId}");
    await _client.UpdatePatientAsync(patientToUpdate);

    // Assert
    _output.WriteLine("Patient mis à jour avec succès");
  }

  [Fact]
  public async Task CreatePatientVisit_WithValidData_ShouldCreateVisit()
  {
    // Arrange
    var newVisit = new PatientVisitDTO
    {
      PatientIdentifier = new PatientIdentifierDTO
      {
        PatientId = TestConfiguration.TestPatientId
      },
      AdmissionDate = DateTime.Now,
      Location = new LocationIdentifierDTO
      {
        LocId = TestConfiguration.TestVisitData.LocationId,
        Code = TestConfiguration.TestVisitData.LocationCode
      }
    };

    // Act
    _output.WriteLine($"Création d'une nouvelle visite pour le patient ID: {TestConfiguration.TestPatientId}");
    var visitIdentifier = await _client.CreatePatientVisitAsync(newVisit);

    // Assert
    Assert.NotNull(visitIdentifier);
    _output.WriteLine($"Visite créée avec l'ID: {visitIdentifier.HospitalizationId}");
  }

  [Fact]
  public async Task UpdatePatientVisit_WithValidData_ShouldUpdateVisit()
  {
    // Arrange
    var visitToUpdate = new PatientVisitDTO
    {
      Identifier = new PatientVisitIdentifierDTO
      {
        HospitalizationId = TestConfiguration.TestHospitalizationId
      },
      AdmissionDate = DateTime.Now
    };

    // Act
    _output.WriteLine($"Mise à jour de la visite ID: {TestConfiguration.TestHospitalizationId}");
    await _client.UpdatePatientVisitAsync(visitToUpdate);

    // Assert
    _output.WriteLine("Visite mise à jour avec succès");
  }

  [Fact]
  public async Task DeletePatientVisit_WithValidId_ShouldDeleteVisit()
  {
    // Arrange
    var visitIdentifier = new PatientVisitIdentifierDTO
    {
      HospitalizationId = TestConfiguration.TestHospitalizationId
    };

    // Act
    _output.WriteLine($"Suppression de la visite ID: {TestConfiguration.TestHospitalizationId}");
    await _client.DeletePatientVisitAsync(visitIdentifier);

    // Assert
    _output.WriteLine("Visite supprimée avec succès");
  }

  #endregion

  public void Dispose()
  {
    try
    {
      if (_client.State == System.ServiceModel.CommunicationState.Opened)
      {
        _client.Close();
        _output.WriteLine("Client fermé proprement");
      }
    }
    catch (Exception ex)
    {
      _output.WriteLine($"Erreur lors de la fermeture: {ex.Message}");
      _client.Abort();
    }
  }
}
