/* tslint:disable:max-line-length */
export type ItemTemplateModuleDefinition = { name: string, displayName: string, description: string, icon: string };

export const itemTemplateModulesDefinitions: ItemTemplateModuleDefinition[] = [
    {name: 'bourrin',      displayName: 'Arme de bourrin',          icon: 'muscle-up ', description: 'Requiert la compétence Armes de bourrin pour pouvoir utiliser cet objet'},
    {name: 'charge',       displayName: 'Charges/Utilisations',     icon: 'usable', description: 'Peut être utilisé (ex: potions, bagues)'},
    {name: 'collect',      displayName: 'Possibilités de récolte',  icon: 'sickle', description: ''},
    {name: 'container',    displayName: 'Conteneur',                icon: 'backpack', description: 'Cette objet peut contenir d\'autres objets'},
    {name: 'currency',     displayName: 'Monnaie',                  icon: 'coins', description: ''},
    {name: 'damage',       displayName: 'Dégât',                    icon: 'sword-wound', description: 'Défini les dégats de l\'arme'},
    {name: 'diceDrop',     displayName: 'Dé',                       icon: 'perspective-dice-six-faces-random', description: ''},
    {name: 'enchantment',  displayName: 'Enchantement',             icon: 'up-card', description: 'Arme magique'},
    {name: 'gem',          displayName: 'Gemme',                    icon: 'gems', description: 'Utilise le poids en U.G.'},
    {name: 'god',          displayName: 'Dieu',                     icon: 'kneeling', description: 'Lié à un dieu (restriction pour l\'utilisation)'},
    {name: 'gun',          displayName: 'Arme à poudre',            icon: 'winchester-rifle', description: 'Kaboom'},
    {name: 'itemTypes',    displayName: 'Type d\'objet',            icon: 'sword-spade', description: ''},
    {name: 'level',        displayName: 'Niveau requis',            icon: 'level-two', description: 'Niveau minimum pour utiliser'},
    {name: 'lifetime',     displayName: 'Temps de conservation',    icon: 'sands-of-time', description: ''},
    {name: 'modifiers',    displayName: 'Modificateurs',            icon: 'skills', description: ''},
    {name: 'origin',       displayName: 'Origine',                  icon: 'treasure-map', description: 'Méthode d\'obtention'},
    {name: 'prereq',       displayName: 'Prérequis',                icon: 'thumb-up', description: ''},
    {name: 'protection',   displayName: 'Protection',               icon: 'shield', description: ''},
    {name: 'quantifiable', displayName: 'Quantifiable',             icon: 'stack', description: ''},
    {name: 'relic',        displayName: 'Relique',                  icon: 'relic-blade', description: ''},
    {name: 'rarity',       displayName: 'Indice de rareté',         icon: 'chest', description: ''},
    {name: 'rupture',      displayName: 'Rupture',                  icon: 'broken-axe', description: ''},
    {name: 'sex',          displayName: 'Sexe',                     icon: 'female', description: 'Cette objet à une restriction d\'utilisation'},
    {name: 'skill',        displayName: 'Compétences',              icon: 'upgrade', description: 'Annule ou confère une compétence'},
    {name: 'skillBook',    displayName: 'Livre de compétences',     icon: 'white-book', description: 'Peut être lu pour apprendre une compétence'},
    {name: 'slots',        displayName: 'Equipement',               icon: 'battle-gear', description: 'Permet d\'équiper l\'objet'},
    {name: 'space',        displayName: 'Encombrement',             icon: 'swap-bag', description: 'Défini la place occupé'},
    {name: 'throwable',    displayName: 'Prévue pour le jet',       icon: 'throwing-ball', description: ''},
    {name: 'weight',       displayName: 'Poids',                    icon: 'weight', description: 'Défini le poids de l\'objet'}
];
