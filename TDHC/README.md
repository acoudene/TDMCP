# Objectif

Fournir un exemple de manipulation de fonctionnalités TDHC à travers l'usage d'un LLM (IA).

# Fonctionnement

![LLM (IA) pilotant TDHC](LLM_IA_pilotant_TDHC.png)

# Intégration

## Claude Desktop Windows

### En stdio

Ouvrir `%AppData%\Roaming\Claude\claude_desktop_config.json`
Puis ajouter : 
```
{
    "mcpServers": {
        "TDMCP": {
          "command": "dotnet",
          "args": [
            "run",
            "--project",
            "C:\\Github\\acoudene\\TDMCP\\TDHC\\TDMCPApp"
          ]
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
