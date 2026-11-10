import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from "../../environments/environment.development";

export const baseUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (!req.url.startsWith('http')) {
    let path = req.url;
    if (path) path = path.slice(1)
    const url = `${environment.apiUrl}/${path}`
    const apiReq = req.clone({ url })
    return next(apiReq)
  }
  
  return next(req);
}