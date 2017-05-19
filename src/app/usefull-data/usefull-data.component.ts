import {Component, OnInit, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {UsefullDataService} from './usefull-data.service';
import {CriticalData} from './usefull-data.model';

@Component({
    selector: 'usefull-data',
    styleUrls: ['./usefull-data.component.scss'],
    templateUrl: './usefull-data.component.html',
})
export class UsefullDataComponent implements OnInit {
    public currentPanel: string = null;
    public currentSubPanel: string = null;
    public selectedSubPanels = {};
    public effectsCategoryId = 1;
    public panelByNames: {[name: string]: Portal<any>} = {};

    public currentOverlayRef: OverlayRef;

    @ViewChild('panelCritic')
    public panelCriticDialog: Portal<any>;
    public criticData: {[name: string]: CriticalData[]} = {};

    @ViewChild('panelEpicfail')
    public panelEpicfailDialog: Portal<any>;

    @ViewChild('panelItems')
    public panelItemsDialog: Portal<any>;

    @ViewChild('panelJobs')
    public panelJobsDialog: Portal<any>;

    @ViewChild('panelOrigins')
    public panelOriginsDialog: Portal<any>;

    @ViewChild('panelEffects')
    public panelEffectsDialog: Portal<any>;

    @ViewChild('panelSkills')
    public panelSkillsDialog: Portal<any>;

    @ViewChild('panelEntropic')
    public panelEntropicDialog: Portal<any>;

    @ViewChild('panelSleep')
    public panelSleepDialog: Portal<any>;

    constructor(private _nhbkDialogService: NhbkDialogService
        , private _usefullDataService: UsefullDataService) {

    }

    toggleDisplayPanel(name: string) {
        if (this.currentOverlayRef) {
            this.currentOverlayRef.detach();
            this.currentOverlayRef = null;
        }

        this._nhbkDialogService.openTopCenteredBackdropDialog(this.panelByNames[name]).detachments().subscribe(
            () =>  {
                if (this.currentPanel === name) {
                    this.currentPanel = null;
                }
            }
        );

        if (this.currentPanel === name) {
            this.currentPanel = null;
        } else {
            if (name) {
                this.currentSubPanel = this.selectedSubPanels[name];
            }
            this.currentPanel = name;
        }
        return false;
    }

    selectSubPanel(name) {
        this.currentSubPanel = name;
        this.selectedSubPanels[this.currentPanel] = name;
        return false;
    }

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        this.currentPanel = 'effects';
        return false;
    }

    ngOnInit(): void {
        this.panelByNames['critic'] = this.panelCriticDialog;
        this.panelByNames['epicfail'] = this.panelEpicfailDialog;
        this.panelByNames['items'] = this.panelItemsDialog;
        this.panelByNames['jobs'] = this.panelJobsDialog;
        this.panelByNames['origins'] = this.panelOriginsDialog;
        this.panelByNames['effects'] = this.panelEffectsDialog;
        this.panelByNames['skills'] = this.panelSkillsDialog;
        this.panelByNames['entropic'] = this.panelEntropicDialog;
        this.panelByNames['sleep'] = this.panelSleepDialog;
        this.criticData = this._usefullDataService.getCriticalData();
    }
}
