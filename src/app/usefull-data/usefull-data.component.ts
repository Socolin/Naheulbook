import {Component, OnInit, ViewChild} from '@angular/core';
import {Portal} from '@angular/material';
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
    public effectsCategoryId = 1;
    public panelByNames: {[name: string]: Portal<any>} = {};

    @ViewChild('panelCritic')
    public panelCriticDialog: Portal<any>;
    public criticData: {[name: string]: CriticalData[]} = {};

    @ViewChild('panelEpicfail')
    public panelEpicfailDialog: Portal<any>;
    public epicfailData: {[name: string]: CriticalData[]} = {};

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

    openPanel(name: string) {
        if (!this.currentPanel) {
            this.currentPanel = name;
        }
        this._nhbkDialogService.openTopCenteredBackdropDialog(this.panelByNames[name]).detachments().subscribe(
            () =>  {
                if (this.currentPanel === name) {
                    this.currentPanel = null;
                }
            }
        );
        return false;
    }

    showEffects(categoryId) {
        this.effectsCategoryId = categoryId;
        this.openPanel('effects');
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
        this.epicfailData = this._usefullDataService.getEpifailData();
    }
}
