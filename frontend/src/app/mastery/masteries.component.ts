import {ChangeDetectionStrategy, Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Subscription} from 'rxjs';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MasteryService} from './mastery.service';
import {MasteryGroupResponse} from '../api/responses';
import {EntryGroupTabsComponent} from '../shared/entry-group-tabs/entry-group-tabs.component';
import {EntryGroupView, toEntryView} from '../shared/entry-group-tabs/entry-group-tabs.model';

@Component({
    selector: 'app-masteries',
    imports: [
        MatProgressSpinner,
        EntryGroupTabsComponent,
    ],
    template: `
        @if (!groups) {
          <mat-spinner></mat-spinner>
        } @else {
          <entry-group-tabs title="Maîtrises" [groups]="groups" [selectedGroupId]="selectedGroupId"
                            (selectedGroupIdChange)="selectGroup($event)"></entry-group-tabs>
        }
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
})
export class MasteriesComponent implements OnInit, OnDestroy {
    groups?: EntryGroupView[];
    selectedGroupId?: string;

    private readonly subscription = new Subscription();

    constructor(
        private readonly service: MasteryService,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.route.params.subscribe(params => {
            this.selectedGroupId = params['groupId'];
        }));
        this.subscription.add(this.service.getMasteryGroups().subscribe(groups => {
            this.groups = this.toGroupViews(groups);
        }));
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    selectGroup(groupId: string) {
        this.router.navigate(['/database/masteries', groupId], {replaceUrl: true});
    }

    private toGroupViews(groups: MasteryGroupResponse[]): EntryGroupView[] {
        const namesById = new Map(groups.flatMap(g => g.masteries).map(e => [e.id, e.name]));
        return groups.map(group => ({
            id: group.id,
            name: group.name,
            description: group.description,
            entries: group.masteries.map(e => toEntryView(e, e.data.prerequisiteMasteryId, namesById)),
        }));
    }
}
