import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';

import {ThemeService} from './theme.service';
import { RouterOutlet } from '@angular/router';
import { QuickCommandComponent } from './quick-command/quick-command.component';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    imports: [RouterOutlet, QuickCommandComponent]
})
export class AppComponent implements OnInit {
    public initialized = false;
    constructor(
        private readonly themeService: ThemeService
    ) {
    };

    ngOnInit() {
        this.themeService.updateTheme();
    }
}

