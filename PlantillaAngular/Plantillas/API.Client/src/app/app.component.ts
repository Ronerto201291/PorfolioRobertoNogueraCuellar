/* eslint-disable @typescript-eslint/no-explicit-any */
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { AccountInfo } from '@azure/msal-browser';
import { Subject } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent  implements OnInit, OnDestroy {
  title = 'PruebaAngularFront';
  isIframe = false;
  loginDisplay = false;

  public sidebarItems = signal<{ title: string; url: string; icon: string }[] | null>(null);

  private readonly _destroying$ = new Subject<void>();
  public loading: boolean = false;
  public historicoVisible: boolean = true;
  public Account!: AccountInfo;
  public selectedOption: number = 1;
  public treeRoutes: { title: string, url: string, icon: string }[] = [];


  constructor(
    private router: Router,private activatedRoute : ActivatedRoute
  ) { }
  
  async ngOnInit(): Promise<void> {
    this.isIframe = window !== window.parent && !window.opener;
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => {
        this.treeRoutes = this.getTreeRoutes(this.activatedRoute.root);
      });

  }


  // unsubscribe to events when component is destroyed
  ngOnDestroy(): void {
    this._destroying$.next();
    this._destroying$.complete();
  }


  selectOption(option: number) {

    if (option == 1) {
      this.login();
    }

  }

  public async login(): Promise<void> {
    //navegamos a una ruta que requiera autenticacion. MSAl maneja la ventana de login
    this.router.navigateByUrl('/privatelogin');

  }

  private getTreeRoutes(route: ActivatedRoute, url: string = '', items: { title: string, url: string, icon: string }[] = []): { title: string, url: string, icon: string }[] {
    const children: ActivatedRoute[] = route.children;

    if (children.length === 0) {
      return items;
    }

    for (const child of children) {
      const routeURL: string = child.snapshot.url.map(segment => segment.path).join('/');
      if (routeURL !== '') {
        url += `/${routeURL}`;
      }

      const data = child.snapshot.data as { title: string, url: string, icon: string };
      if (data) {
        items.push(data);
      }

      return this.getTreeRoutes(child, url, items);
    }

    return items;
  }


}
