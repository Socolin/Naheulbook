import {Component, OnInit, Input} from '@angular/core';
import {NhbkAction} from './nhbk-action.model';

@Component({
    selector: 'nhbk-action',
    styleUrls: ['./nhbk-action.component.scss'],
    templateUrl: './nhbk-action.component.html'
})
export class NhbkActionComponent implements OnInit {
    @Input() action: NhbkAction;

    ngOnInit(): void {
    }
}
