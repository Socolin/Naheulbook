import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {QuickCommandComponent} from './quick-command.component';
import {PortalModule} from '@angular/cdk/portal';
import {MatRippleModule} from '@angular/material/core';
import {MatIconModule} from '@angular/material/icon';
import {SharedModule} from '../shared/shared.module';

@NgModule({
    declarations: [
        QuickCommandComponent
    ],
    exports: [
        QuickCommandComponent
    ],
    imports: [
        CommonModule,
        PortalModule,
        MatRippleModule,
        MatIconModule,
        SharedModule,
    ]
})
export class QuickCommandModule {
}
