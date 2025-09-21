import {Component, OnInit, ViewChild} from '@angular/core';
import { NavigationStart, Router, RouterLink, RouterOutlet } from '@angular/router';

import { MatSidenav, MatSidenavContainer, MatSidenavContent } from '@angular/material/sidenav';
import {CharacterService} from '../character';
import {CharacterSummaryResponse} from '../api/responses';
import { MatNavList, MatListItem, MatListItemIcon } from '@angular/material/list';
import { MatIcon } from '@angular/material/icon';
import { CommonNavComponent } from '../shared/common-nav.component';
import { MatToolbar, MatToolbarRow } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';

@Component({
    templateUrl: './home-player.component.html',
    styleUrls: ['./home-player.component.scss'],
    imports: [MatSidenavContainer, MatSidenav, MatNavList, MatListItem, RouterLink, MatIcon, MatListItemIcon, CommonNavComponent, MatSidenavContent, MatToolbar, MatToolbarRow, MatIconButton, RouterOutlet]
})
export class HomePlayerComponent implements OnInit {
    public characters: CharacterSummaryResponse[];

    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;

    constructor(
        private readonly characterService: CharacterService,
        private readonly router: Router,
    ) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    ngOnInit() {
        this.characterService.loadList().subscribe(
            characters => {
                this.characters = characters.slice(0, 5);
            }
        );
    }
}

