import {Injectable} from '@angular/core';
import {Subject} from 'rxjs';

@Injectable()
export class ErrorReportService {
    public notifyError: Subject<{message: string, error: any}> = new Subject<{message: string, error: any}>();

    notify(message: string, error: any) {
        this.notifyError.next({message: message, error: error});
    }
}
