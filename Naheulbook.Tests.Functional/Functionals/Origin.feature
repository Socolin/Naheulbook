Feature: Origin

  Scenario: Listing origins
    When performing a GET to the url "/api/v2/origins"
    Then the response status code is 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": 6,
        "name": "Elfe sylvain",
        "description": "<p>Les Sylvains sont les descendants des elfes ayant découvert la Terre de Fangh il y a bien longtemps. Ils ont choisi de fuir la\ncompagnie des humains pour vivre dans la forêt car ils s'y sentaient bien, et que tout le monde y est gentil. Ils ont une\nculture particulière, des styles de vie particuliers et sont enclins à la niaiserie et à l'innocence.\n</p>",
        "playerDescription": "Je suis né au milieu des arbres, et j'y ai passé la plus grande partie de mon enfance. Mes parents étaient gentils, tout comme nos amis les animaux de la forêt, et nos voisins qui cueillaient souvent des framboises avec nous. Dans ma famille on tire à l'arc, on aime les poneys et on ne fait pas de mal aux gens quand on a le choix. Nous aimons l'art, les chansons, sentir le vent dans nos cheveux propres.",
        "playerSummary": "Personnage doué en milieu naturel, dangereux sur les attaques à distance. Pas d'arme à 2 mains.\r\nTransport de charges limité à 10 kilos – Sens développés, nyctalopie totale. Armure max : PR4.",
        "maxLoad": 10,
        "maxArmorPR": 4,
        "advantage": "Peut utiliser n’importe quelle forme d'objet enchanté (et parchemin avec « érudition » s'il choisit un métier).\nExcellente vue, bon odorat, nyctalopie totale.",
        "baseEV": 25,
        "baseEA": null,
        "bonusAT": null,
        "bonusPRD": null,
        "diceEVLevelUp": 6,
        "size": "Taille humaine standard, généralement plus mince et plus élancé.",
        "flags": [],
        "speedModifier": 20,
        "skillIds": [
            15,
            36,
            39,
            47,
            48
        ],
        "availableSkillIds": [
            17,
            19,
            31,
            35,
            38,
            46
        ],
        "infos": [
            {
                "title": "Les Sylvains et les armes",
                "description": "ils sont assez évolués pour utiliser toute forme d'arme, mais leur côté précieux et fragile les\nrend quelque peu réfractaire à l'utilisation des armes à deux mains. Considérées comme des « armes de bourrin » dans toute\nla Terre de Fangh, elles donne un côté bestial et fait souvent jaillir le sang à plusieurs mètres à la ronde, ce qui peut tacher\nles vêtements. Ainsi, les Sylvains n'utilisent jamais d'armes à deux mains."
            },
            {
                "title": "Les Sylvains et l'équipement",
                "description": "d'un naturel soigneux et délicat, ce personnage ne veut pas se transformer en dromadaire. Il\nrefusera donc de porter de lourdes charges ou de s'encombrer, tout simplement parce que ce n'est pas dans sa nature. Il\nn'aimera pas non plus se trouver engoncé dans une boîte de conserve, et va préférer le port de plusieurs pièces d'armure de\ncuir ou de tissu à celui d'une pièce d'armure métallique ou renforcée."
            },
            {
                "title": "L'Elfe Sylvain et la magie",
                "description": "il n'a pas d'aptitude à la magie par manque d'imagination et d'intelligence, la plupart du temps,\nmais il n'y est pas réfractaire et trouve généralement cela merveilleux. Il est tout à fait dans le caractère d'un elfe\nd'utiliser n'importe quel objet magique, ou tout type de matériel béni pour peu que ça ne soit pas contraire à sa religion\n(dans le cas où il serait prêtre). En revanche, il ne sera capable d'utiliser les parchemins que s'il sait lire. Note : Certains\nElfes Sylvains d'ailleurs font des études particulières pour être capable de lancer eux mêmes des sortilèges, mais les gens\ns'en méfient."
            },
            {
                "title": "Les armes de jet",
                "description": "l'arme de jet de prédilection des Sylvains est l'arc. La plupart des autres armes de jet sont moches,\nencombrantes, lourdes, peu pratiques ou fabriquées par les Nains. Ils ne sont pas mauvais au poignard également."
            }
        ],
        "bonuses": [
            {
                "description": "+1 point au CHARISME au passage des niveaux 2 et 3",
                "flags": [
                    {
                        "type": "CHA_+1_LVL2_LVL3",
                        "data": null
                    }
                ]
            }
        ],
        "requirements": [
            {
                "stat": "CHA",
                "min": 12,
                "max": null
            },
            {
                "stat": "AD",
                "min": 10,
                "max": null
            },
            {
                "stat": "FO",
                "min": null,
                "max": 11
            }
        ],
        "restricts": [
            {
                "description": "pas d’aptitudes à la magie (métier de mage ou prêtrise)",
                "flags": [
                    {
                        "type": "NO_MAGIC",
                        "data": null
                    }
                ]
            },
            {
                "description": "ne peut utiliser les armes à 2 mains",
                "flags": [
                    {
                        "data": "ARME_BOURRIN",
                        "type": "NO_SKILL"
                    }
                ]
            },
            {
                "description": "transport de charge limitée à 10 kilos",
                "flags": []
            },
            {
                "description": "n’utilise pas d’armure supérieure à PR4 (sauf magique)",
                "flags": []
            }
        ]
    }
    """
