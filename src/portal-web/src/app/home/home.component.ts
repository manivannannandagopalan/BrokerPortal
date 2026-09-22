import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  standalone: true,
  template: '<main><h1>Broker Portal</h1><p>Application shell ready.</p></main>',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomeComponent {}