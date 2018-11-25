Feature: Job

  Scenario: Listing jobs
    When performing a GET to the url "/api/v2/jobs"
    Then the response status code be 200
    And the response should contains a json array containing the following element identified by id
    """
    {
        "id": 10,
        "name": "Mage, Sorcier",
        "internalname": null,
        "informations": "Aptitudes à la magie : peut lancer tous les sortilèges. Peut choisir une spécialité magique ou une affiliation à un dieu ou\ndémon. Peut faire une cure (payante) pour gagner des points astraux. Peut utiliser un bâton magique. Peut pratiquer des\ninvocations ou enchanter des objets.",
        "playerDescription": "Mon truc, c'est la maîtrise de la puissance astrale, et l'apprentissage de mon art. Je n'ai pas fait des études compliquées pendant des années pour me la couler douce dans un bureau ou au milieu des champs de navets ! Alors attention, ça va chauffer pour les miches des gobelins.",
        "playerSummary": "Personnage qui lance des sorts variés, qui sait parlementer, qui peut résoudre les énigmes et détecter la magie.\r\nArmes utilisables : bâton, poignard, dague, gourdin (corps à corps). Pas d'arc ou arbalètes. N’utilise pas de bouclier.\r\nPeut utiliser toute forme de magie, parchemin, protections magiques, potions, etc. Bâton non obligatoire.\r\nTransport de charge limitée, dépendant de l'origine. Armure max : PR2.",
        "maxLoad": 10,
        "maxArmorPR": 2,
        "isMagic": true,
        "baseEv": null,
        "factorEv": 0.7,
        "bonusEv": null,
        "baseEa": 30,
        "diceEaLevelUp": 6,
        "baseAT": null,
        "basePRD": null,
        "parentJobId": null,
        "flags": [
            {
                "type": "SELECT_SPECIALITY_LVL_5_10",
                "data": null
            }
        ],
        "skillIds": [
            22,
            41,
            43
        ],
        "availableSkillIds": [
            3,
            13,
            15,
            26,
            32,
            39
        ],
        "originsWhitelist": [],
        "originsBlacklist": [
            {
                "id": 1,
                "name": "Humain"
            }
        ],
        "bonuses": [],
        "requirements": [
            {
                "stat": "INT",
                "min": 12,
                "max": null
            }
        ],
        "restricts": [
            {
                "description": "ne peut utiliser autre chose que : bâton, poignard, dague, gourdin comme arme de combat rapproché",
                "flags": [
                    {
                        "type": "NO_WEAPON_TYPE",
                        "data": "EPEE"
                    },
                    {
                        "type": "NO_WEAPON_TYPE",
                        "data": "HACHE"
                    }
                ]
            },
            {
                "description": "n’utilise pas les arcs ou arbalètes",
                "flags": [
                    {
                        "type": "NO_WEAPON_TYPE",
                        "data": "ARBALETE"
                    },
                    {
                        "type": "NO_WEAPON_TYPE",
                        "data": "ARC"
                    }
                ]
            },
            {
                "description": "n’utilise pas les boucliers",
                "flags": [
                    {
                        "type": "NO_WEAPON_TYPE",
                        "data": "BOUCLIER"
                    }
                ]
            },
            {
                "description": "transport de charge limitée à 10 kilos (ou moins selon race)",
                "flags": []
            },
            {
                "description": "n’utilise pas d’armure supérieure à PR2 (sauf magique)",
                "flags": []
            }
        ],
        "specialities": [
            {
                "id": 1,
                "name": "Invocation",
                "description": "Le mage invocateur a délaissé la magie généraliste pour s'adonner à son passe-temps favori : faire apparaître dans le\nmonde des humains des créatures étranges, dangereuses et à moitié folles, dans le seul but de se faire servir et d'accéder\nrapidement à la puissance. Ce n'est pas une spécialité très populaire car elle a un côté aléatoire très prononcé. Néanmoins,\nelle peut être vraiment passionnante pour peu qu'on ait un peu d'instinct de survie. L'invocateur sait aussi lancer\nquelques sortilèges normaux, mais ça l'ennuie... Et il ne pourra utiliser qu’un seul grimoire tout au long de sa carrière !",
                "modifiers": [],
                "specials": [
                    {
                        "id": 10,
                        "isBonus": false,
                        "description": "le mage ne peut choisir aucune autre spécialisation que l'invocation – sa panoplie de sorts est réduite",
                        "flags": [
                            {
                                "type": "ONE_SPECIALITY",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 11,
                        "isBonus": false,
                        "description": "le mage doit faire attention à son charisme",
                        "flags": []
                    },
                    {
                        "id": 12,
                        "isBonus": false,
                        "description": "Le mage doit renvoyer toutes ses créatures (hors familier, compagnon ou double) avant de se reposer, ou bien elles\néchapperont à son contrôle",
                        "flags": []
                    },
                    {
                        "id": 13,
                        "isBonus": true,
                        "description": "le mage n'a pas forcément besoin d'être adroit",
                        "flags": []
                    },
                    {
                        "id": 14,
                        "isBonus": true,
                        "description": "le mage a accès à quelques sorts de magie généraliste, mais seulement ceux de son grimoire",
                        "flags": [
                            {
                                "type": "NO_GENERIC_MAGIC",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 15,
                        "isBonus": true,
                        "description": "le mage commence sa carrière avec le « Manuel de l'Invocateur Inconscient » qui lui servira jusqu'au niveau 10*",
                        "flags": []
                    },
                    {
                        "id": 16,
                        "isBonus": true,
                        "description": "le mage a accès aux sortilèges inédits réservés aux invocateurs",
                        "flags": []
                    }
                ],
                "flags": []
            },
            {
                "id": 2,
                "name": "Magie Noire de Tzinntch",
                "description": "Pour pratiquer cette discipline, il faut adhérer à la secte. En récompense de vos bons offices (et d'une contribution non\nnégligeable en monnaie), le culte vous permet d'accéder à des sortilèges de combat inédits, plus puissants et plus\néconomiques en énergie astrale. Vous avez aussi accès à du matériel exceptionnel à des prix relativement intéressants.\nC'est une carrière qui ne vous rendra pas populaire – tout comme celle de nécromant – mais qui propose une escalade\nrapide vers la puissance, comme une sorte de « côté obscur de la force », si vous voyez ce que je veux dire. Les sortilèges de\nTzinntch sont très prisés par les mages vicieux désirant s'illustrer au combat. Le gain de puissance se fait en revanche au\ndétriment d'une partie de votre santé.",
                "modifiers": [
                    {
                        "stat": "EV",
                        "type": "Add",
                        "value": -5
                    },
                    {
                        "stat": "EA",
                        "type": "Add",
                        "value": 5
                    },
                    {
                        "stat": "FO",
                        "type": "Add",
                        "value": -2
                    }
                ],
                "specials": [
                    {
                        "id": 1,
                        "isBonus": false,
                        "description": "le mage ne peut choisir aucune autre spécialisation que celle de Tzinntch",
                        "flags": [
                            {
                                "type": "ONE_SPECIALITY",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 2,
                        "isBonus": false,
                        "description": "le mage doit préserver 50% de ses gains en or, nature et joyaux pour les donner au culte",
                        "flags": []
                    },
                    {
                        "id": 3,
                        "isBonus": false,
                        "description": "le mage commence sa carrière avec -5 points d'EV et -2 en FO sur ses compétences d'origine",
                        "flags": []
                    },
                    {
                        "id": 4,
                        "isBonus": false,
                        "description": "le mage gagne 1D6-1 en EV au changement de niveau s'il choisit d'augmenter l'EV (minimum 1)",
                        "flags": [
                            {
                                "type": "LEVELUP_DICE_EV_-1",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 5,
                        "isBonus": false,
                        "description": "le mage doit obligatoirement porter la robe noire de l'ordre – il peut porter quelque chose en-dessous, coquins",
                        "flags": []
                    },
                    {
                        "id": 6,
                        "isBonus": true,
                        "description": "le mage a accès à tous les sorts de « magie généraliste », à 1 niveau au-dessus du sien",
                        "flags": [
                            {
                                "type": "GENERIC_MAGIC_+1_LEVEL",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 7,
                        "isBonus": true,
                        "description": "le mage commence sa carrière avec +5 PA",
                        "flags": []
                    },
                    {
                        "id": 8,
                        "isBonus": true,
                        "description": "le mage dépense 1 PA de moins pour chaque sort de « magie généraliste » (minimum 1)",
                        "flags": [
                            {
                                "type": "GENERIC_MAGIC_-1_MANA",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 9,
                        "isBonus": false,
                        "description": "le mage a accès aux sortilèges inédits réservés aux cultistes de Tzinntch",
                        "flags": []
                    }
                ],
                "flags": []
            },
            {
                "id": 3,
                "name": "Magie de combat",
                "description": "Premier stade de la magie dédiée à la violence, et jusqu'au niveau 14 uniquement. Plus tard, le mage de combat peut\névoluer vers la « Magie de bataille » ou même la « Magie de guerre ». On retrouve dans cette spécialité un grand nombre\nde sortilèges réservés aux aventuriers, c'est pourquoi la plupart d'entre eux choisissent cette voie. Sorts à destination d'une\ncible, sorts de masse, améliorations personnelles, la magie de combat offre un grand festival de choix variés.",
                "modifiers": [],
                "specials": [],
                "flags": []
            },
            {
                "id": 4,
                "name": "Magie du feu",
                "description": "C'est une discipline particulièrement tournée vers la violence, contrairement à ce que peuvent en dire certains professeurs\négocentriques. Le feu brûle, détruit et cause en général de gros dégâts. Il n'a aucune intelligence et se trouve rarement\nsélectif, ce qui aura des conséquences sur les dommages causés aux équipiers. Néanmoins, il existe dans la spécialité\nquelques sortilèges anodins, purement utilitaires ou simplement esthétiques. On peut même y trouver des sorts pour\namuser les amis. En situation de combat, c'est idéal pour faire de gros dégâts en consommant le moins d'énergie astrale\npossible, surtout dans les premiers niveaux. On notera que peu de créatures résistent au feu, en dehors des dragons et des\ndémons : ce n'est pas le cas pour la magie de la glace par exemple, contre laquelle on peut se prémunir avec un gros pull.\nEn revanche, à moins d'être complètement obsédé par le feu, c'est assez rapidement lassant.",
                "modifiers": [],
                "specials": [],
                "flags": []
            },
            {
                "id": 5,
                "name": "Métamorphoses",
                "description": "La métamorphose est une discipline enseignée par les Dieux du Chaos eux-mêmes. Si les elfes noirs sont passés maîtres\ndans cette spécialité, elle est accessible à toutes les races chaotiques ou assimilées, donc à presque tout le monde, pour peu\nqu’on ait un minimum de dispositions pour la magie. Cependant, malgré le côté spectaculaire de la discipline, de nombreux\nétudiants la délaissent au profit de spécialités plus efficaces au combat. Parmi les magies chaotiques, la métamorphose est\nune des moins dangereuses pour la santé mentale et consomme souvent peu d'énergie astrale.\n",
                "modifiers": [],
                "specials": [],
                "flags": []
            },
            {
                "id": 7,
                "name": "Nécromancie",
                "description": "Les mages nécromants ou nécromanciens - ou nécros’ dans le langage pauvre et imbécile des aventuriers de tous bords -\nsont des êtres sanguinaires, dangereux et irresponsables. Leurs rêves de puissance n’ont d’égal que la folie qui les anime. Et\ncomme ils sont aussi mégalomanes qu’inconscients, leurs espoirs de puissance sont souvent déçus. En outre, ils attirent les\naventuriers comme personne : Il suffit qu’un nécromant s’installe dans un donjon perdu au milieu de nulle part pour que,\nsitôt son installation connue, les bandes d’aventuriers surgissent comme des essaims d’abeilles tueuses. C'est ainsi que le\nnécromant a bien souvent des tas d'ennemis, et pas d'amis. En revanche, s'il trouve des gens assez fous pour l'accompagner\nsur les chemins de l'aventure, le nécromant saura tirer son épingle du jeu dès lors qu'il prend du galon : les sorts de bas\nniveau sont plutôt tranquilles, mais la puissance augmente rapidement. Il faut être patient cela dit car nombre d'entre eux\nsont des rituels : il faut donc dans un premier temps trouver les ingrédients, et dans un second temps savoir les utiliser.",
                "modifiers": [],
                "specials": [
                    {
                        "id": 17,
                        "isBonus": false,
                        "description": "il doit choisir cette spécialisation au niveau 1 et ne pourra en choisir d’autres par la suite",
                        "flags": [
                            {
                                "type": "ONE_SPECIALITY",
                                "data": null
                            }
                        ]
                    },
                    {
                        "id": 18,
                        "isBonus": false,
                        "description": "il doit faire attention à la présence à ces côtés de créatures mortes : celles-ci peuvent attirer l’attention",
                        "flags": []
                    },
                    {
                        "id": 19,
                        "isBonus": false,
                        "description": "il a souvent besoin de trouver des ingrédients et/ou de cadavres",
                        "flags": []
                    },
                    {
                        "id": 20,
                        "isBonus": false,
                        "description": "en voyage à pieds, il avance à la vitesse de ses créatures, soit plutôt lentement",
                        "flags": []
                    },
                    {
                        "id": 21,
                        "isBonus": true,
                        "description": "il n'a pas forcément besoin d'être adroit",
                        "flags": []
                    },
                    {
                        "id": 22,
                        "isBonus": true,
                        "description": "il peut éviter de mettre sa vie en danger en faisant travailler les morts à sa place",
                        "flags": []
                    },
                    {
                        "id": 23,
                        "isBonus": true,
                        "description": "il peut régénérer son énergie vitale et son énergie astrale sans l’aide d’équipiers",
                        "flags": []
                    },
                    {
                        "id": 24,
                        "isBonus": true,
                        "description": "il a accès au grimoire de magie généraliste à chaque niveau, en plus de son propre grimoire de nécromancien",
                        "flags": []
                    }
                ],
                "flags": []
            },
            {
                "id": 8,
                "name": "Magie de la Terre",
                "description": "La magie de la terre peut sembler ridicule aux yeux de l'ignorant. Pour certains, elle se rapproche de la béatitude\nécologique des druides et des elfes sylvains coiffant les poneys sous la lune argentée. Cependant, cette discipline n'a rien\nd'une partie de rigolade. On y trouve des sortilèges très variés : camouflage, assistance, sorts offensifs et défensifs,\ninvocations, malédictions, pièges. En définitive, une offre très complète.",
                "modifiers": [],
                "specials": [],
                "flags": []
            }
        ]
    }
    """
