import { NavigationExtras, Router } from '@angular/router';
import { AlertifyService } from './alertify.service';
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private alertify:AlertifyService, private router: Router){}
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        //intercept the response
        return next.handle(req).pipe(
            catchError(error => {
                //handle unauthorizederror
                if (error.status === 401){
                    this.alertify.error(error.statusText);
                    //return throwError(error.statusText);
                }

                else if(error.status === 400){
                    if(error.error.errors){
                        let modalError = [];
                        for (const key in error.error.errors) {
                            if (error.error.errors[key]) {
                                modalError.push(error.error.errors[key])
                            }
                        }
                        throwError(modalError.flat());
                    }

                    else if(typeof(error.error) === 'object'){
                        this.alertify.error(error.statusText);
                    }

                    else{
                        this.alertify.error(error.error);
                    }
                }

                else if(error.status === 404){
                    this.router.navigateByUrl('/not-found');
                }

                else if(error.status === 500){
                    const navigationExtra: NavigationExtras = {
                        state: {error: error.error}
                    }
                    this.router.navigateByUrl('/server-error', navigationExtra);
                }
                else{
                    this.alertify.error('Something went wrong');
                    console.log(error);
                }
                // if (error instanceof HttpErrorResponse) {
                //     let serverError = error.error;
                //     let modalError = '';
                //     if (serverError.errors && typeof (serverError.errors )=== 'object') {
                //         for (const key in serverError.errors) {
                //             if (serverError.errors[key]) {
                //                 modalError += serverError.errors[key] + '\n';
                //             }
                //         }
                //     }
                //    return throwError(modalError || serverError || 'Server error occured'); 
                // }
                return throwError(error);
            })
        )
    }
}

export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: ErrorInterceptor,
    multi: true //not to overide but to add to the http interceptors that came with angular
}
