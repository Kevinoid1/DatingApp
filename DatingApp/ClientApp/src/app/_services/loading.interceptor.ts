import { LoadingService } from './loading.service';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HTTP_INTERCEPTORS } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { delay, finalize } from "rxjs/operators";


@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

    constructor(private loadingService: LoadingService) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        this.loadingService.show()
        return next.handle(request).pipe(
            delay(5000),
            finalize(() => this.loadingService.hide())
        )
    }

}

export const LoadingInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass: LoadingInterceptor,
    multi : true
} 