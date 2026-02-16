# Utilisation intégrée de l'IA dans nos produits

L'objectif est d'exposer des cas d'usage possibles de nos produits, en l'état, avec pas ou peu de modifications avec un agent IA.

Nous allons découvrir 3 cas possibles :
- **Cas simple** : utilisation d'une saisie afin de récupérer des données patients
- **Cas modéré** : utilisation d'une saisie pour demander des informations que notre application seule ne saurait donner directement.
- **Cas complexe** : utilisation d'une image pour en extraire des informations afin d'interroger notre application et persister le retour dans une base de données.

Enfin, nous verrons les problématiques de sécurité à surveiller tout comme la notion de découverte de ses propres fonctionnalités (adressé prochainement par MCP Toolkit).

# Récupération de données patient

## Objectif Démo

Utilisation d'une saisie afin de récupérer des données patients

## Démo

- Prompt : _"Donne-moi les informations du patient de nom DOE et de prénom John"_
- Présentation de TDHC avec une recherche de patient John DOE dans "Patient Search".
- Présentation de Claude Desktop à côté.
- Comparaison de données entre les 2.

Vidéo secours (si problème technique) :

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

## Fonctionnement technique

![LLM (IA) pilotant TDHC](./Resources/LLM_IA_pilotant_TDHC.png)

# Récupération d'informations déduites par LLM

## Objectif Démo

Utilisation d'une saisie pour demander des informations que notre application seule ne saurait donner directement.

## Démo

## Fonctionnement technique

# Orchestration pour effectuer plusieurs tâches

## Objectif Démo

Utilisation d'une image pour en extraire des informations afin d'interroger notre application et persister le retour dans une base de données.

## Démo

![PatientDOEJohn](./Resources/PatientDOEJohn.jpg)

## Fonctionnement technique

# Sécurité, coûts et autres points à surveiller

## Objectif Démo

Récupération d'informations sensibles comme les mots de passe dans notre base ou des données de connexion d'un utilisateur.

## Démo

