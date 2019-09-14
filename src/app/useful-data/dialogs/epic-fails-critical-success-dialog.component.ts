import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';

import {CriticalData, PanelNames} from '../useful-data.model';
import {UsefulDataService} from '../useful-data.service';
import {UsefulDataDialogResult} from './useful-data-dialog-result';

export interface EpicFailsCriticalSuccessDialogData {
    mode: 'epicFails' | 'criticalSuccess';
}

@Component({
    templateUrl: './epic-fails-critical-success-dialog.component.html',
    styleUrls: ['../../shared/full-screen-dialog.scss', './epic-fails-critical-success-dialog.component.scss']
})
export class EpicFailsCriticalSuccessDialogComponent implements OnInit {
    public criticalData: { [name: string]: CriticalData[] };
    public selectedIndex = 0;
    public isMobile = false;
    public tabs: {
        icon: string,
        title: string,
        labels: string[],
        dataSet: string,
        header: {
            title: string,
            details: string,
            description?: string
        },
        notes: string[],
        showInjuryLink?: boolean,
        showEntropicSpellsLink?: boolean
    }[];

    private epicFailsTabs = [
        {
            icon: 'broad-dagger',
            title: 'Attaque ou parade',
            labels: ['D20', 'Maladresse du combattant', 'Effets'],
            dataSet: 'combat',
            header: {
                title: 'Maladresse attaque-parade',
                details: '20 au jet d\'ATTAQUE ou PARADE'
            },
            notes: [
                `* Tirer 1 D6 pour savoir si l'arme résiste au choc — voir point de rup ure dans le tableau de l'armement`,
                ``
            ],
            showInjuryLink: true
        },
        {
            icon: 'thunder-struck',
            title: 'Lancement du sort',
            labels: ['D20', 'Effets sur le sortilège', 'Effets'],
            dataSet: 'magic',
            header: {
                title: 'Maladresse du sortilège',
                details: '20 à l\'épreuve au lancement du sort'
            },
            notes: [
                `*Le sortilège devient fou et frappe tout le monde dans une zone énorme`,
                `** Un sort surgit de nulle part, et qui peut être n'importe quoi - le MJ décide`
            ],
            showEntropicSpellsLink: true
        },
        {
            icon: 'pierced-heart',
            title: 'Arme de jet',
            labels: ['D20', 'Maladresse du combattant', 'Effets'],
            dataSet: 'arrow',
            header: {
                title: 'Maladresse à l\'arme de jet',
                details: '20 au jet d\'ADRESSE'
            },
            notes: [
                `* Tirer 1 D6 pour savoir si l'arme résiste au choc — voir point de rup ure dans le tableau de l'armement`
            ],
            showInjuryLink: true
        },
        {
            icon: 'fist',
            title: 'Mains nues',
            labels: ['D20', 'Maladresse du combattant', 'Effets'],
            dataSet: 'hand',
            header: {
                title: 'Maladresse à mains nues',
                details: '20 au jet d\'ATTAQUE'
            },
            notes: [],
            showInjuryLink: true
        }
    ];

    private criticalSuccessTabs = [
        {
            icon: 'broad-dagger',
            title: 'Arme tranchante',
            labels: ['D20', 'Effet sur l\'ennemi', 'Modif. dégâts'],
            dataSet: 'sharp',
            header: {
                title: 'Attaque critique',
                details: '1 au jet d\'ATTAQUE',
                description: 'Epées, lances, lames diverses, haches...'
            },
            notes: [
                `** Hémorragie, dégâts sur la durée : la victime perd 1 D6 PV chaque minute jusqu'à ce qu'on la soigne`,
            ],
            showInjuryLink: true
        },
        {
            icon: 'thor-hammer',
            title: 'Arme contondante',
            labels: ['D20', 'Effet sur l\'ennemi', 'Modif. dégâts'],
            dataSet: 'blunt',
            header: {
                title: 'Attaque critique',
                details: '1 au jet d\'ATTAQUE'
            },
            notes: [],
            showInjuryLink: true
        },
        {
            icon: 'shield-reflect',
            title: 'Parade',
            labels: ['D20', 'Effet sur l\'ennemi', 'Effet sur l\'attaquant'],
            dataSet: 'parade',
            header: {
                title: 'Parade critique',
                details: '1 ou 2 au jet de PARADE'
            },
            notes: [
                `* Tirer 1 D6 pour savoir si l'arme résiste au choc — voir point de rup ure dans le tableau de l'armement`
            ]
        },
        {
            icon: 'punch',
            title: 'Mains nues',
            labels: ['D20', 'Effet sur l\'ennemi', 'Modif. dégâts'],
            dataSet: 'hand',
            header: {
                title: 'Attaque critique',
                details: '1 au jet d\'ATTAQUE',
                description: 'A utiliser si jamais le héros s\'engage dans un combat à mains nues'
            },
            notes: [],
            showInjuryLink: true
        },
        {
            icon: 'high-shot',
            title: 'Projectiles',
            labels: ['D20', 'Effet sur l\'ennemi', 'Modif. dégâts'],
            dataSet: 'projectile',
            header: {
                title: 'Attaque critique',
                details: '1 au jet d\'ATTAQUE',
                description: `Projectiles : flèches, carreaux, javelots, poignards, dagues...`
            },
            notes: [
                `** Hémorragie, dégâts sur la durée : la victime perd 1 D6 PV chaque minute jusqu'à ce qu'on la soigne`
            ],
            showInjuryLink: true
        }
    ];

    constructor(
        private dialogRef: MatDialogRef<EpicFailsCriticalSuccessDialogComponent, UsefulDataDialogResult>,
        public usefulDataService: UsefulDataService,
        @Inject(MAT_DIALOG_DATA) public data: EpicFailsCriticalSuccessDialogData,
        public breakpointObserver: BreakpointObserver
    ) {
        breakpointObserver.observe([
            Breakpoints.Handset
        ]).subscribe(result => {
            this.isMobile = result.breakpoints[Breakpoints.HandsetPortrait];
        });
    }

    openPanel(panelName: PanelNames, arg?: any) {
        this.dialogRef.close({openPanel: {panelName: panelName, arg}});
    }

    ngOnInit() {
        if (this.data.mode === 'epicFails') {
            this.criticalData = this.usefulDataService.getEpifailData();
            this.tabs = this.epicFailsTabs;
        } else {
            this.criticalData = this.usefulDataService.getCriticalData();
            this.tabs = this.criticalSuccessTabs;
        }
    }

}
