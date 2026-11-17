import {Component, Input, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {Fighter} from './group.model';

@Component({
    selector: 'fighter-icon',
    templateUrl: './fighter-icon.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    styleUrls: ['./fighter-icon.component.scss']
})
export class FighterIconComponent implements OnInit {
    @Input() fighter: Fighter;
    @Input() fontSize = '2em';

    constructor() {
    }

    ngOnInit() {
    }

}
