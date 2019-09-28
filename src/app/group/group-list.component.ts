import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {Group} from './group.model';
import {GroupService} from './group.service';
import {GroupSummaryResponse} from '../api/responses';

@Component({
    templateUrl: './group-list.component.html',
    styleUrls: ['./group-list.component.scss']
})
export class GroupListComponent implements OnInit {
    public groups: GroupSummaryResponse[];
    public loading = true;

    constructor(
        private readonly groupService: GroupService,
        private readonly notification: NotificationsService,
        private readonly router: Router,
    ) {
    }

    selectGroup(group: Group) {
        this.router.navigate(['/gm/group/', group.id]);
        return false;
    }

    createGroup() {
        this.router.navigate(['/gm/group/create']);
        return false;
    }

    loadGroups() {
        this.groupService.listGroups().subscribe(
            res => {
                this.groups = res;
                this.loading = false;
            }
        );
    }

    ngOnInit() {
        this.loadGroups();
    }
}
