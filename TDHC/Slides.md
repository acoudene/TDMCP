# Utilisation intégrée de l'IA dans nos produits

L'objectif est d'exposer des cas d'usage possibles de nos produits, en l'état, avec pas ou peu de modifications avec un agent IA.

Nous allons découvrir 3 cas possibles :
- **Cas simple** : utilisation d'un agent IA afin de récupérer des données patients
- **Cas modéré** : utilisation d'un agent IA pour demander des informations que notre application seule ne saurait donner directement.
- **Cas complexe** : orchestration avec un agent IA impliquant la reconnaissance de texte dans un document manuscrit, dont les données sont déduites et envoyés à notre produit pour intégration.

# Pause : 4 termes autour de l'IA...

## Prompt

Un prompt est l'instruction ou la question envoyée à une IA pour lui demander d'effectuer une tâche.

C’est le message que l'utilisateur écrit. Il peut contenir :
- une demande ("Résume ce texte")
- du contexte ("Tu es un expert informatique")
- des contraintes ("Réponds en 5 points maximum")
- des données à analyser

👉 La qualité du prompt influence fortement la qualité de la réponse.

## LLM

Un LLM est un modèle d'intelligence artificielle entraîné sur de très grandes quantités de textes pour comprendre et générer du langage.

Il est capable de :
- répondre à des questions
- rédiger des textes
- résumer
- traduire
- expliquer du code

⚠️ Un LLM ne "comprend" pas comme un humain : il prédit les mots les plus probables à partir de son entraînement.

## Agent IA

Un agent IA est un système basé sur un modèle (comme un LLM) capable d’agir de manière autonome pour atteindre un objectif.

Contrairement à un simple échange question/réponse, un agent peut :
- planifier plusieurs étapes
- utiliser des outils (API, bases de données, logiciels)
- prendre des décisions intermédiaires
- enchaîner plusieurs actions

👉 Un LLM répond.

👉 Un agent agit.

## Protocole MCP

Le MCP est un protocole standard qui permet à un modèle d'IA d’accéder de manière structurée à des outils et à des sources de données externes.

Il sert d'interface entre :
- un modèle (LLM)
- des outils (CRM, base documentaire, API métier, etc.)

Il définit :
- comment le modèle demande un accès à un outil
- comment les données sont fournies
- comment les actions sont exécutées

👉 En résumé : le MCP permet à l'IA de se connecter proprement au système d'information.

## Synthèse

**Prompt** : instruction donnée à l’IA

**LLM** : moteur linguistique qui génère du texte

**Agent IA** : IA capable d’agir de manière autonome

**MCP** : protocole permettant à l’IA d’utiliser des outils externes

# Récupération de données patient par LLM

## Démo

## Principe

![LLM (IA) pilotant TDHC](LLM_IA_pilotant_TDHC.png)

# Récupération d'informations déduites par LLM

# Démo

## Principe

# Orchestration pour intégrer un document manuscrit dans notre produit

# Démo

## Principe
