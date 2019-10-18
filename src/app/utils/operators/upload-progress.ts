import {HttpEvent, HttpEventType} from '@angular/common/http';

import {tap} from 'rxjs/operators';

export function uploadProgress<T>(cb: (progress: number | undefined) => void) {
    return tap((event: HttpEvent<T>) => {
        if (event.type === HttpEventType.UploadProgress) {
            if (!event.total) {
                cb(undefined);
                return;
            }
            cb((100 * event.loaded) / event.total);
        }
    });
}
