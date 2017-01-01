import {Component} from "@angular/core";
import {MdDialogRef} from "@angular/material";

@Component({
    selector: 'login-dialog',
    templateUrl: './login-dialog.component.html'
})
export class LoginDialogComponent {
    constructor(public dialogRef: MdDialogRef<LoginDialogComponent>) {
    }
}
