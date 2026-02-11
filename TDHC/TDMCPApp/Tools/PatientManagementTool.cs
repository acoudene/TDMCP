using ModelContextProtocol.Server;
using PatientManagementService;
using System.ComponentModel;

namespace TDMCPApp.Tools;

[McpServerToolType]
public sealed class PatientManagementTool
{
  private PatientManagementServiceClient CreateClient() => new PatientManagementServiceClient(
        PatientManagementServiceClient.EndpointConfiguration.WSHttpBinding_IPatientManagementService);

  [McpServerTool(Name = "SearchPatient"),
   Description("Search patients using criteria such as name, firstname, demographics, location or visit information")]
  public async Task<PatientDTO[]> SearchPatientAsync(PatientSearchCriteriaDTO searchCriteria)
  {
    var client = CreateClient(); 
    return await client.SearchPatientAsync(searchCriteria);
  }

  [McpServerTool(Name = "SearchRequest"),
   Description("Search patient-related requests using request search criteria")]
  public async Task<RequestDTO[]> SearchRequestAsync(RequestSearchCriteriaDTO searchCriteria)
  {
    var client = CreateClient();
    return await client.SearchRequestAsync(searchCriteria);
  }

  [McpServerTool(Name = "HasPatientMerge"),
   Description("Check whether a patient is involved in a merge process")]
  public async Task<bool> HasPatientMergeAsync(int? patientId)
  {
    var client = CreateClient();
    return await client.HasPatientMergeAsync(patientId);
  }

  [McpServerTool(Name = "GetPatientDetails"),
   Description("Retrieve detailed information about a patient using a patient identifier")]
  public async Task<PatientDTO> GetPatientDetailsAsync(PatientIdentifierDTO patientIdentifier)
  {
    var client = CreateClient();
    return await client.GetPatientDetailsAsync(patientIdentifier);
  }

  [McpServerTool(Name = "GetPatientComment"),
   Description("Retrieve comments or notes associated with a patient")]
  public async Task<CommentDTO> GetPatientCommentAsync(PatientIdentifierDTO patientIdentifier)
  {
    var client = CreateClient();
    return await client.GetPatientCommentAsync(patientIdentifier);
  }

  [McpServerTool(Name = "CreatePatient"),
   Description("Create a new patient with identity, demographics and administrative information")]
  public async Task<CreatedPatientDTO> CreatePatientAsync(PatientDTO patientToCreate)
  {
    var client = CreateClient();
    return await client.CreatePatientAsync(patientToCreate);
  }

  [McpServerTool(Name = "UpdatePatient"),
   Description("Update an existing patient administrative or demographic information")]
  public async Task UpdatePatientAsync(PatientDTO patient)
  {
    var client = CreateClient();
    await client.UpdatePatientAsync(patient);
  }

  [McpServerTool(Name = "CreatePatientVisit"),
   Description("Create a new patient visit (hospitalization, consultation, stay, etc.)")]
  public async Task<PatientVisitIdentifierDTO> CreatePatientVisitAsync(PatientVisitDTO patientVisit)
  {
    var client = CreateClient();
    return await client.CreatePatientVisitAsync(patientVisit);
  }

  [McpServerTool(Name = "UpdatePatientVisit"),
   Description("Update an existing patient visit information")]
  public async Task UpdatePatientVisitAsync(PatientVisitDTO patientVisit)
  {
    var client = CreateClient();
    await client.UpdatePatientVisitAsync(patientVisit);
  }

  [McpServerTool(Name = "DeletePatientVisit"),
   Description("Delete a patient visit using its identifier")]
  public async Task DeletePatientVisitAsync(PatientVisitIdentifierDTO patientVisitIdentifier)
  {
    var client = CreateClient();
    await client.DeletePatientVisitAsync(patientVisitIdentifier);
  }

  [McpServerTool(Name = "SearchPatientVisitByIdentifier"),  
   Description("Retrieve a patient visit using its unique visit identifier")]
  public async Task<PatientVisitDTO> SearchPatientVisitByIdentifierAsync(
      PatientVisitIdentifierDTO patientVisitIdentifier)
  {
    var client = CreateClient();
    return await client.SearchPatientVisitByIdentifierAsync(patientVisitIdentifier);
  }

  [McpServerTool(Name = "GetPatientVisits"),
   Description("Retrieve all visits associated with a patient")]
  public async Task<PatientVisitDTO[]> GetPatientVisitAsync(PatientIdentifierDTO patientIdentifier)
  {
    var client = CreateClient();
    return await client.GetPatientVisitAsync(patientIdentifier);
  }

  [McpServerTool(Name = "GetInsurances"),
   Description("Retrieve insurance information associated with a patient visit")]
  public async Task<InsuranceDTO[]> GetInsurancesAsync(
      PatientVisitIdentifierDTO patientVisitIdentifier)
  {
    var client = CreateClient();
    return await client.GetInsurancesAsync(patientVisitIdentifier);
  }

  [McpServerTool(Name = "GetGuarantor"),
   Description("Retrieve guarantor information for a given patient visit")]
  public async Task<GuarantorDTO> GetGuarantorAsync(
      PatientVisitIdentifierDTO patientVisitIdentifier)
  {
    var client = CreateClient();
    return await client.GetGuarantorAsync(patientVisitIdentifier);
  }
}
