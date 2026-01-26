import { Component, Inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MSAL_GUARD_CONFIG, MsalBroadcastService, MsalGuardConfiguration, MsalService } from '@azure/msal-angular';
import { EventMessage, EventType, InteractionStatus, PopupRequest } from '@azure/msal-browser';
import { Subject, filter, takeUntil } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit{

  title = 'PruebaAngularFront';
  isIframe = false;
  loginDisplay = false;
  private readonly _destroying$ = new Subject<void>();

  public constructor(
    @Inject(MSAL_GUARD_CONFIG) private msalGuardConfig: MsalGuardConfiguration,
    private authService: MsalService,
    private msalBroadcastService: MsalBroadcastService,
    private router: Router
  )
  {

  }

  ngOnInit(): void {
      //inicializamos la atenticacion
    this.initAuth();
    this.authService.handleRedirectObservable().subscribe();
  }

  public async login(): Promise<void> {

    //acquireTokenSilent({ ...authRequest, scopes, account })
        if (this.msalGuardConfig.authRequest) {

          this.authService.loginPopup({ ...this.msalGuardConfig.authRequest } as PopupRequest)
            .subscribe({
              next: (result) => {
                console.log(result);
                //debugger;
                //const tokenStorage = {
                //  idtoken: [result.idToken],
                //  accessToken: [result.accessToken]
                //};
                this.authService.instance.setActiveAccount(result.account);
                //obtener el client id y concatenarlo a la clave del session storage
                //const clientId = AppModule.ConfigService.getConfig().auth.clientId;
                //sessionStorage.setItem("msal.token.keys."+clientId, JSON.stringify(tokenStorage));
                //sessionStorage.setItem("access_token", result.accessToken as string);
                //this.authService.handleRedirectObservable().subscribe({
                //  next: (res: AuthenticationResult) => {
                //    // Perform actions related to user accounts here
                //    console.log(res);

                //  },
                //  error: (error) => console.log(error)
                //});
                this.setLoginDisplay();
                this.redirect();
              },
              error: (error) => console.log(error)

            });
        } else {
          this.authService.loginPopup()
            .subscribe({
              next: (result) => {
                console.log(result);
                //this.authService.handleRedirectObservable(result.accessToken);
                this.setLoginDisplay();
                this.redirect();
              },
              error: (error) => console.log(error)
            });
        }
      
    
    //first acquiresilent then interactive
    
    
  }
  redirect(): void {
    this.router.navigateByUrl('/private');
  }
  initAuth() {
    this.isIframe = window !== window.parent && !window.opener;

    //this.setLoginDisplay();

    this.authService.instance.enableAccountStorageEvents(); // Optional - This will enable ACCOUNT_ADDED and ACCOUNT_REMOVED events emitted when a user logs in or out of another tab or window

    /**
     * You can subscribe to MSAL events as shown below. For more info,
     * visit: https://github.com/AzureAD/microsoft-authentication-library-for-js/blob/dev/lib/msal-angular/docs/v2-docs/events.md
     */

    this.msalBroadcastService.msalSubject$
      .pipe(
        filter((msg: EventMessage) => msg.eventType === EventType.ACCOUNT_ADDED || msg.eventType === EventType.ACCOUNT_REMOVED),
      )
      .subscribe((result: EventMessage) => {
        if (this.authService.instance.getAllAccounts().length === 0) {
          window.location.pathname = "/";
        } else {
          this.setLoginDisplay();
        }
      });

    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this._destroying$)
      )
      .subscribe(() => {
        this.setLoginDisplay();
        this.checkAndSetActiveAccount();
      });

    //const request: SilentRequest = {
    //  authority: msalConfig.auth.authority,
    //  scopes: []
    //}
    // await this.authService.instance.acquireTokenSilent(request);

  }
  setLoginDisplay() {
    this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
  }
  checkAndSetActiveAccount() {
    /**
     * If no active account set but there are accounts signed in, sets first account to active account
     * To use active account set here, subscribe to inProgress$ first in your component
     * Note: Basic usage demonstrated. Your app may require more complicated account selection logic
     */
    const activeAccount = this.authService.instance.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      const accounts = this.authService.instance.getAllAccounts();
      // add your code for handling multiple accounts here
      

      this.authService.instance.setActiveAccount(accounts[0]);
    }
  }
}
