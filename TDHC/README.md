# Objectif

Fournir un exemple de manipulation de fonctionnalités TDHC à travers l'usage d'un LLM (IA).

# Fonctionnement

![LLM (IA) pilotant TDHC](./Resources/LLM_IA_pilotant_TDHC.png)

# Intégration

## Claude Desktop Windows

### En stdio

Ouvrir `%AppData%\Roaming\Claude\claude_desktop_config.json`
Puis ajouter (la partie à l'intérieur de "mcpServers") : 
```
{
    "mcpServers": {
        "TDMCP": {
          "command": "C:\\Users\\ACE\\OneDrive - technidata-web.com\\Desktop\\TDMCP\\TDMCPApp.exe"
        }
    }
}
```

## Claude Code

En SSE :

```powershell 
claude mcp add --transport sse tdmcp-sse https://localhost:7228/mcp/sse
```

Vérification :

```
curl -vk https://localhost:7228/mcp/sse
```

### Stdio

Editer : `C:\Users\ACE\.claude.json`

Ajouter par exemple (la partie à l'intérieur de "mcpServers") :

```
"mcpServers": {
    "TDMCP_Claude_Code": {
      "command": "C:\\Users\\ACE\\OneDrive - technidata-web.com\\Desktop\\TDMCP_Claude_Code\\TDMCPApp.exe",
      "args": [
        "--urls http://*:8080;https://*:8443"
      ],
      "env": {
        "ASPNETCORE_URLS": "http://*:8080;https://*:8443"
      }
    }
}
```

# Développement

## MCP Inspector

### Standalone

```
npx @modelcontextprotocol/inspector
```

### En lançant l'application dotnet pour faire le lien

```
npx @modelcontextprotocol/inspector dotnet run --launch-profile "http"
```

# MCP Toolkit

## Catalogue privé


### Création catalogue

```
docker mcp catalog create anthony_catalog
```

### Import du serveur MCP

```yaml
docker mcp catalog import ./TDMCPApp.yaml
```

### Activation du serveur MCP

```
docker mcp server enable TDMCPApp
```

### Consulter le contenu du catalogue

```
docker mcp catalog show anthony_catalog
```

### Lister les serveurs MCP

```
docker mcp server list
```

# Cas serveur WCF Data Services OData V3

```
dotnet add package Microsoft.OData.Client --version 7.*
```


## A voir

### Génération avec Simple.OData.Client

A documenter.

### Génération avec Microsoft.OData.Client

Note : installation .Net 6 peut-être nécessaire... En plus des SDKs déjà installés.

```
dotnet tool install --global Microsoft.OData.Cli
```

```
odata-cli generate --metadataUri https://tdhc-app-dev-2.technidata.net:38431/TDHC9XACE/Technical/TDDatabaseService.svc/$metadata --outputDir ODataClient  --namespace TDDatabaseService.OData
```