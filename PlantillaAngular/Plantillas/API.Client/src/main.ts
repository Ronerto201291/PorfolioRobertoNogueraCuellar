import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { provideAnimations } from '@angular/platform-browser/animations';


platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
