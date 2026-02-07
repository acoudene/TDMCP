using ModelContextProtocol.Server;
using PatientManagementService;
using System.ComponentModel;

namespace TDMCPApp.Tools;

[McpServerToolType]
public sealed class PatientManagementTool
{
  [McpServerTool, Description("Search a patient by giving search criteria like patient name, firstname, visit, demography, location, ...")]  
  public async Task<PatientDTO[]> SearchPatientAsync(PatientSearchCriteriaDTO searchCriteria)
  {
    var client = new PatientManagementServiceClient(PatientManagementServiceClient.EndpointConfiguration.WSHttpBinding_IPatientManagementService);
    return await client.SearchPatientAsync(searchCriteria);
  }
}