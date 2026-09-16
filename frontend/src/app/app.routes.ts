import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'decisions',
    pathMatch: 'full'
  },
  {
    path: 'decisions',
    loadComponent: () =>
      import('./features/decisions/decision-list/decision-list')
        .then(m => m.DecisionListComponent)
  },
  {
    path: 'decisions/new',
    loadComponent: () =>
      import('./features/decisions/decision-form/decision-form')
        .then(m => m.DecisionFormComponent)
  },
  {
  path: 'decisions/:id/wizard',
  loadComponent: () =>
    import('./features/decisions/decision-wizard/decision-wizard')
      .then(m => m.DecisionWizardComponent)
  }
];