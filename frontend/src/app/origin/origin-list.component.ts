import {Component, OnInit} from '@angular/core';

import {Origin} from './origin.model';
import {OriginService} from './origin.service';
import {OriginComponent} from './origin.component';

@Component({
    selector: 'origin-list',
    templateUrl: './origin-list.component.html',
    styleUrls: ['./origin-list.component.scss'],
    imports: [OriginComponent]
})
export class OriginListComponent implements OnInit {
    public origins: Origin[];

    constructor(
        private readonly originService: OriginService
    ) {
    }

    getOrigins() {
        this.originService.getOriginList().subscribe(
            origins => {
                this.origins = origins;
            }
        );
    }

    ngOnInit() {
        this.getOrigins();
    }
}
