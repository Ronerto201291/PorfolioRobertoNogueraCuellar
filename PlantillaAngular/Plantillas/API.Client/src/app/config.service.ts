import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';


export let recaptchaKey: string;
export let coinscrapUrl: string;



@Injectable({ providedIn: 'root' })
export class ConfigService {
    constructor(public _http: HttpClient) {
      
  }

    

  public  getConfig() {
    //return this.msalConfig as Configuration;
  }

  loadConfig() {

    //return lastValueFrom<Configuration>(this._http.get<Configuration>('/assets/msal.json'))
    //  .then(resp => {
    //    this.msalConfig.auth = resp.auth;
    //    this.msalConfig.cache = resp.cache;
    //    this.msalConfig.system = resp.system;
    //    return this.msalConfig;
    //  });
        //.catch((error: string) => {
        //  console.error('No se pudo cargar la configuración' + error ? error : null);
        //});
    }
}
