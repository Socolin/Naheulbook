import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {NotificationsModule} from '../notifications/notifications.module';
import {CharacterModule} from '../character/character.module';
import {UserModule} from '../user/user.module';

import {HomePlayerComponent} from './home-player.component';
import {RouterModule} from '@angular/router';

import {routes} from './home-player.routes';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        MaterialModule,
        FlexLayoutModule,
        RouterModule.forChild(routes),
        NotificationsModule,
        UserModule,
        CharacterModule,
    ],
    declarations: [
        HomePlayerComponent,
    ],
    exports: [
        HomePlayerComponent,
    ],
    providers: [
    ],
})
export class PlayerHomeModule {
}
