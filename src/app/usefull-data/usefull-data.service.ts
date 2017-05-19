import {Injectable} from '@angular/core';
import {CriticalData} from './usefull-data.model';

@Injectable()
export class UsefullDataService {

    public criticData = {
        'sharp': [
            {
                dice: [1, 2],
                effect: 'Incision profonde',
                effect2: 'Degâts +1'
            },
            {
                dice: [3, 4],
                effect: 'Incision vraiment profonde',
                effect2: 'Degâts +2'
            },
            {
                dice: [5, 6],
                effect: 'Plaie impressionnante et dommages à l\'armure',
                effect2: 'Degâts +3 pièce d\'armure PR -1'
            },
            {
                dice: [7, 8],
                effect: 'Coup précis et dommages à l\'armure',
                effect2: 'Degâts +4 pièce d\'armure PR -2'
            },
            {
                dice: [9, 10],
                effect: 'Coup précis de bourrin et dommages à l\'armure',
                effect2: 'Degâts +5 pièce d\'armure PR -3'
            },
            {
                dice: [11],
                effect: 'Une partie de l\'armure est détruite Pièce d\'armure concernée',
                effect2: 'PR=O'
            },
            {
                dice: [12],
                effect: 'Œil crevé',
                effect2: 'Dégâts +5 et modifs*'
            },
            {
                dice: [13],
                effect: 'Main tranchée droite (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +6 et modifs*'
            },
            {
                dice: [14],
                effect: 'Pied tranché droit (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +6 et modifs*'
            },
            {
                dice: [15],
                effect: 'Bras tranché droit (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +7 et modifs*'
            },
            {
                dice: [16],
                effect: 'Jambe tranchée droite (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +8 et modifs*'
            },
            {
                dice: [17],
                effect: 'Organes génitaux endommagés',
                effect2: 'Dégâts +5 et durée**'
            },
            {
                dice: [18],
                effect: 'Organe vital endommagé',
                effect2: 'Dégâts +9 et durée**'
            },
            {
                dice: [19],
                effect: 'Blessure au coeur',
                effect2: 'Décès'
            },
            {
                dice: [20],
                effect: 'Blessure grave à la tête',
                effect2: 'Décès'
            },
        ],
        'blunt': [
            {
                dice: [1, 2],
                effect: 'Ématome impressionnant',
                effect2: 'Degâts +1'
            },
            {
                dice: [3, 4],
                effect: 'Ématome et luxation d\'un membre',
                effect2: 'Degâts +2'
            },
            {
                dice: [5, 6],
                effect: 'Fracture de côte',
                effect2: 'Degâts +3'
            },
            {
                dice: [7, 8],
                effect: 'Coup précis et dommages à l\'armure',
                effect2: 'Degâts +4 pièce d\'armure PR -1'
            },
            {
                dice: [9, 10],
                effect: 'Coup précis de bourrin et dommages à l\'armure',
                effect2: 'Degâts +5 pièce d\'armure PR -2'
            },
            {
                dice: [11],
                effect: 'Une partie de l\'armure est détruite',
                effect2: 'Pièce d\'armure concernée PR=O'
            },
            {
                dice: [12],
                effect: 'Luxation du genou',
                effect2: 'Dégâts +3 et modifs*'
            },
            {
                dice: [13],
                effect: 'Main cassée droite (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +3 et modifs*'
            },
            {
                dice: [14],
                effect: 'Pied écrasé droit (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +3 et modifs*'
            },
            {
                dice: [15],
                effect: 'Bras cassé droit (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +4 et modifs*'
            },
            {
                dice: [16],
                effect: 'Jambe cassée droite (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +5 et modifs*'
            },
            {
                dice: [17],
                effect: 'Organes génitaux écrasés',
                effect2: 'Dégâts +5 et modifs*'
            },
            {
                dice: [18],
                effect: 'Assommé',
                effect2: ' Inconscient'
            },
            {
                dice: [19],
                effect: 'Fracture ouverte du crâne',
                effect2: 'Décès'
            },
            {
                dice: [20],
                effect: 'Ecrasement violent d\'un organe vital',
                effect2: 'Décès'
            }
        ],
        'parade': [
            {
                dice: [1, 2],
                effect: 'Mais non en fait : la parade était normale',
                effect2: 'Résolution normale de parade'
            },
            {
                dice: [3, 5],
                effect: 'L\'ennemi est repoussé',
                effect2: 'Il rate un assaut'
            },
            {
                dice: [6, 7],
                effect: 'L\'ennemi trébuche',
                effect2: 'Il subit 1 attaque imparable'
            },
            {
                dice: [8, 9],
                effect: 'L\'ennemi tombe',
                effect2: 'Il subit 2 attaques imparables'
            },
            {
                dice: [10, 12],
                effect: 'L\'ennemi lâche son arme à 1 D6 mètres',
                effect2: 'Il combat à mains nues'
            },
            {
                dice: [13, 15],
                effect: 'L\'ennemi casse son arme',
                effect2: 'Il combat à mains nues'
            },
            {
                dice: [16, 18],
                effect: 'L\'ennemi reçoit un coup de votre arme',
                effect2: 'Il subit des dégâts'
            },
            {
                dice: [19],
                effect: 'L\'ennemi subit un coup critique avec votre arme',
                effect2: 'Il subit des dégâts : table critique'
            },
            {
                dice: [20],
                effect: 'L\'ennemi, ce loser, subit un coup critique avec son arme',
                effect2: 'II subit des dégâts : table critique'
            }
        ],
        'hand': [
            {
                dice: [1, 2],
                effect: 'Ématome impressionnant',
                effect2: 'Degâts +1'
            },
            {
                dice: [3, 4],
                effect: 'Ématome et luxation d\'un membre',
                effect2: 'Degâts +2'
            },
            {
                dice: [5, 6],
                effect: 'Fracture du nez, hémorragie nasale',
                effect2: 'Degâts +3'
            },
            {
                dice: [7, 8],
                effect: 'Fracture de côte',
                effect2: 'Degâts +4'
            },
            {
                dice: [9, 10],
                effect: 'Coup précis de bourrin dans la tempe',
                effect2: 'Degâts +2, cible étourdie 3 assauts'
            },
            {
                dice: [11],
                effect: 'Coup précis au sternum',
                effect2: 'Cible étourdie 4 assauts'
            },
            {
                dice: [12],
                effect: 'Luxation du genou',
                effect2: 'Dégâts +3 et modifs*'
            },
            {
                dice: [13],
                effect: 'Main cassée droite (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +3 et modifs*'
            },
            {
                dice: [14],
                effect: 'Mâchoire brisée',
                effect2: 'Dégâts +3, incapacité à parler'
            },
            {
                dice: [15],
                effect: 'Bras cassé droit (au D6 - 1-3) ou gauche (au D6 - 4-6)',
                effect2: 'Dégâts +4 et modifs*'
            },
            {
                dice: [16, 17],
                effect: 'Organes génitaux écrasés, houlala',
                effect2: 'Dégâts +5 et modifs*'
            },
            {
                dice: [18, 19],
                effect: 'Assommé',
                effect2: 'Inconscient'
            },
            {
                dice: [20],
                effect: 'Enfoncement du nez dans le cerveau',
                effect2: 'Décès'
            }
        ],
        'projectile': [
            {
                dice: [1, 2],
                effect: 'Projectile bien placé',
                effect2: 'Degâts +1'
            },
            {
                dice: [3, 4],
                effect: 'Projectile magistralement placé',
                effect2: 'Degâts +2'
            },
            {
                dice: [5, 6],
                effect: 'Projectile dans une articulation du bras',
                effect2: 'Degâts +3, AT/PRD-2'
            },
            {
                dice: [7, 8],
                effect: 'Projectile dans une articulation de la jambe',
                effect2: 'Degâts +3, MV-50%'
            },
            {
                dice: [9, 11],
                effect: 'Projectile dans un poumon',
                effect2: 'Degâts +3, toutes caracs -1'
            },
            {
                dice: [12, 16],
                effect: 'Projectile de maître',
                effect2: 'Dégâts +5'
            },
            {
                dice: [17],
                effect: 'En plein dans les organes génitaux, bien fait !',
                effect2: 'Dégâts +5 et durée**'
            },
            {
                dice: [18],
                effect: 'Organe vital endommagé',
                effect2: 'Dégâts +6 et durée**'
            },
            {
                dice: [19],
                effect: 'Coeur transpercé',
                effect2: 'Décès'
            },
            {
                dice: [20],
                effect: 'Oeil et cerveau transpercé, bien fait !',
                effect2: 'Décès'
            }
        ]
    };

    public getCriticalData(): {[name: string]: CriticalData[]} {
        return this.criticData;
    }
}
