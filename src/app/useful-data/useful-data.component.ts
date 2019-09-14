import {Component, EventEmitter, OnDestroy, OnInit, Output} from '@angular/core';
import {Portal} from '@angular/cdk/portal';
import {UsefulDataService} from './useful-data.service';
import {MatDialog} from '@angular/material/dialog';
import {
    EpicFailsCriticalSuccessDialogComponent,
    EpicFailsCriticalSuccessDialogData
} from './dialogs/epic-fails-critical-success-dialog.component';
import {PanelNames, UsefulDataDialogsService} from './useful-data-dialogs.service';
import {assertNever} from '../utils/utils';
import {Subscription} from 'rxjs';

@Component({
    selector: 'useful-data',
    styleUrls: ['./useful-data.component.scss'],
    templateUrl: './useful-data.component.html'
})
export class UsefulDataComponent implements OnInit, OnDestroy {
    @Output() onAction = new EventEmitter<{ action: string, data: any }>();
    public effectsCategoryId = 1;
    public panelByNames: { [name: string]: Portal<any> } = {};
    private subscription: Subscription = new Subscription();

    constructor(
        private usefulDataService: UsefulDataService,
        private usefulDataDialogsService: UsefulDataDialogsService,
        private dialog: MatDialog
    ) {

    }

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        return false;
    }

    openPanel(panel: PanelNames, arg?: any) {
        switch (panel) {
            case 'epicFails':
                this.dialog.open<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent, {
                        minWidth: '100vw',
                        height: '100vh',
                        autoFocus: false,
                        data: {mode: 'epicFails'}
                    });
                break;
            case 'criticalSuccess':
                this.dialog.open<EpicFailsCriticalSuccessDialogComponent, EpicFailsCriticalSuccessDialogData>(
                    EpicFailsCriticalSuccessDialogComponent, {
                        minWidth: '100vw',
                        height: '100vh',
                        autoFocus: false,
                        data: {mode: 'criticalSuccess'}
                    });
                break;
            case 'effects':
            case 'entropicSpells':
            case 'items':
            case 'jobs':
            case 'origins':
            case 'skills':
            case 'sleep':
                break;
            default:
                assertNever(panel);
        }
    }

    ngOnInit(): void {
        this.subscription.add(this.usefulDataDialogsService.onOpenPanel(this.openPanel.bind(this)));
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}
