import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { addIcons } from 'ionicons';
import { addOutline, refreshOutline, createOutline, trashOutline, playOutline, checkmarkOutline, returnUpBackOutline, closeOutline } from 'ionicons/icons';

import { AppModule } from './app/app.module';

// Register commonly used Ionicons globally to avoid network requests for SVGs
try {
  addIcons({ addOutline, refreshOutline, createOutline, trashOutline, playOutline, checkmarkOutline, returnUpBackOutline, closeOutline });
} catch (e) {
  // ignore in non-browser contexts
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
