import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {MatDialogRef} from '@angular/material/dialog';
import {ComponentType} from '@angular/cdk/overlay';

import {NhbkMatDialog} from '../material-workaround';

import {
    EffectListDialogComponent,
    EffectListDialogData,
    EntropicSpellsDialogComponent,
    EpicFailsCriticalSuccessDialogComponent,
    EpicFailsCriticalSuccessDialogData,
    ItemTemplatesDialogComponent,
    JobsDialogComponent,
    OriginsDialogComponent,
    RecoveryDialogComponent,
    SkillsDialogComponent,
    TravelDialogComponent,
    UsefulDataDialogResult,
} from './dialogs';
import {assertNever} from '../utils/utils';
import {PanelNames} from './useful-data.model';
import {MonstersDialogComponent} from '../monster/monsters-dialog.component';
import {CommandSuggestionType, QuickAction, QuickCommandService} from '../quick-command';
import { MatFabButton } from '@angular/material/button';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatIcon } from '@angular/material/icon';

class InfoDefinition {
    displayName: string;
    icon: string;
    tab?: string;
    fontSet: string;
    panel: PanelNames;
}
@Component({
    selector: 'useful-data',
    styleUrls: ['./useful-data.component.scss'],
    templateUrl: './useful-data.component.html',
    imports: [MatFabButton, MatMenuTrigger, MatIcon, MatMenu, MatMenuItem]
})
export class UsefulDataComponent implements OnInit, OnDestroy {
    @Output() actionTriggered = new EventEmitter<{ action: string, data: any }>();
    public basesInfoDefinitions: InfoDefinition[] = [
        { displayName: 'Sorts entropique', icon: 'game-icon-bolt-spell-cast', fontSet: 'game-icon', panel: 'entropicSpells' },
        { displayName: 'Récupération', icon: 'game-icon-sliced-bread', fontSet: 'game-icon', panel: 'recovery' },
        { displayName: 'Déplacements', icon: 'game-icon-walk', fontSet: 'game-icon', panel: 'travel'},
    ];

    public criticalSuccessInfoDefinitions: InfoDefinition[] = [
        { displayName: 'Arme tranchante', icon: 'game-icon-broad-dagger', fontSet: 'game-icon', panel: 'criticalSuccess', tab: 'sharp' },
        { displayName: 'Arme contondante', icon: 'game-icon-thor-hammer', fontSet: 'game-icon', panel: 'criticalSuccess', tab: 'blunt' },
        { displayName: 'Parade', icon: 'game-icon-shield-reflect', fontSet: 'game-icon', panel: 'criticalSuccess', tab: 'parade' },
        { displayName: 'Mains nues', icon: 'game-icon-punch', fontSet: 'game-icon', panel: 'criticalSuccess', tab: 'hand' },
        { displayName: 'Projectiles', icon: 'game-icon-high-shot', fontSet: 'game-icon', panel: 'criticalSuccess', tab: 'projectile' },
    ];

    public epicFailsInfoDefinitions: InfoDefinition[] = [
        { displayName: 'Attaque ou parade', icon: 'game-icon-broad-dagger', fontSet: 'game-icon', panel: 'epicFails', tab: 'combat' },
        { displayName: 'Lancement du sort', icon: 'game-icon-thunder-struck', fontSet: 'game-icon', panel: 'epicFails', tab: 'magic' },
        { displayName: 'Arme de jet', icon: 'game-icon-pierced-heart', fontSet: 'game-icon', panel: 'epicFails', tab: 'arrow' },
        { displayName: 'Mains nues', icon: 'game-icon-fist', fontSet: 'game-icon', panel: 'epicFails', tab: 'hand' },
    ];

    public databaseInfoDefinitions: InfoDefinition[] = [
        { displayName: 'Effets', icon: 'game-icon-pierced-body', fontSet: 'game-icon', panel: 'effects' },
        { displayName: 'Objets', icon: 'game-icon-beer-stein', fontSet: 'game-icon', panel: 'items' },
        { displayName: 'Compétences', icon: 'game-icon-gears', fontSet: 'game-icon', panel: 'skills' },
        { displayName: 'Métiers', icon: 'game-icon-gear-hammer', fontSet: 'game-icon', panel: 'jobs' },
        { displayName: 'Origines', icon: 'game-icon-woman-elf-face', fontSet: 'game-icon', panel: 'origins' },
        { displayName: 'Monstres', icon: 'game-icon-dragon-head', fontSet: 'game-icon', panel: 'monsters' },
    ];

    constructor(
        private readonly dialog: NhbkMatDialog,
        private readonly quickCommandService: QuickCommandService,
    ) {
    }


    openPanel(panel: PanelNames, arg?: any) {
        let dialogRef: MatDialogRef<any, UsefulDataDialogResult>;
        switch (panel) {
            case 'epicFails':
                dialogRef = this.openDialog<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent,
                    {mode: 'epicFails', selectedTab: arg}
                );
                break;
            case 'criticalSuccess':
                dialogRef = this.openDialog<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent,
                    {mode: 'criticalSuccess', selectedTab: arg}
                );
                break;
            case 'effects':
                dialogRef = this.openDialog<EffectListDialogComponent, EffectListDialogData>(EffectListDialogComponent,
                    {inputSubCategoryId: arg}
                );
                break;
            case 'entropicSpells':
                dialogRef = this.openDialog(EntropicSpellsDialogComponent);
                break;
            case 'recovery':
                dialogRef = this.openDialog(RecoveryDialogComponent);
                break;
            case 'travel':
                dialogRef = this.openDialog(TravelDialogComponent);
                break;
            case 'items':
                dialogRef = this.openDialog(ItemTemplatesDialogComponent);
                break;
            case 'jobs':
                dialogRef = this.openDialog(JobsDialogComponent);
                break;
            case 'origins':
                dialogRef = this.openDialog(OriginsDialogComponent);
                break;
            case 'skills':
                dialogRef = this.openDialog(SkillsDialogComponent);
                break;
            case 'monsters':
                dialogRef = this.openDialog(MonstersDialogComponent);
                break;
            default:
                assertNever(panel);
                // FIXME: Remove that when `asserts` is available
                throw new Error('Waiting for asserts');
        }

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            if (result.openPanel) {
                setTimeout(() => this.openPanel(result.openPanel!.panelName, result.openPanel!.arg), 100)
            }
            if (result.action) {
                this.actionTriggered.next(result.action);
            }
        })
    }

    ngOnInit(): void {
        const actions = this.basesInfoDefinitions.concat(this.databaseInfoDefinitions).map(info => this.createQuickCommand(info));
        this.quickCommandService.registerActions('usefull-data', actions);
    }

    private createQuickCommand(info: InfoDefinition): QuickAction {
        return {
            icon: info.icon,
            iconFontSet: info.fontSet,
            displayText: info.displayName,
            type: CommandSuggestionType.Info,
            priority: 25,
            canBeUsedInRecent: true,
            action: () => this.openPanel(info.panel)
        }
    }

    ngOnDestroy(): void {
        this.quickCommandService.unregisterActions('usefull-data');
    }

    private openDialog<TDialog, TData = any>(componentType: ComponentType<TDialog>, data?: TData) {
        return this.dialog.openFullScreen<TDialog, any, UsefulDataDialogResult>(
            componentType, {
                data: data
            });
    }
}
