import { Component, OnInit } from '@angular/core';
import { MatSidenavContainer, MatSidenav } from '@angular/material/sidenav';
import { MatNavList, MatListItem } from '@angular/material/list';
import { RouterLink, RouterOutlet } from '@angular/router';
import { MatIcon } from '@angular/material/icon';
import { CommonNavComponent } from '../shared/common-nav.component';
import { MatToolbar, MatToolbarRow } from '@angular/material/toolbar';
import { MatIconButton } from '@angular/material/button';

@Component({
    selector: 'app-map-home',
    templateUrl: './map-home.component.html',
    styleUrls: ['./map-home.component.scss'],
    imports: [MatSidenavContainer, MatSidenav, MatNavList, MatListItem, RouterLink, MatIcon, CommonNavComponent, MatToolbar, MatToolbarRow, MatIconButton, RouterOutlet]
})
export class MapHomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
