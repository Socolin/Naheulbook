import {Observable, throwError, timer} from 'rxjs';
import {mergeMap, finalize} from 'rxjs/operators';

// from https://www.learnrxjs.io/operators/error_handling/retrywhen.html

export const genericRetryStrategy = ({
                                         maxRetryAttempts = 4,
                                         scalingDuration = 1000,
                                         excludedStatusCodes = []
                                     }: {
    maxRetryAttempts?: number,
    scalingDuration?: number,
    excludedStatusCodes?: number[]
} = {}) => (attempts: Observable<any>) => {
    return attempts.pipe(
        mergeMap((error, i) => {
            const retryAttempt = i + 1;
            // if maximum number of retries have been met
            // or response is a status code we don't wish to retry, throw error
            if (
                retryAttempt > maxRetryAttempts ||
                excludedStatusCodes.find(e => e === error.status)
            ) {
                return throwError(error);
            }
            // retry after 1s, 2s, etc...
            return timer(retryAttempt * scalingDuration);
        })
    );
};
