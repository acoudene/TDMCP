# Démo TDMCP - CRUD Patients

Effectue une démo complète du système CRUD pour les patients TDMCP:

1. Extrais les 20 premiers patients de TDMCP via l'API OData (utilise mcp__TDMCP_Claude_Code__query sur l'entité PATIENTS avec top=20)

2. Insère ces patients dans MongoDB MCP Toolkit:
   - Base: healthcare
   - Collection: patients
   - Utilise mcp__MCP_DOCKER__create-collection puis mcp__MCP_DOCKER__insert-many

3. Crée une application web CRUD Dockerisée sur le port 9999:
   - Crée un dossier patient-crud-docker/
   - Fichiers à créer:
     * package.json (express + mongodb dependencies - version simplifiée sans MongoDB si problèmes d'auth)
     * server.js (API REST Express avec stockage fichier JSON)
     * index.html (interface web moderne avec CRUD complet)
     * patients-data.json (données initiales des 20 patients extraits)
     * Dockerfile (Node 18 Alpine)
   - Build l'image Docker: docker build -t patient-crud-app .
   - Lance le conteneur: docker run -d --name patient-crud-container -p 9999:9999 patient-crud-app
   - Vérifie les logs: docker logs patient-crud-container
   - Teste l'API: curl http://localhost:9999/api/patients

IMPORTANT:
- NE PAS installer de dépendances localement (npm install local)
- Tout doit être dans le conteneur Docker
- Si MongoDB nécessite authentification, utilise un stockage fichier JSON simple
- Crée TOUS les fichiers nécessaires en une seule fois
- Build et lance le conteneur directement
- Affiche l'URL finale: http://localhost:9999
