import {Component, Inject, OnInit} from '@angular/core';
import {MAT_DIALOG_DATA} from '@angular/material/dialog';
import {ItemTemplate} from './item-template.model';
import {forkJoin} from 'rxjs';
import {OriginService} from '../origin';
import {God, MiscService} from '../shared';
import {JobService} from '../job';

export interface ItemTemplateDialogData {
    itemTemplate: ItemTemplate;
}

@Component({
    templateUrl: './item-template-dialog.component.html',
    styleUrls: ['./item-template-dialog.component.scss'],
    standalone: false
})
export class ItemTemplateDialogComponent implements OnInit {
    public originsName: { [originId: string]: string };
    public jobsName: { [jobId: string]: string };
    public godsByTechName: { [techName: string]: God };

    constructor(
        @Inject(MAT_DIALOG_DATA) public readonly data: ItemTemplateDialogData,
        private readonly originService: OriginService,
        private readonly jobService: JobService,
        private readonly miscService: MiscService,
    ) {
    }

    ngOnInit() {
        forkJoin([
            this.jobService.getJobList(),
            this.originService.getOriginList(),
            this.miscService.getGodsByTechName(),
        ]).subscribe(([jobs, origins, godsByTechName]) => {
            this.godsByTechName = godsByTechName;
            this.jobsName = jobs.reduce((result, job) => {
                result[job.id] = job.name;
                return result;
            }, {});
            this.originsName = origins.reduce((result, origin) => {
                result[origin.id] = origin.name;
                return result;
            }, {});
        });
    }
}
