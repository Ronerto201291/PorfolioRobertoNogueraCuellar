import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router } from '@angular/router';
// MSAL typings can be strict; use `as any` where necessary to avoid blocking compilation
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore
import { MSAL_GUARD_CONFIG, MsalBroadcastService, MsalService } from '@azure/msal-angular';
// Keep rxjs imports minimal and use inject()
import { Subject, filter, takeUntil } from 'rxjs';
import { EventMessage, EventType, InteractionStatus, PopupRequest } from '@azure/msal-browser';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
  // dependencies via inject()
  private readonly router = inject(Router);
  private readonly authService = inject(MsalService) as any;
  private readonly msalBroadcastService = inject(MsalBroadcastService) as any;
  private readonly msalGuardConfig = inject(MSAL_GUARD_CONFIG) as any;

  title = 'PruebaAngularFront';
  isIframe = false;
  loginDisplay = false;
  private readonly _destroying$ = new Subject<void>();

  ngOnInit(): void {
    this.initAuth();
    // handle redirect observable, no unused vars
    if (typeof this.authService.handleRedirectObservable === 'function') {
      this.authService.handleRedirectObservable();
    }
  }

  ngOnDestroy(): void {
    this._destroying$.next();
    this._destroying$.complete();
  }

  public login(): void {
    const loginRequest = this.msalGuardConfig?.authRequest ? ({ ...this.msalGuardConfig.authRequest } as PopupRequest) : undefined;
    // use as any to avoid strict msal typing issues
    this.authService.loginPopup(loginRequest).subscribe({
      next: (result: any) => {
        // set active account and update UI
        try { this.authService.instance.setActiveAccount(result?.account); } catch { /* ignore */ }
        this.setLoginDisplay();
        this.router.navigateByUrl('/private');
      },
      error: () => {
        // ignore errors here to avoid unused var warnings
      }
    });
  }

  private initAuth(): void {
    this.isIframe = window !== window.parent && !window.opener;

    // subscribe to MSAL broadcast events - no unused vars in subscribers
    this.msalBroadcastService.msalSubject$
      .pipe(filter((msg: EventMessage) => msg.eventType === EventType.ACCOUNT_ADDED || msg.eventType === EventType.ACCOUNT_REMOVED), takeUntil(this._destroying$))
      .subscribe(() => {
        if (this.authService.instance.getAllAccounts().length === 0) {
          window.location.pathname = '/';
        } else {
          this.setLoginDisplay();
        }
      });

    this.msalBroadcastService.inProgress$.pipe(filter((s: any) => s === InteractionStatus.None), takeUntil(this._destroying$)).subscribe(() => {
      this.setLoginDisplay();
      this.checkAndSetActiveAccount();
    });
  }

  private setLoginDisplay(): void {
    try {
      this.loginDisplay = this.authService.instance.getAllAccounts().length > 0;
    } catch {
      this.loginDisplay = false;
    }
  }

  private checkAndSetActiveAccount(): void {
    try {
      const active = this.authService.instance.getActiveAccount();
      if (!active) {
        const accounts = this.authService.instance.getAllAccounts();
        if (accounts && accounts.length) this.authService.instance.setActiveAccount(accounts[0]);
      }
    } catch { /* ignore msal errors during init */ }
  }
}
