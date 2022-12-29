import {Component, Input, OnInit} from '@angular/core';
import {Fighter} from './group.model';

@Component({
    selector: 'fighter-icon',
    templateUrl: './fighter-icon.component.html',
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
