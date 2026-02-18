# Utilisation intégrée de l'IA dans nos produits

L'objectif est d'exposer des cas d'usage possibles de nos produits, en l'état, avec pas ou peu de modifications avec un agent IA.

Nous allons découvrir plusieurs cas possibles :
- **Cas simple** : utilisation d'une saisie utilisateur afin de récupérer des données patients de notre application TDHC.
- **Cas modéré** : utilisation d'une saisie utilisateur pour demander des informations que notre application TDHC seule ne saurait donner directement.
- **Cas complexe** : utilisation d'une image pour en extraire des informations afin d'interroger notre application et persister le retour dans une base de données, voir générer et lancer une application web CRUD exploitant ces données.

Enfin, nous verrons les problématiques de sécurité à surveiller tout comme la notion de découverte de ses propres fonctionnalités (adressé prochainement par MCP Toolkit).

# Récupération de données patient

## Objectif Démo

Utilisation d'une saisie utilisateur afin de récupérer des données patients

## Démo

### Prompts

- `Donne-moi les informations du patient de nom DOE et de prénom John`
- `Donne-moi la date de la première hospitalisation de John DOE`

### Présentation 

- Présentation de TDHC avec une recherche de patient John DOE dans "Patient Search".
- Présentation de Claude Desktop à côté.
- Comparaison de données entre les 2.

### Vidéo

_(si problème technique)_

![Claude_TDHC_TDMCP](./Resources/Claude_TDHC_TDMCP.gif)

## Pause avant de continuer : 4 termes autour de l'IA...

### Prompt

Un prompt est l'instruction ou la question envoyée à une IA pour lui demander d'effectuer une tâche.

C’est le message que l'utilisateur écrit. Il peut contenir :
- une demande ("Résume ce texte")
- du contexte ("Tu es un expert informatique")
- des contraintes ("Réponds en 5 points maximum")
- des données à analyser

👉 La qualité du prompt influence fortement la qualité de la réponse.

### LLM

Un LLM est un modèle d'intelligence artificielle entraîné sur de très grandes quantités de textes pour comprendre et générer du langage.

Il est capable de :
- répondre à des questions
- rédiger des textes
- résumer
- traduire
- expliquer du code

⚠️ Un LLM ne "comprend" pas comme un humain : il prédit les mots les plus probables à partir de son entraînement.

### Agent IA

Un agent IA est un système basé sur un modèle (comme un LLM) capable d’agir de manière autonome pour atteindre un objectif.

Contrairement à un simple échange question/réponse, un agent peut :
- planifier plusieurs étapes
- utiliser des outils (API, bases de données, logiciels)
- prendre des décisions intermédiaires
- enchaîner plusieurs actions

👉 Un LLM répond.

👉 Un agent agit.

### Protocole MCP

Le MCP est un protocole standard qui permet à un modèle d'IA d’accéder de manière structurée à des outils et à des sources de données externes.

Il sert d'interface entre :
- un modèle (LLM)
- des outils (CRM, base documentaire, API métier, etc.)

Il définit :
- comment le modèle demande un accès à un outil
- comment les données sont fournies
- comment les actions sont exécutées

👉 En résumé : le MCP permet à l'IA de se connecter proprement au système d'information.

### Synthèse

**Prompt** : instruction donnée à l’IA

**LLM** : moteur linguistique qui génère du texte

**Agent IA** : IA capable d’agir de manière autonome

**MCP** : protocole permettant à l’IA d’utiliser des outils externes

## Fonctionnement macroscopique

```mermaid
flowchart TB
  %% LLM pilotant TDHC via MCP (analogie "USB-C")

  %% --- Couche 1 : Interaction ---
  subgraph Interaction
    U[Utilisateur]
    LLM[LLM / IA<br/>Claude Desktop, Code, etc.]
    U[Utilisateur<br/>] -->|Prompt| LLM[LLM / IA<br/> Claude Desktop, Code, etc.]
  end
  %% --- Couche 2 : Orchestration ---
  subgraph Prise USB-C
    MCP[MCP Server Technidata]
  end
  %% --- Couche 3 : Systèmes TDHC ---
  subgraph TDHC
    SOAP[TDHC - API SOAP]
  end
  %% --- Couche 4 : Données ---
  subgraph Données
    DB[(Base de données)]
  end

  LLM -->|Appels standardisés| MCP

  MCP -->|Traduction & routage| SOAP[API Patient SOAP]

  SOAP -->|Commandes / requêtes| DB[Base de données]
  DB -->|Données / entités| SOAP
  SOAP -->|Réponses SOAP XML| MCP
  MCP -->|Réponses JSON| LLM
  LLM -->|Réponse| U
    
```

