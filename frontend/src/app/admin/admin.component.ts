import {ChangeDetectionStrategy, Component, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {NavigationStart, Router, RouterLink, RouterOutlet} from '@angular/router';
import {MatSidenav, MatSidenavContainer, MatSidenavContent} from '@angular/material/sidenav';
import {MatListItem, MatListItemIcon, MatNavList} from '@angular/material/list';
import {MatIcon} from '@angular/material/icon';
import {MatToolbar, MatToolbarRow} from '@angular/material/toolbar';
import {MatIconButton} from '@angular/material/button';
import {Subscription} from 'rxjs';
import {CommonNavComponent} from '../shared/common-nav.component';

@Component({
    templateUrl: './admin.component.html',
    styleUrls: ['./admin.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [MatSidenavContainer, MatSidenav, MatSidenavContent, MatNavList, MatListItem, MatListItemIcon, RouterLink, MatIcon, CommonNavComponent, MatToolbar, MatToolbarRow, MatIconButton, RouterOutlet]
})
export class AdminComponent implements OnInit, OnDestroy {
    @ViewChild('start', {static: true, read: MatSidenav})
    public start: MatSidenav;
    private subscription = new Subscription();

    constructor(
        private readonly router: Router,
    ) {
    }

    ngOnInit() {
        this.subscription.add(this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.start.close().then();
            }
        }));
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe();
    }
}
