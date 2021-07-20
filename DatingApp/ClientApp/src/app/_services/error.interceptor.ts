import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => {
                if (error.status === 401){
                    return throwError(error.statusText);
                }
                if (error instanceof HttpErrorResponse) {
                    let serverError = error.error;
                    let modalError = '';
                    if (serverError.errors && typeof (serverError.errors )=== 'object') {
                        for (const key in serverError.errors) {
                            if (serverError.errors[key]) {
                                modalError += serverError.errors[key] + '\n';
                            }
                        }
                    }
                   return throwError(modalError || serverError || 'Server error occured'); 
                }
            })
        )
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true
}