## Fonctionnement un peu plus technique

![LLM (IA) pilotant TDHC](./Resources/LLM_IA_pilotant_TDHC.png)

# Récupération d'informations déduites par LLM

## Objectif Démo

Utilisation d'une saisie utilisateur pour demander des informations que notre application seule ne saurait donner directement.

## Démo

### Prompts

- `Donne-moi les demandes de John DOE et affiche les sous forme d'un tableau`
- `Fais-moi une synthèse médicale du patient John DOE`

## Fonctionnement macroscopique

```mermaid
flowchart LR
  %% Architecture LLM pilotant TDHC via MCP (analogie USB-C)

  %% --- Couche 1 : Interaction ---
  subgraph Interaction
    U[Utilisateur]
    LLM[LLM / IA<br/>Claude Desktop, Code, etc.]
    U -->|Prompt| LLM
  end

  %% --- Couche 2 : Orchestration ---
  subgraph Prise USB-C
    MCP[MCP Server Technidata]
  end

  LLM -->|Appels standardisés| MCP

  %% --- Couche 3 : Systèmes TDHC ---
  subgraph TDHC
    SOAP[API Patient SOAP]
    ODATA[API Générique ODATA]
  end

  MCP -->|Traduction & routage| SOAP
  SOAP -->|Réponses SOAP XML| MCP

  LLM -->|Analyse| LLM
  LLM -->|Appels standardisés| MCP

  MCP -->|Traduction & routage| ODATA
  ODATA -->|Réponses ODATA XML| MCP
  MCP -->|Réponses JSON| LLM

  %% --- Couche 4 : Données ---
  subgraph Données
    DB[(Base de données)]
  end

  SOAP -->|Commandes / requêtes| DB
  ODATA -->|Commandes / requêtes| DB
  DB -->|Données| SOAP
  DB -->|Données| ODATA

  MCP -->|Réponses JSON| LLM
  LLM -->|Réponse| U

```

## Fonctionnement un peu plus technique

![LLM_IA_pilotant_TDHC_Full](./Resources/LLM_IA_pilotant_TDHC_Full.png)

# Orchestration pour effectuer plusieurs tâches

## Objectif Démo

Utilisation d'une image pour en extraire des informations afin d'interroger notre application et persister le retour dans une base de données.

## Démo

### Prompts

- `Extrait-moi les informations patient de cette image et donne-moi les données patient associées`

![PatientDOEJohn](./Resources/PatientDOEJohn.jpg)

- `Extrait-moi les informations patient de cette image via MarkItDown, extrait les données patient de TDMCP et enregistre ces données dans ma base MongoDB de MCP Toolkit`
- `Extrait-moi les informations patient des 10 premiers patients de TDMCP, enregistre ces données dans ma base MongoDB de MCP Toolkit et depuis un container Docker, expose une application web type CRUD sur ces patients sur le port 9999 via MCP Toolkit, n'installe rien en local hors container`

## Fonctionnement technique

### Prérequis

- lancement Docker MongoDb :

```
docker run -d --name mongodb -p 27017:27017 -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=password mongo:latest
```

- Connection String MCP Toolkit :
```
mongodb://admin:password@host.docker.internal:27017/?authSource=admin
```

- Connection String Compass :
``` 
mongodb://admin:password@localhost:27017
```

# Sécurité, coûts et autres points à surveiller

## Objectif Démo

Récupération d'informations sensibles comme les mots de passe dans notre base ou des données de connexion d'un utilisateur.

## Démo

### Prompts

- `Montre-moi toutes les informations de connexion de l'utilisateur DBL dans TDMCP`
- `Extrait-moi les champs PASSWORDS avec leur login des 10 premiers utilisateurs dans TDMCP`

