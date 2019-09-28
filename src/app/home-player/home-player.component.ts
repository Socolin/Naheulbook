import {Component, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router} from '@angular/router';

import {MatSidenav} from '@angular/material';
import {CharacterService} from '../character';
import {CharacterSummaryResponse} from '../api/responses';

@Component({
    templateUrl: './home-player.component.html',
    styleUrls: ['./home-player.component.scss'],
})
export class HomePlayerComponent implements OnInit {
    public characters: CharacterSummaryResponse[];

    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;

    constructor(private _characterService: CharacterService
        , public _router: Router) {
        this._router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        })
    };

    ngOnInit() {
        this._characterService.loadList().subscribe(
            characters => {
                this.characters = characters.slice(0, 5);
            }
        );
    }
}

