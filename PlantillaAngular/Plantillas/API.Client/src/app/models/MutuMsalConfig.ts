import { BrowserAuthOptions, BrowserSystemOptions, CacheOptions } from '@azure/msal-browser';

export class MutuMsalConfig /*extends Configuration = */ {
  public auth: BrowserAuthOptions;
  public cache?: CacheOptions;
  public system?: BrowserSystemOptions;

  public constructor() {
    this.auth = {
      clientId: '',

    };
    this.cache = {};
    this.system = {};
  }

}
