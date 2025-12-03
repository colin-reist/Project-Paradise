# Guide de Style pour le Projet Paradise

Ce document définit les conventions de codage à respecter pour assurer la cohérence et la lisibilité du code.

## 1. Conventions de Nommage (C#)

- **Classes, Enums, Interfaces** : `PascalCase`
  - *Exemple : `PlayerMovement`, `GameState`*

- **Méthodes publiques et Propriétés** : `PascalCase`
  - *Exemple : `HandleFlip()`, `MoveSpeed`*

- **Champs privés** : `_camelCase` (commençant par un underscore)
  - *Exemple : `_rb`, `_moveInput`*

- **Variables locales et paramètres de méthode** : `camelCase`
  - *Exemple : `var theScale = transform.localScale;`*

- **Constantes** : `PascalCase` ou `ALL_CAPS_SNAKE_CASE`
  - *Exemple : `MaxHealth`, `DEFAULT_SPEED`*

## 2. Formatage

- **Accolades** : Les accolades `{` se placent sur la ligne suivante de la déclaration (style Allman).
- **Indentation** : Utilisez 4 espaces pour l'indentation (pas de tabulations).
- **Directives `using`** : Placez les directives `using` à l'extérieur de la déclaration de `namespace`.

## 3. Commentaires

- Utilisez des commentaires XML `///` pour toutes les méthodes et propriétés publiques.
- Évitez les commentaires qui expliquent *ce que* fait le code. Commentez plutôt *pourquoi* le code est là.
