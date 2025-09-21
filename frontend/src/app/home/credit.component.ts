import {Component} from '@angular/core';
import { MatSidenavContainer } from '@angular/material/sidenav';
import { MatToolbar } from '@angular/material/toolbar';
import { RouterLink } from '@angular/router';
import { MatIcon } from '@angular/material/icon';

@Component({
    templateUrl: './credit.component.html',
    styleUrls: ['./credit.component.scss'],
    imports: [MatSidenavContainer, MatToolbar, RouterLink, MatIcon]
})
export class CreditComponent {
}
