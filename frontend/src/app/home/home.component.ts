import {Component} from '@angular/core';
import { MatSidenavContainer } from '@angular/material/sidenav';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { MatCard, MatCardContent } from '@angular/material/card';

@Component({
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    imports: [MatSidenavContainer, MatButton, RouterLink, MatCard, MatCardContent]
})
export class HomeComponent {
}
