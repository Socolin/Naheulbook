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

@Component({
    selector: 'useful-data',
    styleUrls: ['./useful-data.component.scss'],
    templateUrl: './useful-data.component.html'
})
export class UsefulDataComponent implements OnInit, OnDestroy {
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();

    constructor(
        private readonly dialog: NhbkMatDialog,
    ) {
    }

    openPanel(panel: PanelNames, arg?: any) {
        let dialogRef: MatDialogRef<any, UsefulDataDialogResult>;
        switch (panel) {
            case 'epicFails':
                dialogRef = this.openDialog<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent,
                    {mode: 'epicFails'}
                );
                break;
            case 'criticalSuccess':
                dialogRef = this.openDialog<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent,
                    {mode: 'criticalSuccess'}
                );
                break;
            case 'effects':
                dialogRef = this.openDialog<EffectListDialogComponent, EffectListDialogData>(EffectListDialogComponent,
                    {inputCategoryId: arg}
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
                this.openPanel(result.openPanel.panelName, result.openPanel.arg);
            }
            if (result.action) {
                this.onAction.next(result.action);
        }
        })
    }

    ngOnInit(): void {
    }

    ngOnDestroy(): void {
    }

    private openDialog<TDialog, TData = any>(componentType: ComponentType<TDialog>, data?: TData) {
        return this.dialog.openFullScreen<TDialog, any, UsefulDataDialogResult>(
            componentType, {
                data: data
            });
    }
}
